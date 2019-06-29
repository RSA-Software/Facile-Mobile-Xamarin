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
	public partial class FornitoriFilter : ContentPage
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

		public FornitoriFilter(bool modal)
		{
			first_ = true;
			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			last_search_ = "";
			api_url = "";

			query_ = "SELECT * FROM fornito1 ORDER BY for_desc";
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
					string sql = $"SELECT * FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.FORNITORE} ORDER BY fil_codice";
					filList_ = await dbcon_.QueryAsync<FiltersDb>(sql);
					recTotal_ = await dbcon_.Table<Fornitori>().CountAsync();
					sql = query_ + " LIMIT " + recToLoad_.ToString();
					var forList = await dbcon_.QueryAsync<Fornitori>(sql);
					recLoaded_ = forList.Count;
					foreach (var forn in forList)
					{
						forn.for_desc = forn.for_desc.ProperCase();
					}
					listView.SelectedItems.Clear();
					listView.ItemsSource = new ObservableCollection<Fornitori>(forList);
					if ((filList_ != null) && (filList_.Count > 0))
					{
						foreach (var fil in filList_)
						{
							for (var idx = 0; idx < forList.Count; idx++)
							{
								if (forList[idx].for_codice == fil.fil_codice)
								{
									listView.SelectedItems.Add(forList[idx]);
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
				var collection = (ObservableCollection<Fornitori>)listView.ItemsSource;
				string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
				var forList = await dbcon_.QueryAsync<Fornitori>(sql);
				recLoaded_ += forList.Count;
				foreach (var forn in forList)
				{
					forn.for_desc = forn.for_desc.ProperCase();
					collection.Add(forn);
					foreach (var fil in filList_)
					{
						if (forn.for_codice == fil.fil_codice)
						{
							listView.SelectedItems.Add(forn);
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
						query_ = "SELECT * FROM fornito1 ORDER BY for_desc";
						recTotal_ = await dbcon_.Table<Fornitori>().CountAsync();
					}
					else
					{
						query_ = $"SELECT * FROM fornito1 WHERE for_desc LIKE({search.Trim().ToUpper().SqlQuote(true)}) ORDER BY for_desc";
						recTotal_ = await dbcon_.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM fornito1 WHERE for_desc LIKE({search.Trim().ToUpper().SqlQuote(true)})");
					}
					string sql = query_ + " LIMIT " + recToLoad_.ToString();
					var forList = await dbcon_.QueryAsync<Fornitori>(sql);
					recLoaded_ = forList.Count;
					foreach (var forn in forList)
					{
						forn.for_desc = forn.for_desc.ProperCase();
					}
					listView.ItemsSource = new ObservableCollection<Fornitori>(forList);
					foreach (var forn in forList)
					{
						if ((filList_ != null) && (filList_.Count > 0))
						{
							foreach (var fil in filList_)
							{
								for (var idx = 0; idx < forList.Count; idx++)
								{
									if (forList[idx].for_codice == fil.fil_codice)
									{
										listView.SelectedItems.Add(forList[idx]);
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
					var forn = (Fornitori)item;
					var fil = new FiltersDb();


					fil.fil_tipo = (short)FiltersType.FORNITORE;
					fil.fil_codice = forn.for_codice;
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
						var forn = (Fornitori)item;
						var sql = $"DELETE FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.FORNITORE} AND fil_codice = {forn.for_codice}";
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

		public SfListView ForList { get { return listView; } }
	}
}
