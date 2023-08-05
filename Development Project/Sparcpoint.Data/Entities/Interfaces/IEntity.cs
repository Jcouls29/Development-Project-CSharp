using System;

namespace Sparcpoint.Data.Entities.Interfaces
{
	public interface IEntity
	{
		string Name { get; set; }
		string Description { get; set; }
		DateTime CreatedTimestamp { get; set; }
	}
}
