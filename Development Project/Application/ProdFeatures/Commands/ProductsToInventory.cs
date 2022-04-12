using Domain.Dtos;
using Domain.Entity;
using Domain.Entitys;
using MediatR;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ProdFeatures.Commands
{
    public class AddProductsToInventoryCommand : IRequest<IReadOnlyList<InventoryDto>>
    {
        public Dictionary<int, int> Product_Quantity { get; set; }

        public class AddProductsToInventoryCommandHandler : IRequestHandler<AddProductsToInventoryCommand, IReadOnlyList<InventoryDto>>
        {
            private readonly IProductRepository _productRepository;
            private readonly IInventoryRepository _inventoryRepository;

            public AddProductsToInventoryCommandHandler(IProductRepository productRepository, IInventoryRepository inventoryRepository)
            {
                this._productRepository = productRepository;
                this._inventoryRepository = inventoryRepository;
            }
            public async Task<IReadOnlyList<InventoryDto>> Handle(AddProductsToInventoryCommand request, CancellationToken cancellationToken)
            {

                var listOfProductsToAdd = new List<Inventory>();
                var listofAddedProductsDtoToInventory = new List<InventoryDto>();
                foreach (KeyValuePair<int, int> kvp in request.Product_Quantity)
                {
                    var product = _productRepository.GetProductById(kvp.Key);
                    listOfProductsToAdd.Add(new Inventory(product, kvp.Value, null));
                }
                var listofAddedProductsToInventory = _inventoryRepository.AddProdcutsToInventory(listOfProductsToAdd);
                foreach (Inventory inv in listofAddedProductsToInventory)
                {
                    var inventoryDto = new InventoryDto
                    {
                        CompletedOn = inv.CompletedDateTime,
                        Quantity = inv.Quantity,
                        Product = new ProductDto()
                        {
                            ProductName = inv.Product.ProductName,
                            ProductDescription = inv.Product.Description,
                            StockUnitCode = inv.Product.StockUnitCode,
                            ProductDescription = inv.Product.ProductDescription
                        }
                    };
                    listofAddedProductsDtoToInventory.Add(inventoryDto);
                }
                return await Task.FromResult(listofAddedProductsDtoToInventory);
            }
        }
    }
}