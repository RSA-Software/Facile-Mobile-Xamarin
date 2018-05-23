using System;
namespace Facile.ExportModels
{
	public class ScadenzeSinc
	{
		public short dsr_rel_sca { get; set; }
		public int dsr_num_sca { get; set; }
		public double dsr_paginc { get; set; }

		public ScadenzeSinc()
		{
			dsr_rel_sca = 0;
			dsr_num_sca = 0;
		}
	}
}
