using System;
using SQLite;

namespace Facile.Models
{
	[Table("descriz1")]
	public class Descrizioni
	{
		[PrimaryKey]
		public int des_codice { get; set; }

		public string des_old_desc_0 { get; set; }
		public string des_old_desc_1 { get; set; }
		public string des_old_desc_2 { get; set; }
		public string des_old_desc_3 { get; set; }
		public string des_old_desc_4 { get; set; }
		public string des_old_desc_5 { get; set; }
		public string des_user { get; set; }
		public DateTime? des_last_update { get; set; }
		public string des_newdes { get; set; }
	}
}
