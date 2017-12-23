using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
			recToLoad_ = 20;
			query_ = "SELECT * FROM clienti1 ORDER BY cli_desc";

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
				query_ = "SELECT COUNT(*) FROM clienti1  WHERE cli_desc LIKE(" + StringSqlQuote(e.NewTextValue.Trim(), true) + ")";
				recTotal_ = await dbcon_.ExecuteScalarAsync<int>(query_);
				query_ = "SELECT * FROM clienti1  WHERE cli_desc LIKE(" + StringSqlQuote(e.NewTextValue.Trim(), true) + ") ORDER BY cli_desc";
			}
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			var cliList = await dbcon_.QueryAsync<Clienti>(sql);
			recLoaded_ = cliList.Count;
			listView.ItemsSource = new ObservableCollection<Clienti>(cliList);
			listView.IsBusy = false;
		}

		async void OnItemDoubleTapped(object sender, Syncfusion.ListView.XForms.ItemDoubleTappedEventArgs e)
		{
			if (!e.Handled)
			{
				MessagingCenter.Send(this,"ClienteChanged",(Clienti)e.ItemData);
				await Navigation.PopAsync();
			}
		}
	}
}
