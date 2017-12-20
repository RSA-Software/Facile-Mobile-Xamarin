using System;
namespace Facile.ViewModels
{
	public class Documents
	{
		public int fat_tipo { get; set; }
		public int fat_n_doc { get; set; }
		public DateTime fat_d_doc { get; set; }
		public string cli_desc { get; set; }
		public double fat_tot_fattura { get; set; }
		public int fat_credito { get; set; }
		public string fat_registro { get; set; }

		public Documents()
		{
			fat_tipo = 0;
			fat_n_doc = 0;

			cli_desc = "";
			fat_tot_fattura = 0.0;
			fat_credito = 0;
			fat_registro = "";
		}
	}
}
