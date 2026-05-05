namespace Interview.Web.Models;

public class ProductAttributeModel
{
    // EVAL: I would have, personally named this ProductInstanceId
    public int InstanceId { get; set; }
    public string Key  { get; set; }
    public string Value { get; set; }
}