using Xunit;
using static Xunit.Assert;
namespace Sparcpoint.Tests.Controllers
{
	public class ProductControllerTests
	{
		// do we want to return a message or an error or just an empty dataset?
		[Fact(DisplayName = "When No products are in the table. Then return an empty list.")]
		public void WhenNoItemsAreInTheTable_ThenReturnEmptyList()
		{
			//Arrange

			//Act

			//Assert
			Equal("", "I'm a test, and I'm not configured yet!!");
		}

		[Fact(DisplayName = "When products are are in the table then return a list of products. ")]
		public void WhenProductsAreInTheTable_ThenReturnAListOfProducts()
		{
			//Arrange

			//Act

			//Assert
			Equal("", "I'm a test, and I'm not configured yet!!");
		}

		[Fact(DisplayName = "When a product has a category. Return uri for getting the category data.")]
		public void WhenProductHasCategory_ThenProductCategoryWillShowAUriForTheCategory()
		{
			//Arrange

			//Act

			//Assert
			Equal("", "I'm a test, and I'm not configured yet!!");
		}
	}
}
