using System;
using System.Collections.Generic;
using Facile.Models;

namespace Facile.ExportModels
{
	public class Documento
	{
		public Clienti cliente { get; set; }
		public Destinazioni destinazione { get; set; }
		public Fatture documento { get; set; }
		public List<FatRow> righe { get; set; }

		public Documento()
		{
			cliente = null;
			destinazione = null;
			documento = null;
			righe = null;
		}
	}
}
