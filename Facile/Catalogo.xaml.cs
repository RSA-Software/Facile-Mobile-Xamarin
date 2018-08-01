using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.ViewModels;
using PCLStorage;
using Syncfusion.SfCarousel.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Catalogo : ContentPage
	{
		private readonly int _load_block;
		private bool first;
		private int last_index;
		private int first_load;
		private int last_load;
		private IList<IFile> _files;
		public CarouselViewModel carouselList;

		public Catalogo()
		{
			first = true;
			last_index = 0;
			first_load = -1;
			last_load = -1;
			_files = null;
			_load_block = 10;

			InitializeComponent();
			carouselList = new CarouselViewModel();

			/*
			var catalogoModelDataTemplate = new DataTemplate(() =>
			{
				var grid = new Grid();
				var nameLabel = new Image();
				nameLabel.SetBinding(Image.SourceProperty, "Image");
				grid.Children.Add(nameLabel);
				return grid;
			});

			//m_carousel.BindingContext = carouselList;
			//m_carousel.ItemTemplate = catalogoModelDataTemplate;
			//m_carousel.DataSource = catalogoModels;
			//m_carousel.BindingContext = catalogoModels;
			*/
		}

		protected async override void OnAppearing()
		{
			if (first)
			{
				IFolder rootFolder = FileSystem.Current.LocalStorage;
				IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

				first = false;
				_files = await imagesFolder.GetFilesAsync();

				//
				// Rimuoviamo tutti i file con estensione non valida
				//
				var idx = 0;
				for (idx = 0; idx < _files.Count; idx++)
				{
					bool remove = true;
					var x = _files[idx].Name.LastIndexOf('.');
					if (x >= 0)
					{
						var ext = _files[idx].Name.Substring(x);
						if (ext.ToUpper() == ".JPG" || ext.ToUpper() == ".PNG") remove = false;
					}
					if (remove)
					{
						_files.RemoveAt(idx);
						idx--;
					}
				}
				carouselList.ImageCollection.Clear();
				first_load = 0;
				for (idx = 0; idx < (_files.Count < 20 ? _files.Count : 20); idx++)
				{
					var item = new CarouselModel(_files[idx].Path);
					carouselList.ImageCollection.Add(item);
					last_load = idx;
				}
				m_carousel.BindingContext = carouselList;
			}
			base.OnAppearing();
		}

		protected override void OnSizeAllocated(double width, double height)
		{

			m_carousel.ItemWidth = (int)(Application.Current.MainPage.Width - 300);
			m_carousel.ItemHeight = (int)(Application.Current.MainPage.Height - 300);

			base.OnSizeAllocated(width, height);
		}

		async void OnSelectionChanged(object sender, Syncfusion.SfCarousel.XForms.SelectionChangedEventArgs e)
		{
			try
			{
				busyIndicator.IsBusy = true;
				if (e.SelectedIndex > last_index)
				{
					if ((e.SelectedIndex + first_load) == (last_load - 4) && last_load < _files.Count)
					{
						var index = e.SelectedIndex;

						var start = last_load + 1;
						m_carousel.BindingContext = null;
						for (int idx = start; idx < (_files.Count > (start + _load_block) ? (start + _load_block) : _files.Count); idx++)
						{

							if (idx > last_load) last_load = idx;

							var item = new CarouselModel(_files[idx].Path);
							carouselList.ImageCollection.Add(item);
							carouselList.ImageCollection.RemoveAt(0);
							first_load++;
							index--;
						}
						m_carousel.BindingContext = carouselList;
						last_index = index;
						if (index != e.SelectedIndex) m_carousel.SelectedIndex = index;
					}
				}
				else if (e.SelectedIndex < last_index)
				{
					if ((e.SelectedIndex + first_load) == (first_load + 4))
					{
						var index = e.SelectedIndex;

						var start = first_load - 1;
						var xxx = (start - _load_block) > -1 ? start - _load_block : -1;

						m_carousel.BindingContext = null;
						for (int idx = start; idx > xxx; idx--)
						{

							if (idx < first_load) first_load = idx;

							var item = new CarouselModel(_files[idx].Path);
							carouselList.ImageCollection.Insert(0, item);

							carouselList.ImageCollection.RemoveAt(carouselList.ImageCollection.Count - 1);
							last_load--;
							index++;
						}
						m_carousel.BindingContext = carouselList;
						last_index = index;
						if (index != e.SelectedIndex) m_carousel.SelectedIndex = index;
					}
				}
				else
				{
					last_index = e.SelectedIndex;
				}
			}
			catch (OutOfMemoryException)
			{
				await DisplayAlert("Errore!", "Memoria non sufficiente per eseguire l'operazione", "OK");
			}
			busyIndicator.IsBusy = false;
			m_desc.Text = $"Immagini caricate : Memory {first_load}-{last_load}      {e.SelectedIndex}+{first_load} == {e.SelectedIndex + first_load} == {last_load - 4}/{_files.Count} - Corrente : {e.SelectedIndex}";
		}
	}
}
