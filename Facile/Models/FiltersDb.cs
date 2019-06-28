using System;
using SQLite;

namespace Facile.Models
{
	enum FiltersType : short
	{
		REPARTO = 0,
		MARCHIO = 1,
		CATMERC = 2,
		STAGIONE = 3,
		FORNITORE = 4,
	}


	public class FiltersDb
	{
		[PrimaryKey, AutoIncrement]
		public int fil_id { get; set; }

		[Indexed(Name = "TipoCodice", Order = 1, Unique = true)]
		public short fil_tipo { get; set; }

		[Indexed(Name = "TipoCodice", Order = 2, Unique = true)]
		public int fil_codice { get; set; }

		[Indexed]
		public string fil_desc { get; set; }

		public FiltersDb()
		{
			fil_id = 0;
			fil_tipo = 0;
			fil_codice = 0;
			fil_desc = "";
		}
	}
}
