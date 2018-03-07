using System;
using System.Collections.Generic;
using Facile.Extension;
using Facile.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Facile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DocumentRow : ContentPage
	{
		protected FatRow rig_;
		private bool change_;

		public DocumentRow(ref FatRow rig)
		{
			rig_ = rig;
			change_ = false;
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			SetField();
		}

		public void SetField()
		{
			m_art.Text = rig_.rig_art;
			m_desc.Text = rig_.rig_newdes;
			m_quantita.Value = rig_.rig_qta;
			m_prezzo.Value = rig_.rig_prezzo;
			m_sco1.Value = rig_.rig_sconto1;
			m_sco2.Value = rig_.rig_sconto2;
			m_sco3.Value = rig_.rig_sconto3;
			m_totale.Value = rig_.rig_importo;
			m_sostituzione.IsToggled = rig_.rig_sost != 0 ? true : false;
		}

		public void GetField()
		{
			rig_.rig_art = m_art.Text;
			rig_.rig_newdes = m_desc.Text;

			rig_.rig_qta = double.Parse(m_quantita.Value.ToString());
			rig_.rig_prezzo = double.Parse(m_prezzo.Value.ToString());
			rig_.rig_sconto1 = double.Parse(m_sco1.Value.ToString());
			rig_.rig_sconto2 = double.Parse(m_sco2.Value.ToString());
			rig_.rig_sconto3 = double.Parse(m_sco3.Value.ToString());

			if (m_totale.Value.GetType().Equals(typeof(decimal)))
				rig_.rig_importo = decimal.ToDouble((decimal)m_totale.Value);
			else
				rig_.rig_importo = (double)m_totale.Value;
			
			rig_.rig_sost = m_sostituzione.IsToggled ? 1 : 0;
		}

		async void OnValueChanged(object sender, Syncfusion.SfNumericTextBox.XForms.ValueEventArgs e)
		{
			if (!change_)
			{
				change_ = true;
				GetField();
				await rig_.RecalcAsync();
				SetField();
				change_ = false;
			}
		}

		async void OnaChangedQta(object sender, Syncfusion.SfNumericUpDown.XForms.ValueEventArgs e)
		{
			if (!change_)
			{
				change_ = true;
				GetField();
				await rig_.RecalcAsync();
				SetField();
				change_ = false;
			}
		}

		async void OnSwitchToggled(object sender, Xamarin.Forms.ToggledEventArgs e)
		{
			if (!change_)
			{
				change_ = true;
				GetField();
				await rig_.RecalcAsync();
				SetField();
				change_ = false;
			}
		}

		void OnArticoloTapped(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new ArticoliSearch());
		}
	}
}
