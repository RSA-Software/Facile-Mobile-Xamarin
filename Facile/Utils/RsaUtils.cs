using System;
namespace Facile.Utils
{
	public class RsaUtils
	{
		private static readonly int _gap_registro = 70000000;

		public static int GetShowedNumDoc(int num)
		{
			return (num % _gap_registro);
		}

		public static int GetStoredNumDoc(int num, string reg)
		{
			int numero = num + (reg[0] - 'A') * _gap_registro;
			return (numero);
		}

		public static int GetStoredNumDoc(int num, int reg)
		{
			int numero = num + reg  * _gap_registro;
			return (numero);
		}

		public static string GetRegistro(int num)
		{
			char[] reg = { (char)('A' + (num / _gap_registro)) };
			string str = new string(reg);
			return (str);
		}

		public static int GetFirstRegNumber(string reg)
		{
			int num;

			if (string.IsNullOrWhiteSpace(reg) || reg.Length < 1 || reg.Length > 2)
				throw new ArgumentException("registro non valido");

			if (reg.Length == 1)
			{
				num = (reg[0] - 'A') * _gap_registro;
			}
			else
			{
				num = (reg[0] - 'A') * _gap_registro + (27 + (reg[1] - 'A')) * _gap_registro;
			}

			return (num);
		}

		public static int GetLastRegNumber(string reg)
		{
			int num;

			if (string.IsNullOrWhiteSpace(reg) || reg.Length < 1 || reg.Length > 2)
				throw new ArgumentException("registro non valido");

			if (reg.Length == 1)
			{
				num = (reg[0] - 'A' + 1) * _gap_registro;
			}
			else
			{
				num = (reg[0] - 'A') * _gap_registro + (27 + (reg[1] - 'A' + 1)) * _gap_registro;
			}

			return (num - 1);
		}


	}
}
