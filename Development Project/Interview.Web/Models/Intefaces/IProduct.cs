namespace Interview.Web.Models.Intefaces
{
	public interface IProduct : IEntity
	{
		string ProductImageUris { get; set; }
		string ValidSkus { get; set; }
	}
}
