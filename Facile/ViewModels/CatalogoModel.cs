using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Facile.ViewModels
{
	public class CatalogoModel
	{
		private string _image;

		public CatalogoModel(string str)
		{
			Image = str;
		}

		public string Image
		{
			get { return _image; }
			set { _image = value; }
		}
	}

	public class CarouselModel
	{
		public CarouselModel(string imageString)
		{
			Image = imageString;
		}
		private string _image;
		public string Image
		{
			get { return _image; }
			set { _image = value; }
		}
	}

	public class CarouselViewModel
	{
		public CarouselViewModel()
		{
			ImageCollection.Add(new CarouselModel("header_wallpaper.jpg"));
		}
		private ObservableCollection<CarouselModel> imageCollection = new ObservableCollection<CarouselModel>();
		public ObservableCollection<CarouselModel> ImageCollection
		{
			get { return imageCollection; }
			set { imageCollection = value; }
		}
	}
}
