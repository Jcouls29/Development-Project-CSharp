namespace Sparcpoint.Api.Requests;

public record ProductTransactionRequest(long ProductId, decimal Quantity);

public record ProductTransactionRequests(List<ProductTransactionRequest> Products);
