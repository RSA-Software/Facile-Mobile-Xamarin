using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using PCLStorage;
using SQLite;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ArticoliSearch : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;

		public ArticoliSearch()
		{
			InitializeComponent();

			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			query_ = "SELECT * FROM artanag ORDER BY ana_desc1";

			listView.LoadMoreOption = Syncfusion.ListView.XForms.LoadMoreOption.Auto;
			listView.LoadMoreCommandParameter = listView;
			listView.LoadMoreCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);

			if (Device.Idiom == TargetIdiom.Phone && Device.RuntimePlatform == Device.Android)
			{
				searchBar.HeightRequest = 40;
			}
		}

		protected override async void OnAppearing()
		{
			try
			{
				string sql = query_ + " LIMIT " + recToLoad_.ToString();
				recTotal_ = await dbcon_.Table<Artanag>().CountAsync();
				var anaList = await dbcon_.QueryAsync<Artanag>(sql);
				recLoaded_ = anaList.Count;
				foreach(var ana in anaList)
				{
					ana.ana_desc = ana.ana_desc1 + " " + ana.ana_desc2;
					ana.ana_desc = ana.ana_desc.Trim();
					ana.ana_img_path = await GetImagePath(ana.ana_codice);
				}
				listView.ItemsSource = new ObservableCollection<Artanag>(anaList);
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore!", ex.Message, "OK");
			}
			base.OnAppearing();
		}

		private bool CanLoadMoreItems(object obj)
		{
			if (recLoaded_ >= recTotal_)
				return false;
			return true;
		}

		private async void LoadMoreItems(object obj)
		{
			if (listView.ItemsSource == null) return;
			listView.IsBusy = true;
			var collection = (ObservableCollection<Artanag>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var anaList = await dbcon_.QueryAsync<Artanag>(sql);
			foreach (Artanag ana in anaList)
			{
				ana.ana_desc = ana.ana_desc1 + " " + ana.ana_desc2;
				ana.ana_desc = ana.ana_desc.Trim();
				ana.ana_img_path = await GetImagePath(ana.ana_codice);
				collection.Add(ana);
			}
			recLoaded_ = collection.Count;
			listView.IsBusy = false;
		}

		async void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (e.OldTextValue == e.NewTextValue) return;
			listView.IsBusy = true;
			if (String.IsNullOrWhiteSpace(e.NewTextValue))
			{
				query_ = "SELECT * FROM artanag ORDER BY ana_desc1";
				recTotal_ = await dbcon_.Table<Artanag>().CountAsync();
			}
			else
			{
				query_ = "SELECT COUNT(*) FROM artanag WHERE ana_desc1 LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM artanag WHERE ana_desc1 LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ") ORDER BY ana_desc1";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var anaList = await dbcon_.QueryAsync<Artanag>(sql);
			foreach (var ana in anaList)
			{
				ana.ana_desc = ana.ana_desc1 + " " + ana.ana_desc2;
				ana.ana_desc = ana.ana_desc.Trim();
				ana.ana_img_path = await GetImagePath(ana.ana_codice);
			}
			recLoaded_ = anaList.Count;
			listView.ItemsSource = new ObservableCollection<Artanag>(anaList);
			listView.IsBusy = false;
		}


		protected async Task<string> GetImagePath(string codart)
		{
			if (!string.IsNullOrWhiteSpace(codart))
			{
				IFolder rootFolder = FileSystem.Current.LocalStorage;
				IFolder imagesFolder = await rootFolder.CreateFolderAsync("images", CreationCollisionOption.OpenIfExists);

				String fileName = codart.Trim() + "_0.PNG";
				ExistenceCheckResult status = await imagesFolder.CheckExistsAsync(fileName);
				if (status == ExistenceCheckResult.FileExists)
				{
					return (rootFolder.Path + "/images/" + fileName);
				}
				fileName = codart.Trim() + "_0.JPG";
				status = await imagesFolder.CheckExistsAsync(fileName);
				if (status == ExistenceCheckResult.FileExists)
				{
					return(rootFolder.Path + "/images/" + fileName);
				}
			}
			return("header_wallpaper.jpg");
			//return (null);
		}

		public SfListView AnaList { get { return listView; } }
	}
}
