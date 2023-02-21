using System;
using System.Collections.Generic;
using System.Text;

namespace Sparcpoint.Models
{
    public class ProductModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductImageUris { get; set; }
        public string ValidSkus { get; set; }
        public Dictionary<string, int> Categories { get; set; }
        //EVAL: MetaData String key, object value eg; "SKU, 21324" and "Color, Blue" or "Discount, 0.1"
        public Dictionary<string, object> MetaData { get; set; }
        //EVAL: Can't be deleted, easily undo with an update
        public bool IsDeleted { get; set; } = false;

    //   [InstanceId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	//   [Name] VARCHAR(256) NOT NULL,
    //   [Description] VARCHAR(256) NOT NULL,
    //   [ProductImageUris] VARCHAR(MAX) NOT NULL,
    //   [ValidSkus] VARCHAR(MAX) NOT NULL,
    }
}
