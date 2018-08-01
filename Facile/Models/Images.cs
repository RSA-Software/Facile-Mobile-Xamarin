using System;
using SQLite;

namespace Facile.Models
{
	[Table("images")]
	public class Images
	{
		[PrimaryKey]
		public string img_name { get; set; }
		public DateTime? img_last_update { get; set; }
	}
}
