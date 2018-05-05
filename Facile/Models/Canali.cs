using System;
using SQLite;

namespace Facile.Models
{
	public class Canali
	{
		[PrimaryKey]
		public int can_codice { get; set; }

		[Indexed]
		public string can_desc { get; set; }

		public string can_sottocanale_0 { get; set; }
		public string can_sottocanale_1 { get; set; }
		public string can_sottocanale_2 { get; set; }
		public string can_sottocanale_3 { get; set; }
		public string can_sottocanale_4 { get; set; }
		public string can_sottocanale_5 { get; set; }
		public double can_non_usato { get; set; }
		public int can_cessione { get; set; }
		public int can_canale_spec { get; set; }
		public int can_ribalt_sco { get; set; }
		public int can_no_agg_off { get; set; }
		public int can_totale { get; set; }
		public string can_user { get; set; }
		public DateTime can_last_update { get; set; }
	}
}
