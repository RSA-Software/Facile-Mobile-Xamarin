using System;
namespace Facile.Extension
{
	public static class NumericExtensions
	{
		public const double EPSILON = 0.0000000000000000001;

		public static double MyFloor(this double num, short dec)
		{
			for (int idx = 1; idx <= dec; idx++)
			{
				num = num * 10;
			}
			num = Math.Floor(num);
			for (int idx = 1; idx <= dec; idx++)
			{
				num = num / 10;
			}
			return (num);
		}

		public static double MyCeil(this double num, short dec)
		{
			for (int idx = 1; idx <= dec; idx++)
			{
				num = num * 10;
			}
			num = Math.Ceiling(num);
			for (int idx = 1; idx <= dec; idx++)
			{
				num = num / 10;
			}
			return (num);
		}

		public static bool TestIfZero(this double val, int dec)
		{
			double absval;

			if (dec > 8) dec = 8;
			absval = Math.Abs(val);
			if (absval > 1) return(false);
			switch (dec)
			{
				case 0:
					if (absval < 0.9999999999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 1:
					if (absval < 0.0999999999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 2:
					if (absval < 0.0099999999999999999)
					{
						val = 0.0;
						return(true);
					}
					break;

				case 3:
					if (absval < 0.0009999999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 4:
					if (absval < 0.0000999999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 5:
					if (absval < 0.0000099999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 6:
					if (absval < 0.0000009999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;


				case 7:
					if (absval < 0.0000000999999999999)
					{
						val = 0.0;
						return (true);
					}
					break;

				case 8:
					if (absval < 0.0000000099999999999)
					{
						val = 0.0;
						return (true);
					}
					break;
			}

			string str = String.Format("{0:0.00000000}", val);
			string buf = str.Substring(0, str.Length - (8 - dec));

			var str1 = "0.0000000000";
			var str2 = "-0.0000000000";
			if (buf != str1.Substring(0, dec + 2) && buf != str2.Substring(0, dec + 3))
			{
				val = Convert.ToDouble(buf);
				return (false);
			}
			return (true);
		}


		public static bool TestIfZero(this float val, int dec)
		{
			double absval;

			if (dec > 8) dec = 8;
			absval = Math.Abs(val);
			if (absval > 1) return (false);
			switch (dec)
			{
				case 0:
					if (absval < 0.9999999999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 1:
					if (absval < 0.0999999999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 2:
					if (absval < 0.0099999999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 3:
					if (absval < 0.0009999999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 4:
					if (absval < 0.0000999999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 5:
					if (absval < 0.0000099999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 6:
					if (absval < 0.0000009999999999999)
					{
						val = 0;
						return (true);
					}
					break;


				case 7:
					if (absval < 0.0000000999999999999)
					{
						val = 0;
						return (true);
					}
					break;

				case 8:
					if (absval < 0.0000000099999999999)
					{
						val = 0;
						return (true);
					}
					break;
			}

			string str = String.Format("{0:0.00000000}", val);
			string buf = str.Substring(0, str.Length - (8 - dec));

			var str1 = "0.0000000000";
			var str2 = "-0.0000000000";
			if (buf != str1.Substring(0, dec + 2) && buf != str2.Substring(0, dec + 3))
			{
				val = Convert.ToSingle(buf);
				return (false);
			}
			return (true);
		}


	}
}

