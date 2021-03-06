﻿using System;
using System.Globalization;
using Facile.Models;
using Facile.ViewModels;
using Xamarin.Forms;

namespace Facile.Converters
{
	public class DocumentsTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return "";

			var doc = (Documents)value;
			switch(doc.fat_tipo)
			{
				case (int)DocTipo.TIPO_DDT: return("DDT"); 
				case (int)DocTipo.TIPO_BOL: return("BOL"); 
				case (int)DocTipo.TIPO_ORD: return("ORD"); 
				case (int)DocTipo.TIPO_FAT:
					if (doc.fat_credito != 0)
						return ("CRE");
					else
						return("FAT"); 
				case (int)DocTipo.TIPO_PRE: return("PRE"); 
				default : return("***");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}	
	}
}
