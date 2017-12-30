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
	public partial class DestinazioniSearch : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;
		private readonly int cod_cli_;

		public DestinazioniSearch(int cod_cli)
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			cod_cli_ = cod_cli;
			query_ = "SELECT * FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_ + " ORDER BY dst_desc";

			listView.LoadMoreOption = Syncfusion.ListView.XForms.LoadMoreOption.Auto;
			listView.LoadMoreCommandParameter = listView;
			listView.LoadMoreCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);

			if (Device.Idiom == TargetIdiom.Phone && Device.RuntimePlatform == Device.Android)
			{
				searchBar.HeightRequest = 25;
			}
		}

		protected override async void OnAppearing()
		{
			string sql  = "SELECT COUNT(*) FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_;
			recTotal_ = await dbcon_.ExecuteScalarAsync<int>(sql);

			sql = query_ + " LIMIT " + recToLoad_.ToString();
			var cliList = await dbcon_.QueryAsync<Destinazioni>(sql);

			recLoaded_ = cliList.Count;
			listView.ItemsSource = new ObservableCollection<Destinazioni>(cliList);
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
			var collection = (ObservableCollection<Destinazioni>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var dstList = await dbcon_.QueryAsync<Destinazioni>(sql);
			foreach (Destinazioni cli in dstList)
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
				query_ = "SELECT COUNT(*) FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_;
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM destina1 WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_ + " ORDER BY dst_desc";
			}
			else
			{
				query_ = "SELECT COUNT(*) FROM destina1  WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_ + " AND dst_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM destina1  WHERE dst_rel = 0 AND dst_cli_for = " + cod_cli_ + " AND dst_desc LIKE(" + e.NewTextValue.Trim().SqlQuote(true) + ") ORDER BY dst_desc";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var cliList = await dbcon_.QueryAsync<Destinazioni>(sql);
			recLoaded_ = cliList.Count;
			listView.ItemsSource = new ObservableCollection<Destinazioni>(cliList);
			listView.IsBusy = false;
		}

		public SfListView DstList { get { return listView; } }
	}
}
