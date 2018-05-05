using System;
using System.Globalization;
using Facile.ViewModels;
using Xamarin.Forms;

namespace Facile.Converters
{
	public class DocumentsValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Color.Black;

			var val = (double)value;

			if (val < 0)
				return Color.Red;
			else
				return (Color.Black);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}	
	}
}
