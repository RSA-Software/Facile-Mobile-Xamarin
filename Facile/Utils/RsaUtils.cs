using System;
using System.Text;

namespace Facile.Utils
{
	public class RsaUtils
	{
		private static readonly int _gap_registro = 70000000;
		private static readonly int _gap_interno = 2500000;
		public static readonly int max_reg_data = 702;

		public static bool IsRegistroValid(string reg)
		{
			if (reg.Length == 1)
			{
				if ((reg[0] >= 'A') && (reg[0] <= 'Z')) return (true);
			}
			else if (reg.Length == 2)
			{
				if (((reg[0] >= 'A') && (reg[0] <= 'Z')) && ((reg[1] >= 'A') && (reg[1] <= 'Z'))) return (true);
			}
			return (false);
		}

		public static int GetShowedNumDoc(int num)
		{
			var val = num % _gap_registro;
			if (val >= _gap_interno) val = val % _gap_interno;
			return (val);
		}

		public static int GetStoredNumDoc(int num, string reg)
		{
			var numero = 0;
			if (!IsRegistroValid(reg)) throw new ArgumentException("Registro non valido");
			if (reg.Length > 0)
			{
				numero = num + (reg[0] - 'A') * _gap_registro;
			}
			if (reg.Length > 1)
			{
				numero += _gap_interno + (reg[1] - 'A') * _gap_interno;
			}
			return (numero);
		}

		public static int GetStoredNumDoc(int num, int reg)
		{
			if (reg < 0 || reg >= max_reg_data) throw new ArgumentException("Registro non valido");
			var numero = 0;
			if (reg < 26)
			{
				numero = num + reg * _gap_registro;
			}
			else
			{
				var mul1 = (reg / 26) - 1;
				var mul2 = (reg % 26);
				numero = num + mul1 * _gap_registro;
				numero += _gap_interno + mul2 * _gap_interno;
			}
			return (numero);
		}

		public static string GetRegistroFromStoredNumDoc(int num)
		{
			var reg = new StringBuilder();
			reg.Append((char)('A' + (num / _gap_registro)));
			num = num % _gap_registro;
			if (num >= _gap_interno)
			{
				reg.Append((char)('A' + (num / _gap_interno) - 1));
			}
			var str = reg.ToString();
			return (str);
		}

		public static string GetRegistroFromOrdinal(int ordinal)
		{
			if (ordinal < 0 || ordinal >= max_reg_data) throw new ArgumentException("Registro non valido");
			var reg = new StringBuilder();
			var num = GetFirstRegNumber(ordinal);
			reg.Append((char)('A' + (num / _gap_registro)));
			num = num % _gap_registro;
			if (num >= _gap_interno)
			{
				reg.Append((char)('A' + (num / _gap_interno) - 1));
			}
			var str = reg.ToString();
			return (str);
		}

		public static int GetOrdinalFromNumDoc(int num)
		{
			var reg = num / _gap_registro;
			num = num % _gap_registro;
			if (num >= _gap_interno)
			{
				reg = (reg * 26) + 26 + (num / _gap_interno) - 1;
			}
			return (reg);
		}

		public static int GetOrdinalFromRegistro(string reg)
		{
			var num = 0;
			if (!IsRegistroValid(reg)) throw new ArgumentException("Registro non valido");
			if (reg.Length < 1) return (num);
			if (reg.Length == 1)
				num = reg[0] - 'A';
			else
				num = 26 * (reg[0] - 'A') + 26 + reg[1] - 'A';
			return (num);
		}

		public static int GetFirstRegNumber(string reg)
		{
			var num = 0;

			if (!IsRegistroValid(reg)) throw new ArgumentException("Registro non valido");
			if (reg.Length == 1)
			{
				num = (reg[0] - 'A') * _gap_registro;
			}
			else
			{
				num = (reg[0] - 'A') * _gap_registro + _gap_interno + (reg[1] - 'A') * _gap_interno;
			}

			return num;
		}

		public static int GetFirstRegNumber(int reg_ordinal)
		{
			if (reg_ordinal < 0 || reg_ordinal >= max_reg_data) throw new ArgumentException("Registro non valido");
			var num = 0;
			if (reg_ordinal < 26)
			{
				num = reg_ordinal * _gap_registro;
			}
			else
			{
				var mul1 = reg_ordinal / 26 - 1;
				var mul2 = reg_ordinal % 26;
				num = mul1 * _gap_registro + _gap_interno + mul2 * _gap_interno;
			}
			return (num);
		}

		public static int GetLastRegNumber(string reg)
		{
			if (!IsRegistroValid(reg)) throw new ArgumentException("Registro non valido");
			return (GetFirstRegNumber(reg) + _gap_interno - 1);
		}

		public static int GetLastRegNumber(int reg_ordinal)
		{
			if (reg_ordinal < 0 || reg_ordinal > max_reg_data) throw new ArgumentException("Registro non valido");
			return (GetFirstRegNumber(reg_ordinal) + _gap_interno - 1);
		}

	}
}
