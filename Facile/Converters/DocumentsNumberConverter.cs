using System;
using System.Globalization;
using Facile.Utils;
using Facile.ViewModels;
using Xamarin.Forms;

namespace Facile.Converters
{
	public class DocumentsNumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "";

			var doc = (Documents)value;
			string str = String.Format("{0}/{1}", RsaUtils.GetShowedNumDoc(doc.fat_n_doc), doc.fat_registro);

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}	
	}
}
