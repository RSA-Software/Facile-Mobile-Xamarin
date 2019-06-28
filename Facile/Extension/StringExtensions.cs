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
		
		public static string FirstCharToUpper(this string str)
		{
			switch (str)
			{
				case null: throw new ArgumentNullException(nameof(str));
				case "": throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str));
				default: return str.Substring(0, 1).ToUpper() + str.Substring(1); 
			}
		}
	}
}
