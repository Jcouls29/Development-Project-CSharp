﻿using System;

namespace Interview.Web.Models
{
	public class Products
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ProductImageUris { get; set; }
		public string ValidSkus { get; set; }
		public DateTime CreatedTimestamp { get; set; }

		}
	}