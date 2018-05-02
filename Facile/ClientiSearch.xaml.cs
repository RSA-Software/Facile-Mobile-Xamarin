using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.ListView.XForms;
using Facile.Extension;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ClientiSearch : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;

		public ClientiSearch()
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			query_ = "SELECT * FROM clienti1 ORDER BY cli_desc";

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
			recTotal_ = await dbcon_.Table<Clienti>().CountAsync();
			var cliList = await dbcon_.QueryAsync<Clienti>(sql);
			recLoaded_ = cliList.Count;
			listView.ItemsSource = new ObservableCollection<Clienti>(cliList);
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
			var collection = (ObservableCollection<Clienti>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var cliList = await dbcon_.QueryAsync<Clienti>(sql);
			foreach (Clienti cli in cliList)
			{
				collection.Add(cli);
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
				query_ = "SELECT * FROM clienti1 ORDER BY cli_desc";
				recTotal_ = await dbcon_.Table<Clienti>().CountAsync();
			}
			else
			{
				query_ = "SELECT COUNT(*) FROM clienti1  WHERE cli_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM clienti1  WHERE cli_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ") ORDER BY cli_desc";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var cliList = await dbcon_.QueryAsync<Clienti>(sql);
			recLoaded_ = cliList.Count;
			listView.ItemsSource = new ObservableCollection<Clienti>(cliList);
			listView.IsBusy = false;
		}

		public SfListView CliList { get { return listView; }}
	}
}
