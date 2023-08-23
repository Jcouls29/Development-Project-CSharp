namespace Sparcpoint.Inventory.Domain.Entities.Instances
{
    public class InstanceAttribute
    {
        public int InstanceId { get; set; } = int.MinValue;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
