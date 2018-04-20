using System;
namespace Facile
{
	public class ZebraExceptions : Exception
	{
		public static readonly string MsgNoPrinterSelected = "Non è stata selezionata alcuna stampante";
		public static readonly string MsgNoZplPrinter      = "ZPL non impostato. La stampante non supporta il linguaggio ZPL";
		public static readonly string MsgUnableToPrint     = "Impossibile Stampare : Printer Status";
		public static readonly string MsgDuringPrint       = "Errore in fase di stampa : Printer Status";


		public static readonly int ErrNoPrinterSelected = 1;
		public static readonly int ErrNoZplPrinter      = 2;
		public static readonly int ErrUnableToPrint     = 3;
		public static readonly int ErrDuringPrint       = 4;


		public readonly int _error;

		public ZebraExceptions()
		{
			_error = 0;
		}

		public ZebraExceptions(string message, int err)
			:base(message)
		{
			_error = err;
		}

		public ZebraExceptions(string message, int err, Exception inner)
			: base(message, inner)
		{
			_error = err;
		}

		public int GetError()
		{
			return _error;
		}

	}
}
