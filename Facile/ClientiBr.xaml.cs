using System;
using System.Collections.Generic;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class ClientiBr : ContentPage
	{
		private SQLiteAsyncConnection dbcon_;
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;

		public ClientiBr()
		{
			InitializeComponent();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 100;
			query_ = "SELECT * FROM clienti1 ORDER BY cli_desc";
		}

		protected override async void OnAppearing()
		{
			string sql = query_ + " LIMIT " + recToLoad_.ToString();
			recTotal_ = await dbcon_.Table<Clienti>().CountAsync();
			var clienti = await dbcon_.QueryAsync<Clienti>(sql);
			recLoaded_ = clienti.Count;
			ListView.ItemsSource = clienti;
			base.OnAppearing();
		}

		async void OnItemAppearing(object sender, Xamarin.Forms.ItemVisibilityEventArgs e)
		{
			if (recLoaded_ < recTotal_)
			{
				var clienti = (List<Clienti>)ListView.ItemsSource;
				var cliente = (Clienti)e.Item;

				int idx = clienti.IndexOf(cliente);
				int count = recLoaded_;

				if (clienti.IndexOf(cliente) == recLoaded_ - 1)
				{
					string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
					var cli = await dbcon_.QueryAsync<Clienti>(sql);
					clienti.AddRange(cli);
					recLoaded_ = clienti.Count;
					ListView.IsRefreshing = true;
					ListView.ItemsSource = null;
					ListView.ItemsSource = clienti;
					ListView.ScrollTo(cliente, ScrollToPosition.End, false);
					ListView.IsRefreshing = false;
				}
			}
		}

		string StringSqlQuote (string str, bool jolly)
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
			if (jolly && !dest.EndsWith("%",StringComparison.CurrentCulture) && !dest.EndsWith("_", StringComparison.CurrentCulture)) dest += "%";
			dest += "'";

			return (dest);
		}

		async void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (e.OldTextValue == e.NewTextValue) return;

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
			var clienti = await dbcon_.QueryAsync<Clienti>(sql);
			recLoaded_ = clienti.Count;
			ListView.IsRefreshing = true;
			ListView.ItemsSource = null;
			ListView.ItemsSource = clienti;
			ListView.IsRefreshing = false;
		}
	}
}
