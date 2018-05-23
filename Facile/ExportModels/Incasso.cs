using System;
using System.Collections.Generic;
using Facile.Models;

namespace Facile.ExportModels
{
	public class Incasso
	{
		public Clienti cliente { get; set; }
		public Destinazioni destinazione { get; set; }
		public ScaPagHead head { get; set; }
		public List<ScaPagRow> rows { get; set; }

		public Incasso()
		{
			cliente = null;
			destinazione = null;
			head = null;
			rows = null;
		}
	}
}
