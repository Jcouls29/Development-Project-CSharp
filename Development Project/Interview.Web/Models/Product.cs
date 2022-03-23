﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Interview.Web.Models
{
	[Table("Products", Schema = "Instances")]
	public class Product
	{
		[Key]
		public int InstanceId { get; set; }

		//EVAL: max length 256 chars
		[Required]
		[StringLength(256)]
		public string Name { get; set; }

		//EVAL: max length 256 chars
		[Required]
		[StringLength(256)]
		public string Description { get; set; }
		
		[Required]
		public string ProductImageUris { get; set; }

		//EVAL:regex for specific SKU format ###-###-####
		[Required]
		[RegularExpression(@"[0-9][0-9][0-9]-[0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]")]
		public string ValidSkus { get; set; }

		//EVAL:regex to constrain to DateTime
		[Required]
		[DataType(DataType.Date)]
		public DateTime CreatedTimestamp { get; set; }
	}
}
