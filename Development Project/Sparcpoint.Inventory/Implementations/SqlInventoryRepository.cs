using Dapper;
using Sparcpoint.Inventory.Abstract;
using Sparcpoint.Inventory.Models.Requests;
using Sparcpoint.SqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sparcpoint.Inventory.Implementations
{
    public class SqlInventoryRepository : IInventoryRepository
    {
        private readonly ISqlExecutor _Executor;

        public SqlInventoryRepository(ISqlExecutor executor)
        {
            _Executor = executor ?? throw new ArgumentNullException(nameof(executor));
        }

        public async Task<int> AddAsync(InventoryAdjustmentRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.IntGreaterThanZero(request.ProductInstanceId, nameof(request.ProductInstanceId));
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(request));
            PreConditions.StringMaxLength(request.TypeCategory, nameof(request.TypeCategory), 32);

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                await EnsureProductExistsAsync(conn, trans, request.ProductInstanceId);

                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                    VALUES (@ProductInstanceId, @Quantity, SYSUTCDATETIME(), @TypeCategory);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    request.ProductInstanceId,
                    request.Quantity,
                    request.TypeCategory
                }, trans);
            });
        }

        public async Task<int> RemoveAsync(InventoryAdjustmentRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            PreConditions.IntGreaterThanZero(request.ProductInstanceId, nameof(request.ProductInstanceId));
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(request));
            PreConditions.StringMaxLength(request.TypeCategory, nameof(request.TypeCategory), 32);

            return await _Executor.ExecuteAsync<int>(async (conn, trans) =>
            {
                await EnsureProductExistsAsync(conn, trans, request.ProductInstanceId);

                // EVAL: Removal is recorded as a negative quantity to maintain a complete, auditable
                // transaction ledger. Net inventory = SUM(Quantity) across all transactions.
                const string sql = @"
                    INSERT INTO [Transactions].[InventoryTransactions]
                        ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                    VALUES (@ProductInstanceId, -@Quantity, SYSUTCDATETIME(), @TypeCategory);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                return await conn.ExecuteScalarAsync<int>(sql, new
                {
                    request.ProductInstanceId,
                    request.Quantity,
                    request.TypeCategory
                }, trans);
            });
        }

        public async Task<IEnumerable<int>> AddBatchAsync(IEnumerable<InventoryAdjustmentRequest> requests)
        {
            PreConditions.ParameterNotNull(requests, nameof(requests));
            var items = requests.ToList();
            if (!items.Any())
                throw new ArgumentException("At least one request must be provided.", nameof(requests));

            foreach (var item in items)
            {
                PreConditions.IntGreaterThanZero(item.ProductInstanceId, nameof(item.ProductInstanceId));
                if (item.Quantity <= 0)
                    throw new ArgumentException($"Quantity must be greater than zero for ProductInstanceId {item.ProductInstanceId}.", nameof(requests));
                PreConditions.StringMaxLength(item.TypeCategory, nameof(item.TypeCategory), 32);
            }

            return await ExecuteBatchAsync(items, isRemoval: false);
        }

        public async Task<IEnumerable<int>> RemoveBatchAsync(IEnumerable<InventoryAdjustmentRequest> requests)
        {
            PreConditions.ParameterNotNull(requests, nameof(requests));
            var items = requests.ToList();
            if (!items.Any())
                throw new ArgumentException("At least one request must be provided.", nameof(requests));

            foreach (var item in items)
            {
                PreConditions.IntGreaterThanZero(item.ProductInstanceId, nameof(item.ProductInstanceId));
                if (item.Quantity <= 0)
                    throw new ArgumentException($"Quantity must be greater than zero for ProductInstanceId {item.ProductInstanceId}.", nameof(requests));
                PreConditions.StringMaxLength(item.TypeCategory, nameof(item.TypeCategory), 32);
            }

            return await ExecuteBatchAsync(items, isRemoval: true);
        }

        public async Task<decimal> GetCountAsync(int productInstanceId)
        {
            return await _Executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                const string sql = @"
                    SELECT ISNULL(SUM([Quantity]), 0)
                    FROM [Transactions].[InventoryTransactions]
                    WHERE [ProductInstanceId] = @ProductInstanceId";

                return await conn.ExecuteScalarAsync<decimal>(sql,
                    new { ProductInstanceId = productInstanceId }, trans);
            });
        }

        public async Task<decimal> GetCountByMetadataAsync(InventoryCountByMetadataRequest request)
        {
            PreConditions.ParameterNotNull(request, nameof(request));
            var attrs = request.Attributes == null
                ? new List<KeyValuePair<string, string>>()
                : request.Attributes.Where(a => !string.IsNullOrWhiteSpace(a.Key)).ToList();

            if (!attrs.Any())
                throw new ArgumentException("At least one attribute with a non-whitespace key must be specified.", nameof(request));

            return await _Executor.ExecuteAsync<decimal>(async (conn, trans) =>
            {
                // EVAL: Each attribute pair is a separate JOIN so all conditions must be satisfied
                // (AND semantics) — matches the same pattern used in product search
                var parameters = new DynamicParameters();
                var joins = new List<string>();

                for (int i = 0; i < attrs.Count; i++)
                {
                    string alias = $"pa{i}";
                    joins.Add($"INNER JOIN [Instances].[ProductAttributes] {alias} " +
                              $"ON t.[ProductInstanceId] = {alias}.[InstanceId] " +
                              $"AND {alias}.[Key] = @attrKey{i} AND {alias}.[Value] = @attrVal{i}");
                    parameters.Add($"attrKey{i}", attrs[i].Key);
                    parameters.Add($"attrVal{i}", attrs[i].Value);
                }

                var sql = "SELECT ISNULL(SUM(t.[Quantity]), 0) FROM [Transactions].[InventoryTransactions] t"
                    + " " + string.Join(" ", joins);

                return await conn.ExecuteScalarAsync<decimal>(sql, parameters, trans);
            });
        }

        public async Task RemoveTransactionAsync(int transactionId)
        {
            PreConditions.IntGreaterThanZero(transactionId, nameof(transactionId));

            await _Executor.ExecuteAsync(async (conn, trans) =>
            {
                const string sql = @"
                    DELETE FROM [Transactions].[InventoryTransactions]
                    WHERE [TransactionId] = @TransactionId";

                await conn.ExecuteAsync(sql, new { TransactionId = transactionId }, trans);
            });
        }

        // EVAL: All inserts share one transaction — if any row fails the entire batch rolls back
        private async Task<IEnumerable<int>> ExecuteBatchAsync(List<InventoryAdjustmentRequest> items, bool isRemoval)
        {
            string quantityExpr = isRemoval ? "-@Quantity" : "@Quantity";
            string sql = $@"
                INSERT INTO [Transactions].[InventoryTransactions]
                    ([ProductInstanceId], [Quantity], [CompletedTimestamp], [TypeCategory])
                VALUES (@ProductInstanceId, {quantityExpr}, SYSUTCDATETIME(), @TypeCategory);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _Executor.ExecuteAsync<IEnumerable<int>>(async (conn, trans) =>
            {
                // EVAL: Validate all IDs in one query before inserting any rows — keeps the
                // error message informative and avoids a partial batch insert on failure
                await EnsureProductsExistAsync(conn, trans, items.Select(x => x.ProductInstanceId));

                var ids = new List<int>();
                foreach (var item in items)
                    ids.Add(await conn.ExecuteScalarAsync<int>(sql, new
                    {
                        item.ProductInstanceId,
                        item.Quantity,
                        item.TypeCategory
                    }, trans));

                return ids;
            });
        }

        private static async Task EnsureProductExistsAsync(IDbConnection conn, IDbTransaction trans, int productInstanceId)
        {
            const string sql = "SELECT COUNT(1) FROM [Instances].[Products] WHERE [InstanceId] = @ProductInstanceId";
            var count = await conn.ExecuteScalarAsync<int>(sql, new { ProductInstanceId = productInstanceId }, trans);
            if (count == 0)
                throw new ArgumentException($"Product with ID {productInstanceId} does not exist.");
        }

        private static async Task EnsureProductsExistAsync(IDbConnection conn, IDbTransaction trans, IEnumerable<int> productInstanceIds)
        {
            var ids = productInstanceIds.Distinct().ToList();
            const string sql = "SELECT [InstanceId] FROM [Instances].[Products] WHERE [InstanceId] IN @Ids";
            var found = (await conn.QueryAsync<int>(sql, new { Ids = ids }, trans)).ToList();
            var missing = ids.Except(found).ToList();
            if (missing.Any())
                throw new ArgumentException($"The following product ID(s) do not exist: {string.Join(", ", missing)}.");
        }
    }
}
