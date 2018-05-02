using System;
using System.Collections.Generic;
using Facile.Models;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiFooter : ContentPage
	{
		private DocumentiEdit _parent;


		public DocumentiFooter(DocumentiEdit par)
		{
			_parent = par;
			InitializeComponent();
			SetProtection();
		}

		protected override void OnAppearing()
		{
			SetField();
			base.OnAppearing();
		}

		protected void SetProtection()
		{
			m_tot_merce.IsEnabled = false;
			m_tot_netto.IsEnabled = false;
			m_Imponibile.IsEnabled = false;
			m_Imposta.IsEnabled = false;
			m_totale.IsEnabled = false;

			m_sconto.IsEnabled = _parent.doc.fat_editable;
			m_colli.IsEnabled = _parent.doc.fat_editable;
			m_cod_pag.IsEnabled = _parent.doc.fat_editable;
			m_acconto.IsEnabled = _parent.doc.fat_editable;

			if (_parent.doc.fat_tipo == (short)DocTipo.TIPO_ORD)
			{
				m_consegna_text.IsVisible = true;
				m_consegna_val.IsVisible = true;
				m_d_consegna.IsEnabled = _parent.doc.fat_editable;
				m_consegna_text.IsEnabled = true;
				m_consegna_val.IsEnabled = true;
			}
			else
			{
				m_consegna_text.IsEnabled = false;
				m_consegna_val.IsEnabled = false;
				m_d_consegna.IsEnabled = false;
				m_consegna_text.IsVisible = false;
				m_consegna_val.IsVisible = false;
			}
		}

		public void SetBusy(bool isBusy)
		{
			busyIndicator.IsBusy = isBusy;	
		}

		public void SetField()
		{
			m_tot_merce.Value = _parent.doc.fat_tot_merce;
			m_sconto.Value = _parent.doc.fat_sconto;
			m_tot_netto.Value = _parent.doc.fat_tot_netto;
			m_colli.Value = _parent.doc.fat_colli;
			m_cod_pag.Value = _parent.doc.fat_pag;

			m_Imponibile.Value = _parent.doc.fat_totale_imponibile;
			m_Imposta.Value = _parent.doc.fat_tot_iva;
			m_totale.Value = _parent.doc.fat_tot_fattura;
			m_acconto.Value = _parent.doc.fat_anticipo;
		}


	}
}
