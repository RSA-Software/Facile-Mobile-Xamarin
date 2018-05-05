using System;
using SQLite;

namespace Facile.Models
{
	[Table("associaz")]
	public class Associazioni
	{
		[PrimaryKey, AutoIncrement]
		public int asg_id { get; set; }

		[Indexed(Name = "AsgCliDstGruAge", Order = 1, Unique = true)] 
		public int asg_cli { get; set; }

		[Indexed(Name = "AsgCliDstGruAge", Order = 3, Unique = true)] 
		public int asg_gru { get; set; }

		[Indexed(Name = "AsgCliDstGruAge", Order = 1, Unique = true)] 
		public int asg_age { get; set; }

		public int asg_vet { get; set; }
		public int asg_tab_gru { get; set; }
		public int asg_pag { get; set; }
		public int asg_gir { get; set; }
		public int asg_tab_gir { get; set; }
		public int asg_sequenza { get; set; }

		[Indexed(Name = "AsgCliDstGruAge", Order = 2, Unique = true)] 
		public int asg_dst { get; set; }

		public int asg_gir1 { get; set; }
		public int asg_tab_gir1 { get; set; }
		public int asg_sequenza1 { get; set; }
		public int asg_gir2 { get; set; }
		public int asg_tab_gir2 { get; set; }
		public int asg_sequenza2 { get; set; }
		public string asg_user { get; set; }
		public DateTime asg_last_update { get; set; }
	}
}
