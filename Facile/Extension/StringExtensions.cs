using System;

namespace Facile.Extension
{
	public static class StringExtensions
	{
		public static string SqlQuote(this string str, bool jolly)
		{
			str = str.Replace("\\", "\\\\");
			str = str.Replace("*", "%");
			str = str.Replace("?", "_");

			string dest = "'";
			if (!string.IsNullOrEmpty(str))
			{
				if (jolly && !str.StartsWith("%", StringComparison.CurrentCulture) && !str.StartsWith("_", StringComparison.CurrentCulture)) dest += "%";
				dest += str;
			}
			if (jolly && !dest.EndsWith("%", StringComparison.CurrentCulture) && !dest.EndsWith("_", StringComparison.CurrentCulture)) dest += "%";
			dest += "'";

			return (dest);
		}

		public static bool AllDigits (this string str)
		{
			foreach (char c in str)
			{
				if (c < '0' || c > '9')
					return false;
			}
			return true;
		}
		
		public static string ProperCase(this string str)
		{
			if (string.IsNullOrWhiteSpace(str)) return (str);

			string ret = "";
			var first = true;
			foreach (char c in str)
			{
				if (first)
				{
					first = false;
					ret += Char.ToUpper(c);
				}
				else
					ret += Char.ToLower(c);
				if (c == ' ') first = true;
			}
			return (ret);
		}
	}
}
