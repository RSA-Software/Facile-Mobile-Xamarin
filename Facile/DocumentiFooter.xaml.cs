using System;
using System.Collections.Generic;
using Facile.Extension;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiFooter : ContentPage
	{
		private DocumentiEdit _parent;
		private bool change_event;
		private readonly SQLiteAsyncConnection dbcon_;

		public DocumentiFooter(DocumentiEdit par)
		{
			_parent = par;
			change_event = true;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();

			if (Device.Idiom == TargetIdiom.Tablet)
			{
				m_colli_text.WidthRequest = 100;
				m_colli.WidthRequest = 100;

				m_cod_pag_text.WidthRequest = 100;
				m_cod_pag.WidthRequest = 100;

				grid.RowSpacing = grid.RowSpacing += 20;
			}
		}

		public void SetProtection()
		{
			m_tot_merce.IsEnabled = false;
			m_tot_netto.IsEnabled = false;
			m_imponibile.IsEnabled = false;
			m_imposta.IsEnabled = false;
			m_totale.IsEnabled = false;

			m_sconto.IsEnabled = _parent.doc.fat_editable;
			m_colli.IsEnabled = _parent.doc.fat_editable;
			m_cod_pag.IsEnabled = _parent.doc.fat_editable;
			m_acconto.IsEnabled = _parent.doc.fat_editable;

			if (_parent.doc.fat_tipo == (short)DocTipo.TIPO_ORD)
			{
				m_consegna_text.Height = 20;
				m_consegna_val.Height  = 50;
				m_d_consegna.IsEnabled = _parent.doc.fat_editable;
				m_d_consegna.IsVisible = true;
			}
			else
			{
				m_d_consegna.IsEnabled = false;
				m_d_consegna.IsVisible = false;
				m_consegna_text.Height = 0;
				m_consegna_val.Height  = 0;
			}

			m_salva.IsVisible = _parent.doc.fat_editable;
			m_salva.IsEnabled = _parent.doc.fat_editable;
		}

		public void SetBusy(bool isBusy)
		{
			busyIndicator.IsBusy = isBusy;	
		}

		public void SetField()
		{
			change_event = false;
			m_tot_merce.Value = _parent.doc.fat_tot_merce;
			m_sconto.Value = _parent.doc.fat_sconto;
			m_tot_netto.Value = _parent.doc.fat_tot_netto;
			m_colli.Value = _parent.doc.fat_colli;
			m_cod_pag.Value = _parent.doc.fat_pag;

			m_imponibile.Value = _parent.doc.fat_totale_imponibile;
			m_imposta.Value = _parent.doc.fat_tot_iva;
			m_totale.Value = _parent.doc.fat_tot_fattura;
			m_acconto.Value = _parent.doc.fat_anticipo;
			change_event = true;
		}

		public void GetField()
		{
			_parent.doc.fat_tot_merce = m_tot_merce.Value == null ?  0 : double.Parse(m_tot_merce.Value.ToString());
			_parent.doc.fat_sconto    = m_sconto.Value == null ? 0 : double.Parse(m_sconto.Value.ToString());
			_parent.doc.fat_tot_netto = m_tot_netto.Value == null ? 0 : double.Parse(m_tot_netto.Value.ToString());
			_parent.doc.fat_colli     = m_colli.Value == null ? 0 : int.Parse(m_colli.Value.ToString());
			_parent.doc.fat_pag       = m_cod_pag.Value == null ? 0 : int.Parse(m_cod_pag.Value.ToString());

			_parent.doc.fat_totale_imponibile = m_imponibile.Value == null ? 0 : double.Parse(m_imponibile.Value.ToString());
			_parent.doc.fat_tot_iva           = m_imposta.Value == null ? 0 : double.Parse(m_imposta.Value.ToString());
			_parent.doc.fat_tot_fattura       = m_totale.Value == null ? 0 : double.Parse(m_totale.Value.ToString());
			_parent.doc.fat_anticipo          = m_acconto.Value == null ? 0 : double.Parse(m_acconto.Value.ToString());
		}


		async void OnScontoChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
		{
			if (!change_event) return;

			GetField();
			try
			{
				await _parent.doc.GetTotaliAsync();
			}
			catch (Exception ex)
			{
				await DisplayAlert("Errore", ex.Message, "OK");
			}
			SetField();
		}

		void OnColliChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
		{
			if (!change_event) return;

			_parent.doc.fat_recalc_colli = 0;
		}


		async void OnPagamentoChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
		{
			if (!change_event) return;

			GetField();
			try
			{
				var pag = await dbcon_.GetAsync<Pagamenti>(_parent.doc.fat_pag);
				_parent.doc.fat_pag = pag.pag_codice;
			}
			catch
			{
				_parent.doc.fat_pag = 0;
			}
			SetField();
		}

		void OnCodPagClicked(object sender, System.EventArgs e)
		{
			var page = new PagamentiBr();
			page.PagList.ItemDoubleTapped += (source, args) =>
			{
				var pag = (Pagamenti)args.ItemData;
				_parent.doc.fat_pag = pag.pag_codice;
				SetField();
				Navigation.PopModalAsync();
			};
			Navigation.PushModalAsync(page);	
		}

		async void OnSaveClicked(object sender, System.EventArgs e)
		{
			GetField();
			if ((_parent.doc.fat_editable) && (_parent.doc.fat_pag == 0))
			{
				await DisplayAlert("Attenzione!", "Indicare il tipo di Pagamento.", "OK");
				m_cod_pag.Focus();
				return;
			}
			if ((_parent.doc.fat_editable) && (_parent.doc.fat_tipo == (short)DocTipo.TIPO_ORD) && (_parent.doc.fat_d_doc > _parent.doc.fat_d_consegna))
			{
				await DisplayAlert("Attenzione!", "La data di consegna deve essere successiva o uguale alla data del documento.", "OK");
				m_d_consegna.Focus();
				return;
			}
			busyIndicator.IsBusy = true;
			await dbcon_.UpdateAsync(_parent.doc);
			busyIndicator.IsBusy = false;
		}
	}
}
