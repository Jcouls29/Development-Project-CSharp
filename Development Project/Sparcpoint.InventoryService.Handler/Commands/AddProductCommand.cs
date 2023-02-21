using MediatR;
using Sparcpoint.Inventory.Core.Entities.Shared;
using Sparcpoint.Inventory.Core.Requests;
using Sparcpoint.Inventory.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Inventory.Handler.Commands
{
    public class AddProductCommand : Audit, IRequest<AddProductResponse>
    {
        public int InstanceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public List<AddCategoryCommand> Categories {get; set; }
        public List<AddProductAttributesRequest> ProductAttributes {get; set; }
    }
}
