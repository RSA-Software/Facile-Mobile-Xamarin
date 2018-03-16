using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiBody : ContentPage
	{
		private Fatture doc_;
		private readonly SQLiteAsyncConnection dbcon_;
		ObservableCollection <FatRow> rigCollection = null;
		private int swipeIndex;

		public DocumentiBody(ref Fatture f)
		{
			doc_ = f;
			swipeIndex = 0;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
			SetField();
			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;

			dataGrid.GridLongPressed += DataGrid_GridLongPressed; 
		}

		private void SetField ()
		{
			//colli.Text = doc_.fat_colli.ToString();
		}

		private void GetField()
		{
			//doc_.fat_colli = Int32.Parse(colli.Text);
		}

		protected override async void OnAppearing()
		{
			string sql = String.Format("SELECT * FROM fatrow2 WHERE rig_tipo = {0} AND rig_n_doc = {1}", doc_.fat_tipo, doc_.fat_n_doc); 
			var rigList = await dbcon_.QueryAsync<FatRow>(sql);

			//foreach (FatRow row in rigList)
			//{
			//	await row.RecalcAsync();
			//}

			rigCollection = new ObservableCollection<FatRow>(rigList);
			dataGrid.ItemsSource = rigCollection;

			base.OnAppearing();
		}

		async void DataGrid_GridLongPressed(object sender, Syncfusion.SfDataGrid.XForms.GridLongPressedEventArgs e)
		{
			var rig = e.RowData as FatRow;
			var page = new DocumentRow(ref rig, false);
			await this.Navigation.PushModalAsync(page);
		}

		async void OnAddClicked(object sender, System.EventArgs e)
		{
			var ditta = await dbcon_.QueryAsync<Ditte>("SELECT * FROM impostazioni LIMIT 1");

			var rig = new FatRow();
			rig.rig_tipo = doc_.fat_tipo;
			rig.rig_n_doc = doc_.fat_n_doc;
			if (ditta.Count > 0) rig.rig_iva_inclusa = ditta[0].impo_iva_inc;
			rig.rig_coef_mol = 1;
			rig.rig_coef_mol2 = 1;
			var page = new DocumentRow(ref rig, true);
			await this.Navigation.PushModalAsync(page);
		}

		//
		// Otteniamo l'indice della riga per cui si è fatto lo swipe
		//
		// Se il documento non è editabile lo swiping viene cancellato
		// Riattivare il codice commentato nella versione definitiva
		//
		void OnSwipeStarted(object sender, Syncfusion.SfDataGrid.XForms.SwipeStartedEventArgs e)
		{
			//if (!doc_.fat_editable)
			//{
			//	e.Cancel = true;
			//	return;
			//}
			swipeIndex = e.RowIndex;
		}

		private void OnTapEdit(object sender, EventArgs args)
		{
			if (swipeIndex == 0) return;

			DisplayAlert("Click", "Edit Started", "ok");
		}

		private async void OnTapDelete(object sender, EventArgs args)
		{
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
			dataGrid.ResetSwipeOffset();
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
					rigCollection.Insert(swipeIndex - 1, rig);
					rigCollection.RemoveAt(swipeIndex);
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
