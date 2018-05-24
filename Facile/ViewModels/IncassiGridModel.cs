using System;
namespace Facile.ViewModels
{
	public class IncassiGridModel
	{
		public int dsp_codice { get; set; }
		public int dsp_clifor { get; set; }
		public DateTime? dsp_data { get; set; }
		public double dsp_totale { get; set; }
		public string cli_desc { get; set; }

		public IncassiGridModel()
		{
			dsp_codice = 0;
			dsp_clifor = 0;
			dsp_data = null;
			dsp_totale = 0.0;
			cli_desc = "";
		}
	}
}
