using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Facile.Interfaces;
using Facile.Models;
using Facile.Extension;
using SQLite;
using Syncfusion.ListView.XForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile.Articoli
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MarchiFilter : ContentPage
	{
		private int recTotal_;
		private int recLoaded_;
		private int recToLoad_;
		private string query_;
		private bool first_;
		private string last_search_;
		private string api_url;
		private List<FiltersDb> filList_;
		private readonly SQLiteAsyncConnection dbcon_;
		private readonly bool modal_;

		public MarchiFilter(bool modal)
		{
			first_ = true;
			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			last_search_ = "";
			api_url = "";
						
			query_ = "SELECT * FROM marchi ORDER BY mar_desc";
			filList_ = new List<FiltersDb>();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			modal_= modal;

			InitializeComponent();
			m_navigation.IsEnabled = modal;
			m_navigation.IsVisible = modal;

			listView.LoadMoreOption = LoadMoreOption.Auto;
			listView.LoadMoreCommandParameter = listView;
			listView.LoadMoreCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);

			searchBar.SearchCommand = new Command<object>(SearchCommand);
			if (Device.RuntimePlatform == Device.Android)
			{
				searchBar.HeightRequest = 42;
			}
			
		}

		protected override async void OnAppearing()
		{
			if (first_)
			{
				first_ = false;
				busyIndicator.IsBusy = true;

				try
				{
					string sql = $"SELECT * FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.MARCHIO} ORDER BY fil_codice";
					filList_ = await dbcon_.QueryAsync<FiltersDb>(sql);
					recTotal_ = await dbcon_.Table<Marchi>().CountAsync();
					sql = query_ + " LIMIT " + recToLoad_.ToString();
					var marList = await dbcon_.QueryAsync<Marchi>(sql);

					listView.SelectedItems.Clear();
					listView.ItemsSource = new ObservableCollection<Marchi>(marList);
					if ((filList_ != null) && (filList_.Count > 0))
					{
						foreach (var fil in filList_)
						{
							for (var idx = 0; idx < marList.Count; idx++)
							{
								if (marList[idx].mar_codice == fil.fil_codice)
								{
									listView.SelectedItems.Add(marList[idx]);
									break;
								}
							}
						}
					}
					recLoaded_ = marList.Count;
					busyIndicator.IsBusy = false;
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.ToString(), "OK");
					if (modal_)
						await Navigation.PopModalAsync();
					else
						await Navigation.PopAsync();
					return;
				}
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

			try
			{
				var collection = (ObservableCollection<Marchi>)listView.ItemsSource;
				string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
				var marList = await dbcon_.QueryAsync<Marchi>(sql);
				foreach (Marchi mar in marList)
				{
					collection.Add(mar);
					foreach (var fil in filList_)
					{
						if (mar.mar_codice == fil.fil_codice)
						{
							listView.SelectedItems.Add(mar);
							break;
						}
					}
				}
				recLoaded_ = collection.Count;
				listView.IsBusy = false;
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", ex.ToString(), "OK");
				if (modal_)
					await Navigation.PopModalAsync();
				else
					await Navigation.PopAsync();
				return;
			}
		}

		private async void SearchCommand(object obj)
		{
			var search = searchBar.Text ?? "";
			if (search != last_search_)
			{
				last_search_ = search;
				listView.IsBusy = true;
				busyIndicator.IsBusy = true;

				recTotal_ = 0;
				listView.SelectedItems.Clear();
				listView.ItemsSource = null;
				try
				{
					if (String.IsNullOrWhiteSpace(search))
					{
						query_ = "SELECT * FROM marchi ORDER BY mar_desc";
						recTotal_ = await dbcon_.Table<Marchi>().CountAsync();
					}
					else
					{
						query_ = $"SELECT * FROM marchi WHERE mar_desc LIKE({search.Trim().ToUpper().SqlQuote(true)}) ORDER BY mar_desc";
						recTotal_ = await dbcon_.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM marchi WHERE mar_desc LIKE({search.Trim().ToUpper().SqlQuote(true)})");
					}
					string sql = query_ + " LIMIT " + recToLoad_.ToString();
					var marList = await dbcon_.QueryAsync<Marchi>(sql);
					listView.ItemsSource = new ObservableCollection<Marchi>(marList);
					foreach (var mar in marList)
					{
						if ((filList_ != null) && (filList_.Count > 0))
						{
							foreach (var fil in filList_)
							{
								for (var idx = 0; idx < marList.Count; idx++)
								{
									if (marList[idx].mar_codice == fil.fil_codice)
									{
										listView.SelectedItems.Add(marList[idx]);
										break;
									}
								}
							}
						}
					}
					recLoaded_ = marList.Count;
					listView.IsBusy = false;
					busyIndicator.IsBusy = false;
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.ToString(), "OK");
					if (modal_)
						await Navigation.PopModalAsync();
					else
						await Navigation.PopAsync();
					return;
				}
			}
		}

		void OnTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
		{
			if (e.OldTextValue == null) return;
			if (e.NewTextValue == null) return;
			if (e.NewTextValue == "") SearchCommand(null);
		}

		void OnUnfocusedSearchBar(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			SearchCommand(null);
		}

		async void Handle_SelectionChanged(object sender, Syncfusion.ListView.XForms.ItemSelectionChangedEventArgs e)
		{			
			if ((e.AddedItems != null) && (e.AddedItems.Count > 0))
			{
				foreach (var item in e.AddedItems)
				{
					var mar = (Marchi)item;
					var fil = new FiltersDb();

					
					fil.fil_tipo = (short)FiltersType.MARCHIO;
					fil.fil_codice = mar.mar_codice;
					//fil.fil_desc = fil.fil_desc.FirstCharToUpper();
					try
					{
						await dbcon_.InsertAsync(fil);
					}
					catch (Exception ex)
					{
						await DisplayAlert("Errore", ex.Message, "OK");
					}
				}
			}

			if ((e.RemovedItems != null) && (e.RemovedItems.Count > 0))
			{
				foreach (var item in e.RemovedItems)
				{
					try
					{
						var mar = (Marchi)item;
						var sql = $"DELETE FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.MARCHIO} AND fil_codice = {mar.mar_codice}";
						await dbcon_.ExecuteAsync(sql);
					}
					catch (Exception ex)
					{
						await DisplayAlert("Errore", ex.Message, "OK");
					}
				}
			}
		}

		async void OnLeftArrowClicked(object sender, System.EventArgs e)
		{
			if (modal_)
				await Navigation.PopModalAsync();
			else
				await Navigation.PopAsync();
		}

		public SfListView MarList { get { return listView; } }
	}
}
