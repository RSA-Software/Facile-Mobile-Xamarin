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

		public static string GetRegistro(int num)
		{
			char[] reg = { (char)('A' + (num / _gap_registro)) };
			string str = new string(reg);
			return (str);
		}

		internal static object GetShowedNumDoc()
		{
			throw new NotImplementedException();
		}
	}
}
