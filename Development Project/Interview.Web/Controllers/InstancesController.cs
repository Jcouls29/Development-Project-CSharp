using Dapper;
using Microsoft.AspNetCore.Mvc;
using Sparcpoint.SqlServer.Abstractions;
using Sparcpoint.SqlServer.Abstractions.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Interview.Web.Controllers
{
    /// <summary>
    /// Handles CRUD operations for "instance" database objects (categories, products).
    /// I'm calling this the instance controller instead of a product controller because it cares about categories as well as products. Non-transactions.
    /// </summary>
    [ApiController]
    [Route("api/v1/instances")]
    public class InstancesController : Controller
    {
        // Goal: 2-hour window
        // Realistic: 10-hour window
        // Compromise: Yes

        // Time Breakdown: (assuming unit testing is a part of the coding process)
        // [expected:actual]
        // Each item represents 15 minutes unless stated otherwise.

        // DONE
        // [5] Publishing to SQL
        // [25] Read instructions + planning
        // [5] Decide on what fake data would work well for this project
        // [25] Read through codebase to ensure we know about what we can use. 
        // [] Adding products into Inventory
        // [] Removing products from Inventory - one transaction
        // [] Clean: TODOs
        // [30] Retrieving a Count of Product Inventory based on a unique product identifier 

        // WIP
        // [] Adding a Product with all necessary details: Categories, Metadata, and General Details
        // [5]   Products don't have Categories, Metadata directly. I'll have to piece together how they should work together.

        // Priority
        // [10] Searching for Products by Category, Metadata, General Details
        // [20] or any combination thereof
        // [] Clean: Go through code at the end to ensure it is well-commented (I swear I comment a lot IRL)
        // [] Clean: Make interfaces (I may be able to make some for my models), use them for testing
        // [5] Clean: Ensure proper DI is used (I think it is already)
        // [5] Ensure system is configurable (I think it is already)
        // [15] Transfer code to github acct, submit a GitHub Pull Request into the master branch (https://github.com/Jcouls29/Development-Project-CSharp)

        // Lower Priority
        // [] Undo transactions (remove the latest inventory transaction)
        // [] The ability to categorize and create hierarchies of products for simple sorting on various UI frontends. (Isn't this natural for this kind of data?)
        // [] (Low Priority) Add products to Inventory - multi-transaction: "should happen on an individual product level or multiple products at once."
        // [] (Low Priority) Removing products from Inventory - multi-transaction: "should happen on an individual product level or multiple products at once."
        // [] Add the API route IDs, POST vs GET
        // [] Retrieving a Count of Product Inventory based on a unique product identifier based on metadata on the products
        // [] async
        // [] Nomenclature: add/remove (adjust quantity) vs create/delete

        // Notes on requirements

        // "The basic set of functions should be accessible from the API:"
        //1. Adding a Product with all necessary details: Categories, Metadata, and General Details
        //2. Searching for Products by Category, Metadata, General Details, or any combination thereof
        //3. Adding products into Inventory
        //4. Removing products from Inventory
        //5. Retrieving a Count of Product Inventory based on a unique product identifier or metadata on the products
        // Does this mean "These features should already be in the codebase you were given."
        // Or "You should make these features."? (I do not see these features, so I am going to assume these are requirements.)

        // There's the major goal of an SKU system, but is that really something to worry about or do the existing SQL tables allow for its support enough to focus elsewhere?

        // EVAL: Requirement: Products must allow arbitrary amounts of metadata and categories.
        // Don't they already? Isn't that what the DB tables allow? I saw 2 (Key, Value) tables, one for Categories, one for Products.
        // I am unsure what this means, so I will have to research the document for hints.
        // Hint 1: Consider how you might extend the System or alter the API(s) for new customer requirements, 
        // that may have not been foreseen, without impacting older customers.
        // Hint 2: Metadata on products and categories can be pre-determined, and do not need to be “dynamically” added to the System. 
        // Instead, come up with a handful of extra attributes for products and/or categories to be consistent (i.e.SKU(s), Color, Length, Package Unit, etc).
        // I think this means I can choose for this system a few metadata fields and assume no more will be added to break tests.

        private readonly ISqlExecutor _sqlExecutor;
        public InstancesController(ISqlExecutor sqlExecutor)
        {
            _sqlExecutor = sqlExecutor ?? throw new ArgumentNullException(nameof(sqlExecutor));
        }

        [HttpGet("/products/create")]
        public IActionResult CreateProduct(Product product)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) 
                            VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)",
                            product,
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                    return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/products/create/test")]
        public IActionResult CreateTestProduct()
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO Instances.Products (Name, Description, ProductImageUris, ValidSkus, CreatedTimestamp) VALUES (@Name, @Description, @ProductImageUris, @ValidSkus, @CreatedTimestamp)";

                            // Assuming you have variables to hold the values you want to insert
                            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = "Test Name2" });
                            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar) { Value = "Test Desc" });
                            command.Parameters.Add(new SqlParameter("@ProductImageUris", SqlDbType.VarChar) { Value = "Test Test" });
                            command.Parameters.Add(new SqlParameter("@ValidSkus", SqlDbType.VarChar) { Value = "Test SKU" });
                            command.Parameters.Add(new SqlParameter("@CreatedTimestamp", SqlDbType.DateTime) { Value = DateTime.UtcNow });

                            return command.ExecuteNonQuery(); // Return the number of rows affected
                        }
                    }
                );

                if (numberOfRowsAffected > 0)
                    return Ok();
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/products/all")]
        public IActionResult GetAllProducts()
        {
            try
            {
                var result = _sqlExecutor.Execute<IEnumerable<Product>>(
                    (connection, transaction) =>
                    {
                        return connection.Query<Product>(
                            sql: @"SELECT * FROM Instances.Products",
                            transaction: transaction
                        );
                    }
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/products/delete")]
        public IActionResult DeleteProduct(Product product)
        {
            // EVAL: Requirement 1: Products can be added to the System but never deleted
            return StatusCode(500, $"Products should not be able to be deleted.");
        }

        /// <summary>
        /// I do not think this should work unless there is uniqueness (PKs for names).
        /// But maybe there's just one detail I'm missing, and the DB is fine.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("/products/instanceId/{name}")]
        public IActionResult GetProductInstanceIdFromName(string name)
        {
            try
            {
                var result = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.QueryFirstOrDefault<int>(
                            sql: @"SELECT top 1 InstanceId FROM [master].[Instances].[Products] 
                                WHERE name=@name",
                            new { Name = name },
                            transaction: transaction
                        );
                    }
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        [HttpGet("/categories/create")]
        public IActionResult CreateCategory(Category category)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.Categories (Name, Description, CreatedTimestamp) 
                            VALUES (@Name, @Description, @CreatedTimestamp)",
                            category,
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // Fetch the InstanceId of the newly inserted category
                    int instanceId = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            return connection.QueryFirstOrDefault<int>(
                                "SELECT SCOPE_IDENTITY()",
                                transaction: transaction
                            );
                        }
                    );

                    return Ok(instanceId);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates an association between a Product Id and a Category Id.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("/product/categories")]
        public IActionResult CreateProductCategory(ProductCategory productCategory)
        {
            try
            {
                int numberOfRowsAffected = _sqlExecutor.Execute<int>(
                    (connection, transaction) =>
                    {
                        return connection.Execute(
                            @"INSERT INTO Instances.ProductCategories (InstanceId, CategoryInstanceId) 
                            VALUES (@InstanceId, @CategoryInstanceId)",
                            productCategory,
                            transaction
                        );
                    }
                );

                if (numberOfRowsAffected > 0)
                {
                    // Fetch the InstanceId of the newly inserted category
                    int instanceId = _sqlExecutor.Execute<int>(
                        (connection, transaction) =>
                        {
                            return connection.QueryFirstOrDefault<int>(
                                "SELECT SCOPE_IDENTITY()",
                                transaction: transaction
                            );
                        }
                    );

                    // Return the InstanceId in the response
                    return Ok(instanceId);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the product to inventory: {ex.Message}");
            }
        }
    }
}
