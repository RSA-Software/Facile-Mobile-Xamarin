using System;
using SQLite;

namespace Facile.Models
{
	[Table("vettori1")]
	public class Vettori
	{
		[PrimaryKey]
		public int vet_codice { get; set; }

		public string vet_citta { get; set; }
		public string vet_pro { get; set; }
		public string vet_cap { get; set; }
		public string vet_tel { get; set; }
		public string vet_fax { get; set; }
		public string vet_targa { get; set; }
		public string vet_albo { get; set; }
		public DateTime vet_last_update { get; set; }
		public string vet_user { get; set; }

		[Indexed]
		public string vet_desc { get; set; }
		public string vet_indirizzo { get; set; }
		public string vet_rag_soc1 { get; set; }
		public string vet_rag_soc2 { get; set; }
		public string vet_pers { get; set; }
		public string vet_codfis { get; set; }
		public string vet_naz { get; set; }
		public string vet_piva { get; set; }
	}
}
