using System;
namespace Facile.ViewModels
{
	public class ScadenzeInfo
	{
		private int cliId;
		private string cliDesc;
		private string cliTel;
		private double cliTotale;

		public int CliId
		{
			get { return cliId; }
			set { this.cliId = value; }
		}

		public string CliDesc
		{
			get { return cliDesc; }
			set { this.cliDesc = value; }
		}

		public string CliTel
		{
			get { return cliTel; }
			set { this.cliTel = value; }
		}

		public double CliTotale
		{
			get { return this.cliTotale; }
			set { this.cliTotale = value; }
		}

		public ScadenzeInfo()
		{
			cliId = 0;
			cliDesc = String.Empty;
			cliTel = String.Empty;
			CliTotale = 0.0;
		}
	}
}
