using System.ComponentModel.DataAnnotations.Schema;
using Inverview.Web.Models;

namespace Interview.Web.Models
{
    public class Product
    {
        [Column("InstanceId")]
        public long Id {get; set;}
    	public string Name {get; set;}
	    public string Description {get; set;}
        public string ProductImageUris {get; set;}
        public string ValidSkus {get; set;}
        public ProductAttributes[] Attributes {get; set;}
        public ProductCategories[] Categories {get; set;}
    }
}

