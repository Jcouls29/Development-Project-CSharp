namespace Interview.Web.Models;

public class AddInventoryRequest
{
    public int ProductInstanceId { get; set; }

    // EVAL: Decimal matches DECIMAL(19,6) in the DB — supports fractional units (weight, volume).
    public decimal Quantity { get; set; }

    // EVAL: Optional label for the transaction type e.g. "RECEIVE", "SALE", "ADJUSTMENT".
    public string TypeCategory { get; set; }
}
