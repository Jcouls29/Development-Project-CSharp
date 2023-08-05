using System;

namespace Interview.Web.Models.Intefaces
{
	public interface IEntity
	{
		string Name { get; set; }
		string Description { get; set; }
		DateTime CreatedTimestamp { get; set; }
	}
}
