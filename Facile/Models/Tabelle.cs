using System;
using SQLite;

namespace Facile.Models
{
	public class Tabelle
	{
		[PrimaryKey, AutoIncrement]
		public int tab_id { get; set; }

		[Indexed(Name = "TabTipoCodice", Order = 1, Unique = true)] 
		public int tab_tipo { get; set; }

		[Indexed(Name = "TabTipoCodice", Order = 2, Unique = true)] 
		public int tab_codice { get; set; }

		[Indexed] 
		public string tab_desc { get; set; }

		public string tab_barcode { get; set; }
		public double tab_float1 { get; set; }
		public int tab_backcolor { get; set; }
		public int tab_forecolor { get; set; }
		public bool tab_bold { get; set; }
		public bool tab_italicus { get; set; }
		public bool tab_underline { get; set; }
		public bool tab_strikeout { get; set; }
		public string tab_user { get; set; }
		public DateTime tab_last_update { get; set; }
		public int tab_parent { get; set; }
		public bool tab_disabled { get; set; }
		public int tab_giorno_set { get; set; }
		public int tab_web_id { get; set; }
		public string tab_pwd { get; set; }
	}
}
