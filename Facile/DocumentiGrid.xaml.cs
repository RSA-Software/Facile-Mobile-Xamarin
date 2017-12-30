using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using Facile.ViewModels;
using SQLite;
using Syncfusion.Data;
using Syncfusion.SfBusyIndicator.XForms;
using Syncfusion.SfDataGrid.XForms;
using Xamarin.Forms;

namespace Facile
{
	public enum TipoDocumento
	{
		TIPO_FAT = 0,
		TIPO_BOL = 1,
		TIPO_DDT = 2,
		TIPO_BUO = 3,
		TIPO_ACC = 4,
		TIPO_RIC = 5,
		TIPO_PRE = 6,
		TIPO_ORD = 7,
		TIPO_FAR = 8,
		TIPO_OFO = 9,
		TIPO_AUF = 10,
		TIPO_RIO = 11,
		TIPO_DRI = 12,
		TIPO_FPF = 13,
	}

	//[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentiGrid : ContentPage
	{
		private readonly TipoDocumento tipo_;
		private readonly SQLiteAsyncConnection dbcon_;
		private string query_;
		private string filter_;
		private int swipeIndex;
		private Image leftImage;
		private int cliCodice_;

		ObservableCollection<Documents> docCollection = null;

		public DocumentiGrid(TipoDocumento t_doc)
		{
			InitializeComponent();
			tipo_ = t_doc;
			cliCodice_ = 0;
			//cli_desc.Text = "TUTTI";
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			//MessagingCenter.Subscribe<ClientiSearch, Clienti>(this, "ClienteChanged", OnClienteChanged);

			busyIndicator.IsBusy = true;
			leftImage = null;
			swipeIndex = 0;
			switch(tipo_)
			{
				case TipoDocumento.TIPO_DDT :
					Title = "Documenti di Trasporto";
					break;

				case TipoDocumento.TIPO_FAT:
					Title = "Fatture";
					break;

				case TipoDocumento.TIPO_PRE:
					Title = "Preventivi";
					break;

				case TipoDocumento.TIPO_ORD:
					Title = "Ordini";
					break;

				default:
					Title = "*** Documento Sconosciuto ***";
					break;
			}
			//
			// Inserire Massimo e minimo per le date dopo aver impostato l'anno nelle impostazioni ditta
			// 
			dStart.Date = new DateTime(2016, 1, 1); // DateTime.Now;
			dStop.Date = DateTime.Now;


			query_ = "SELECT fat_tipo, fat_n_doc, fat_d_doc, fat_tot_fattura, fat_registro, cli_desc " +
				"FROM fatture2 " +
				"LEFT JOIN clienti1 on fat_inte = cli_codice";
			
			filter_ = String.Format(" WHERE fat_tipo = {0}", (int)tipo_);

			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;

			GridTableSummaryRow summaryRow1 = new GridTableSummaryRow();
			summaryRow1.Title = "Totale {Totale} - Numero Doc. : {DocCount}";
			summaryRow1.ShowSummaryInRow = true;
			summaryRow1.Position = Position.Bottom;
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "Totale",
				MappingName = "fat_tot_fattura",
				Format = "{Sum:c}",
				SummaryType = SummaryType.DoubleAggregate
			});
			summaryRow1.SummaryColumns.Add(new GridSummaryColumn()
			{
				Name = "DocCount",
				MappingName = "fat_tot_fattura",
				Format = "{Count:#,#}",
				SummaryType = SummaryType.CountAggregate
			});
			dataGrid.TableSummaryRows.Add(summaryRow1);

		}
		async protected override void OnAppearing()
		{
			string sql = query_ + filter_;
			var docList = await dbcon_.QueryAsync<Documents>(sql);
			foreach(Documents doc in docList)
			{
				if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT && doc.fat_credito != 0)
					doc.fat_tot_fattura = -doc.fat_tot_fattura;
			}
			if (docList.Count == 0)
			{
				await DisplayAlert("Attenzione!", "Dati non trovati", "OK");
				await Navigation.PopAsync();
			}
			else
			{
				docCollection = new ObservableCollection<Documents>(docList);
				dataGrid.ItemsSource = docCollection;
			}
			busyIndicator.IsBusy = false;
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			//MessagingCenter.Unsubscribe<ClientiSearch>(this, "ClienteChanged");
			base.OnDisappearing();
		}

		async void OnDateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
		{
			if (!String.IsNullOrEmpty(query_))
			{
				busyIndicator.IsBusy = true;
				string where;
				if (cliCodice_ != 0)
					where = String.Format(" AND fat_d_doc BETWEEN {0} AND {1} AND fat_inte = {2}", dStart.Date.Ticks,dStop.Date.Ticks, cliCodice_);
				else 
					where = String.Format(" AND fat_d_doc BETWEEN {0} AND {1}", dStart.Date.Ticks, dStop.Date.Ticks);
				string sql = query_ + filter_ + where;
				var docList = await dbcon_.QueryAsync<Documents>(sql);
				foreach (Documents doc in docList)
				{
					if (doc.fat_tipo == (int)TipoDocumento.TIPO_FAT && doc.fat_credito != 0)
						doc.fat_tot_fattura = -doc.fat_tot_fattura;
				}
				docCollection = new ObservableCollection<Documents>(docList);
				dataGrid.ItemsSource = docCollection;
				busyIndicator.IsBusy = false;
			}
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
					    
			if (Device.Idiom != TargetIdiom.Phone)
			{
				if (width > height)
				{
					if (Device.RuntimePlatform == Device.iOS)
					{
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .1));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.9));
					}
					if (Device.RuntimePlatform == Device.Android)
					{
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .09));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.91));
					}
				}
				else
				{
					if (Device.RuntimePlatform == Device.iOS)
					{
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .08));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.92));
					}
					if (Device.RuntimePlatform == Device.Android)
					{ 
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .05));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.95));
					}
				}
			}
			else
			{
				var cols = dataGrid.Columns;
				if (width > height)
				{
					foreach (var col in cols)
					{
						if (col.HeaderText == "Tipo")
							col.IsHidden = false;
					}

					if (Device.RuntimePlatform == Device.iOS)
					{
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .16));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.86));
					}
				}
				else
				{
					foreach (var col in cols)
					{
						if (col.HeaderText == "Tipo")
							col.IsHidden = true;
					}
					if (Device.RuntimePlatform == Device.iOS)
					{
						AbsoluteLayout.SetLayoutBounds(table, new Rectangle(0, 0, 1, .09));
						AbsoluteLayout.SetLayoutBounds(dataGrid, new Rectangle(0, 1, 1, 0.91));
					}

			

				}
			}
		}

		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			swipeIndex = e.RowIndex;
		}


		void OnLeftBindingContextChanged(object sender, System.EventArgs e)
		{
			if (leftImage == null)
			{
				leftImage = sender as Image;
				(leftImage.Parent as View).GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(Edit) });
				//leftImage.Source = ImageSource.FromResource("SampleBrowser.Icons.DataGrid.Edit.png");
			}
		}

		private async void Edit ()
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex == 0)
			{
				return;
			}

			var doc = docCollection[swipeIndex-1];

			Fatture fat = null;
			bool nuova = false;
			bool editable = false;

			swipeIndex = 0;
			try
			{
				string sql = String.Format("SELECT * from FATTURE2 WHERE fat_tipo = {0} AND fat_n_doc = {1} LIMIT 1", doc.fat_tipo, doc.fat_n_doc);
				var docList = await dbcon_.QueryAsync<Fatture>(sql);
				if (docList.Count > 0)
					fat = docList[0];
			}
			catch (Exception ex)
			{
				await DisplayAlert("Attenzione!", ex.Message, "OK");
				return;
			}
			if (fat == null) return;
			var page = new DocumentiEdit(ref fat, ref nuova, ref editable);
			await Navigation.PushAsync(page);
		}

		void OnTappedClienti(object sender, System.EventArgs e)
		{
			var page = new ClientiSearch();
			page.CliList.ItemDoubleTapped += (source, args) =>
			{
				var cli = (Clienti)args.ItemData;
				cliCodice_ = cli.cli_codice;

			};
			Navigation.PushAsync(page);
		}

		void OnClienteChanged(ClientiSearch source, Clienti cli)
		{
			if (cli.cli_codice == cliCodice_) return;
			cliCodice_ = cli.cli_codice;
			//cli_desc.Text = cli.cli_desc;
		}

	}
}
