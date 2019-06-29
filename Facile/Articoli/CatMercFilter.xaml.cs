using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public partial class CatMercFilter : ContentPage
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

		public CatMercFilter(bool modal)
		{
			first_ = true;
			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			last_search_ = "";
			api_url = "";

			query_ = "SELECT * FROM catmerc1 ORDER BY mer_desc";
			filList_ = new List<FiltersDb>();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			modal_ = modal;

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
					string sql = $"SELECT * FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.CATMERC} ORDER BY fil_codice";
					filList_ = await dbcon_.QueryAsync<FiltersDb>(sql);
					recTotal_ = await dbcon_.Table<Catmerc>().CountAsync();
					sql = query_ + " LIMIT " + recToLoad_.ToString();
					var merList = await dbcon_.QueryAsync<Catmerc>(sql);
					recLoaded_ = merList.Count;
					foreach (var mer in merList)
					{
						mer.mer_desc = mer.mer_desc.ProperCase();
					}
					listView.SelectedItems.Clear();
					listView.ItemsSource = new ObservableCollection<Catmerc>(merList);
					if ((filList_ != null) && (filList_.Count > 0))
					{
						foreach (var fil in filList_)
						{
							for (var idx = 0; idx < merList.Count; idx++)
							{
								if (merList[idx].mer_codice == fil.fil_codice)
								{
									listView.SelectedItems.Add(merList[idx]);
									break;
								}
							}
						}
					}
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
				var collection = (ObservableCollection<Catmerc>)listView.ItemsSource;
				string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
				var merList = await dbcon_.QueryAsync<Catmerc>(sql);
				recLoaded_ += merList.Count;
				foreach (var mer in merList)
				{
					mer.mer_desc = mer.mer_desc.ProperCase();
					collection.Add(mer);
					foreach (var fil in filList_)
					{
						if (mer.mer_codice == fil.fil_codice)
						{
							listView.SelectedItems.Add(mer);
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
						query_ = "SELECT * FROM catmerc1 ORDER BY mer_desc";
						recTotal_ = await dbcon_.Table<Catmerc>().CountAsync();
					}
					else
					{
						query_ = $"SELECT * FROM catmerc1 WHERE mer_desc LIKE({search.Trim().ToUpper().SqlQuote(true)}) ORDER BY mer_desc";
						recTotal_ = await dbcon_.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM catmerc1 WHERE mer_desc LIKE({search.Trim().ToUpper().SqlQuote(true)})");
					}
					string sql = query_ + " LIMIT " + recToLoad_.ToString();
					var merList = await dbcon_.QueryAsync<Catmerc>(sql);
					recLoaded_ = merList.Count;
					foreach (var mer in merList)
					{
						mer.mer_desc = mer.mer_desc.ProperCase();
					}
					listView.ItemsSource = new ObservableCollection<Catmerc>(merList);
					foreach (var mer in merList)
					{
						if ((filList_ != null) && (filList_.Count > 0))
						{
							foreach (var fil in filList_)
							{
								for (var idx = 0; idx < merList.Count; idx++)
								{
									if (merList[idx].mer_codice == fil.fil_codice)
									{
										listView.SelectedItems.Add(merList[idx]);
										break;
									}
								}
							}
						}
					}
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
					var mer = (Catmerc)item;
					var fil = new FiltersDb();


					fil.fil_tipo = (short)FiltersType.CATMERC;
					fil.fil_codice = mer.mer_codice;
					fil.fil_desc = fil.fil_desc.ProperCase();
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
						var mer = (Catmerc)item;
						var sql = $"DELETE FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.CATMERC} AND fil_codice = {mer.mer_codice}";
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

		public SfListView MerList { get { return listView; } }
	}
}
