using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using SQLite;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LottiSearch : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;
		private readonly string cod_art_;

		public LottiSearch(string codart)
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			cod_art_ = codart;
			query_ = "SELECT * FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false) + " ORDER BY lot_scadenza";

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
			string sql = "SELECT COUNT(*) FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false);
			recTotal_ = await dbcon_.ExecuteScalarAsync<int>(sql);

			sql = query_ + " LIMIT " + recToLoad_.ToString();
			var lotList = await dbcon_.QueryAsync<Lotti>(sql);

			recLoaded_ = lotList.Count;
			listView.ItemsSource = new ObservableCollection<Lotti>(lotList);
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
			listView.IsBusy = true;
			var collection = (ObservableCollection<Lotti>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var lotList = await dbcon_.QueryAsync<Lotti>(sql);
			foreach (var lot in lotList)
			{
				collection.Add(lot);
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
				query_ = "SELECT COUNT(*) FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false);
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false) + " ORDER BY lot_scadenza";
			}
			else
			{
				query_ = "SELECT COUNT(*) FROM lotti1 WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false) + " AND lot_lotto LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM lotti1  WHERE lot_stop IS NULL AND lot_start IS NOT NULL AND lot_codice = " + cod_art_.SqlQuote(false) + " AND lot_lotto LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ") ORDER BY lot_scadenza";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var lotList = await dbcon_.QueryAsync<Lotti>(sql);
			recLoaded_ = lotList.Count;
			listView.ItemsSource = new ObservableCollection<Lotti>(lotList);
			listView.IsBusy = false;
		}

		public SfListView LotList { get { return listView; } }
	}
}
