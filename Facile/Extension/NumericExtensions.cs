using System;
namespace Facile.Extension
{
	public static class NumericExtensions
	{

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

			if (buf.Substring(0, dec + 2) != "0.0000000000" && buf.Substring(0, dec + 3) != "-0.0000000000")
			{
				val = Convert.ToDouble(buf);
				return (false);
			}
			//sprintf_s(buffer, sizeof(buffer), "%.8f", *val);
			//buffer[strlen(buffer) - (8 - dec)] = '\x0';
			//if ((strncmp(buffer, "0.0000000000", dec + 2) != 0) && (strncmp(buffer, "-0.0000000000", dec + 3) != 0))
			//{
			//	*val = atof(buffer);
			//	return (FALSE);
			//}
			//*val = 0.0;




			return (true);
		}
	}
}
