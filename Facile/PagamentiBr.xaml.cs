using System;
using System.Collections.ObjectModel;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PagamentiBr : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;

		public PagamentiBr()
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 20;
			query_ = "SELECT * FROM pagament ORDER BY pag_desc";
		
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
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			recTotal_ = await dbcon_.Table<Pagamenti>().CountAsync();
			var pagList = await dbcon_.QueryAsync<Pagamenti>(sql);
			recLoaded_ = pagList.Count;
			listView.ItemsSource = new ObservableCollection<Pagamenti>(pagList);
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
			var collection = (ObservableCollection<Pagamenti>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var pagList = await dbcon_.QueryAsync<Pagamenti>(sql);
			foreach (Pagamenti pag in pagList)
			{
				collection.Add(pag);
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
				query_ = "SELECT * FROM pagament ORDER BY pag_desc";
				recTotal_ = await dbcon_.Table<Pagamenti>().CountAsync();
			}
			else
			{
				query_ = "SELECT COUNT(*) FROM pagament  WHERE pag_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM pagament  WHERE pag_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ") ORDER BY pag_desc";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var pagList = await dbcon_.QueryAsync<Pagamenti>(sql);
			recLoaded_ = pagList.Count;
			listView.ItemsSource = new ObservableCollection<Pagamenti>(pagList);
			listView.IsBusy = false;
		}

		public SfListView PagList { get { return listView; } }
	}
}
