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
	public partial class RepartiFilter : ContentPage
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

		public RepartiFilter(bool modal)
		{
			first_ = true;
			recTotal_ = 0;
			recLoaded_ = 0;
			recToLoad_ = 50;
			last_search_ = "";
			api_url = "";

			query_ = "SELECT * FROM reparti ORDER BY rep_desc";
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
					string sql = $"SELECT * FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.REPARTO} ORDER BY fil_codice";
					filList_ = await dbcon_.QueryAsync<FiltersDb>(sql);
					recTotal_ = await dbcon_.Table<Reparti>().CountAsync();
					sql = query_ + " LIMIT " + recToLoad_.ToString();
					var repList = await dbcon_.QueryAsync<Reparti>(sql);
					recLoaded_ = repList.Count;
					foreach (var rep in repList)
					{
						rep.rep_desc = rep.rep_desc.ProperCase();
					}
					listView.SelectedItems.Clear();
					listView.ItemsSource = new ObservableCollection<Reparti>(repList);
					if ((filList_ != null) && (filList_.Count > 0))
					{
						foreach (var fil in filList_)
						{
							for (var idx = 0; idx < repList.Count; idx++)
							{
								if (repList[idx].rep_codice == fil.fil_codice)
								{
									listView.SelectedItems.Add(repList[idx]);
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
				var collection = (ObservableCollection<Reparti>)listView.ItemsSource;
				string sql = query_ + " LIMIT " + recToLoad_.ToString() + " OFFSET " + recLoaded_.ToString();
				var repList = await dbcon_.QueryAsync<Reparti>(sql);
				recLoaded_ += repList.Count;
				foreach (var rep in repList)
				{
					rep.rep_desc = rep.rep_desc.ProperCase();
					collection.Add(rep);
					foreach (var fil in filList_)
					{
						if (rep.rep_codice == fil.fil_codice)
						{
							listView.SelectedItems.Add(rep);
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
						query_ = "SELECT * FROM reparti ORDER BY rep_desc";
						recTotal_ = await dbcon_.Table<Reparti>().CountAsync();
					}
					else
					{
						query_ = $"SELECT * FROM reparti WHERE rep_desc LIKE({search.Trim().ToUpper().SqlQuote(true)}) ORDER BY rep_desc";
						recTotal_ = await dbcon_.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM reparti WHERE rep_desc LIKE({search.Trim().ToUpper().SqlQuote(true)})");
					}
					string sql = query_ + " LIMIT " + recToLoad_.ToString();
					var repList = await dbcon_.QueryAsync<Reparti>(sql);
					recLoaded_ = repList.Count;
					foreach (var rep in repList)
					{
						rep.rep_desc = rep.rep_desc.ProperCase();
					}
					listView.ItemsSource = new ObservableCollection<Reparti>(repList);
					foreach (var rep in repList)
					{
						if ((filList_ != null) && (filList_.Count > 0))
						{
							foreach (var fil in filList_)
							{
								for (var idx = 0; idx < repList.Count; idx++)
								{
									if (repList[idx].rep_codice == fil.fil_codice)
									{
										listView.SelectedItems.Add(repList[idx]);
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
					var rep = (Reparti)item;
					var fil = new FiltersDb();


					fil.fil_tipo = (short)FiltersType.REPARTO;
					fil.fil_codice = rep.rep_codice;
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
						var rep = (Reparti)item;
						var sql = $"DELETE FROM FiltersDb WHERE fil_tipo = {(short)FiltersType.REPARTO} AND fil_codice = {rep.rep_codice}";
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

		public SfListView RepList { get { return listView; } }
	}
}
