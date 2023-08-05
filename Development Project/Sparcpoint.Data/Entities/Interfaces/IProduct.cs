namespace Sparcpoint.Data.Entities.Interfaces
{
	public interface IProduct : IEntity
	{
		string ProductImageUris { get; set; }
		string ValidSkus { get; set; }
	}
}
