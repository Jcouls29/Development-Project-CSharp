namespace Interview.Web.Models;

public class AddInventoryRequest
{
    public int ProductInstanceId { get; set; }

    // EVAL: Quantity is decimal to match DECIMAL(19,6) in the DB, supporting fractional
    // units such as products sold by weight or volume rather than whole units only.
    public decimal Quantity { get; set; }

    // EVAL: TypeCategory is optional and allows callers to label the transaction type
    // (e.g. "RECEIVE", "ADJUSTMENT") for reporting without changing the schema.
    public string TypeCategory { get; set; }
}
