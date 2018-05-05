using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiBody : ContentPage
	{
		private DocumentiEdit _parent;
		private readonly SQLiteAsyncConnection dbcon_;
		public ObservableCollection <FatRow> rigCollection = null;
		private int swipeIndex;

		public DocumentiBody(DocumentiEdit par)
		{
			swipeIndex = 0;		
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			_parent = par;
			InitializeComponent();
			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;
			dataGrid.GridLongPressed += DataGrid_GridLongPressed; 
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		public async Task SetItemSource ()
		{
			busyIndicator.IsBusy = true;
			dataGrid.ItemsSource = null;
			string sql = String.Format("SELECT * FROM fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1}", _parent.doc.fat_tipo, _parent.doc.fat_n_doc);
			var rigList = await dbcon_.QueryAsync<FatRow>(sql);
			rigCollection = new ObservableCollection<FatRow>(rigList);
			dataGrid.ItemsSource = rigCollection;

			m_add.IsEnabled = _parent.doc.fat_editable;
			m_add.IsVisible = _parent.doc.fat_editable;
			busyIndicator.IsBusy = false;
		}


		async void DataGrid_GridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			var rig = e.RowData as FatRow;
			var page = new DocumentRow(this, ref rig, rigCollection.IndexOf(rig), _parent.doc.fat_editable);
			await this.Navigation.PushModalAsync(page);
		}

		async void OnAddClicked(object sender, System.EventArgs e)
		{
			var app = (App)Application.Current;
			LocalImpo lim = null;
			//
			// Leggiamo le impostazioni
			//
			try
			{
				lim = await dbcon_.GetAsync<LocalImpo>(1);
			}
			catch
			{
				await DisplayAlert("Attenzione!", "Impostazioni locali non trovate!\nRiavviare l'App.", "OK");
				return;
			}

			if (lim.dep == 0)
			{
				await DisplayAlert("Attenzione!", "Deposito non impostato!\nEffettuare le impostazioni iniziali.", "OK");
				return;
			}


			var rig = new FatRow();
			rig.rig_tipo = _parent.doc.fat_tipo;
			rig.rig_n_doc = _parent.doc.fat_n_doc;
			rig.rig_dep = lim.dep;
			if (app.facile_db_impo != null) rig.rig_iva_inclusa = app.facile_db_impo.dit_iva_inc;
			rig.rig_coef_mol = 1;
			rig.rig_coef_mol2 = 1;
			var page = new DocumentRow(this, ref rig, -1, _parent.doc.fat_editable);
			await Navigation.PushModalAsync(page);

		}

		//
		// Otteniamo l'indice della riga per cui si è fatto lo swipe
		//
		// Se il documento non è editabile lo swiping viene cancellato
		// Riattivare il codice commentato nella versione definitiva
		//
		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			if (!_parent.doc.fat_editable)
			{
				e.Cancel = true;
				return;
			}
			swipeIndex = e.RowIndex;
		}

		async private void OnTapEdit(object sender, EventArgs args)
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex > 0 && rigCollection != null)
			{
				var rig = rigCollection[swipeIndex - 1];
				var page = new DocumentRow(this, ref rig, swipeIndex - 1, _parent.doc.fat_editable);
				await this.Navigation.PushModalAsync(page);
			}
		}

		private async void OnTapDelete(object sender, EventArgs args)
		{
			dataGrid.ResetSwipeOffset();
			if (swipeIndex > 0 && rigCollection != null)
			{ 
				if (await DisplayAlert("Attenzione!", "Confermi la cancellazione della riga?", "Si", "No"))
				{
					var rig = rigCollection[swipeIndex - 1];
					if (await dbcon_.DeleteAsync(rig) != 0)
						rigCollection.RemoveAt(swipeIndex - 1);
					else
						await DisplayAlert("Attenzione!", "Non è stato possibile eliminare la riga", "Ok");
				}
			}
		}

		private async void OnTapAdd(object sender, EventArgs args)
		{
			if (swipeIndex > 0 && rigCollection != null)
			{
				var rig = rigCollection[swipeIndex - 1];
				rig.rig_qta += 1;
				await rig.RecalcAsync();
				if (await dbcon_.UpdateAsync(rig) == 1)
				{
					rigCollection.RemoveAt(swipeIndex - 1);
					rigCollection.Insert(swipeIndex -1, rig);
				}
				else
				{
					rig.rig_qta -= 1;
					await rig.RecalcAsync();
				}
			}
		}

		private async void OnTapMin(object sender, EventArgs args)
		{
			if (swipeIndex > 0 && rigCollection != null)
			{
				var rig = rigCollection[swipeIndex - 1];
				rig.rig_qta -= 1;
				await rig.RecalcAsync();
				if (await dbcon_.UpdateAsync(rig) == 1)
				{
					rigCollection.RemoveAt(swipeIndex - 1);
					rigCollection.Insert(swipeIndex - 1, rig);
				}
				else
				{
					rig.rig_qta -= 1;
					await rig.RecalcAsync();
				}
			}
		}

	}
}
