using System;
using System.Globalization;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;

namespace Facile.Converters
{

	public class IndexToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var listview = parameter as SfListView;
			var index = listview.DataSource.DisplayItems.IndexOf(value);

			if (index % 2 == 0)
				return "#E3F2FD";
			return "#BBDEFB";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
