using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Helpers;
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

		ObservableCollection<Pagamenti> PagList = null;

		public Pagamenti pagSel = null;

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
		}

		protected override async void OnAppearing()
		{
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			recTotal_ = await dbcon_.Table<Pagamenti>().CountAsync();
			var pag = await dbcon_.QueryAsync<Pagamenti>(sql);
			PagList = new ObservableCollection<Pagamenti>(pag);
			recLoaded_ = pag.Count;
			listView.ItemsSource = PagList;
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
			var pagamenti = (ObservableCollection<Pagamenti>)listView.ItemsSource;
			string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
			var pagList = await dbcon_.QueryAsync<Pagamenti>(sql);
			foreach (Pagamenti pag in pagList)
			{
				pagamenti.Add(pag);
			}
			recLoaded_ = pagamenti.Count;
			listView.IsBusy = false;
		}

		string StringSqlQuote(string str, bool jolly)
		{
			str = str.Replace("\\", "\\\\");
			str = str.Replace("*", "%");
			str = str.Replace("?", "_");

			string dest = "'";
			if (!string.IsNullOrEmpty(str))
			{
				if (jolly && !str.StartsWith("%", StringComparison.CurrentCulture) && !str.StartsWith("_", StringComparison.CurrentCulture)) dest += "%";
				dest += str;
			}
			if (jolly && !dest.EndsWith("%", StringComparison.CurrentCulture) && !dest.EndsWith("_", StringComparison.CurrentCulture)) dest += "%";
			dest += "'";

			return (dest);
		}

		async void OnSearchButtonPressed(object sender, System.EventArgs e)
		{
			SearchBar search = sender as SearchBar;

			if (!String.IsNullOrWhiteSpace(search.Text))
			{
				query_ = "SELECT COUNT(*) FROM pagament  WHERE pag_desc LIKE(" + StringSqlQuote(search.Text.Trim(), true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM pagament WHERE pag_desc LIKE(" + StringSqlQuote(search.Text.Trim(), true) + ") ORDER BY pag_desc";
				string sql = query_ + " LIMIT " + recToLoad_.ToString();
				var pag = await dbcon_.QueryAsync<Pagamenti>(sql);
				PagList = new ObservableCollection<Pagamenti>(pag);
				recLoaded_ = PagList.Count;
				listView.ItemsSource = PagList;		
			}
		}

		async void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (String.IsNullOrWhiteSpace(e.NewTextValue))
			{
				query_ = query_ = "SELECT * FROM pagament ORDER BY pag_desc";
				recTotal_ = await dbcon_.Table<Pagamenti>().CountAsync();
				string sql = query_ + " LIMIT " + recToLoad_.ToString();
				var pag = await dbcon_.QueryAsync<Pagamenti>(sql);
				PagList = new ObservableCollection<Pagamenti>(pag);
				recLoaded_ = PagList.Count;
				listView.ItemsSource = PagList;
				listView.Focus();
			}
		}

		async void OnItemDoubleTapped(object sender, Syncfusion.ListView.XForms.ItemDoubleTappedEventArgs e)
		{
			if (!e.Handled)
			{
				pagSel = (Pagamenti)e.ItemData;
				await Navigation.PopAsync();
			}
		}

		void Handle_Tapped(object sender, System.EventArgs e)
		{
			var grid = sender as Grid; 
			var pizzaInfo = grid.BindingContext as Pagamenti;
			listView.SelectedItem = pizzaInfo;

			listView.SelectionBackgroundColor = Color.Blue;
			listView.RefreshView();
		}
	}
}
