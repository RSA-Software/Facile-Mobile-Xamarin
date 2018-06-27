using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using Newtonsoft.Json;
using PCLStorage;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
    public partial class SetupPage : ContentPage
    {
		private readonly SQLiteAsyncConnection dbcon_;
		private LocalImpo imp_;
		private bool first_;

        public SetupPage( )
        {
			first_ = true;
            InitializeComponent();

			imp_ = new LocalImpo();
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			m_age.FormatString = "N";
			m_age.Culture = new System.Globalization.CultureInfo("it-IT");

			m_server_picker.Items.Add("Facile - 01");
			m_server_picker.Items.Add("Facile - 02");
			m_server_picker.Items.Add("Facile - 03");

			m_reg_picker.Items.Add("A");
			m_reg_picker.Items.Add("B");
			m_reg_picker.Items.Add("C");
			m_reg_picker.Items.Add("D");
			m_reg_picker.Items.Add("E");
			m_reg_picker.Items.Add("F");
			m_reg_picker.Items.Add("G");
			m_reg_picker.Items.Add("H");
			m_reg_picker.Items.Add("I");
			m_reg_picker.Items.Add("J");
			m_reg_picker.Items.Add("K");
			m_reg_picker.Items.Add("L");
			m_reg_picker.Items.Add("M");
			m_reg_picker.Items.Add("N");
			m_reg_picker.Items.Add("O");
			m_reg_picker.Items.Add("P");
			m_reg_picker.Items.Add("Q");
			m_reg_picker.Items.Add("R");
			m_reg_picker.Items.Add("S");
			m_reg_picker.Items.Add("T");
			m_reg_picker.Items.Add("U");
			m_reg_picker.Items.Add("V");
			m_reg_picker.Items.Add("W");
			m_reg_picker.Items.Add("X");
			m_reg_picker.Items.Add("Y");
			m_reg_picker.Items.Add("Z");

			m_copy_picker.Items.Add("A RICHIESTA");
			m_copy_picker.Items.Add("AUTOMATICA");
			m_copy_picker.Items.Add("NON STAMPARE");

			SetField();
        }

		protected override async  void OnAppearing()
		{
			if (first_)
			{
				first_ = false;
				try
				{
					imp_ = await dbcon_.GetAsync<LocalImpo>(1);
				}
				catch
				{
					await DisplayAlert("Attenzione!", "Sto configurando il Database.\n\nAttendere qualche secondo e riprovare.", "OK");
					await Navigation.PopAsync();
					return;
				}
				SetField();
			}
			base.OnAppearing();
		}

		void GetField()
		{
			imp_.ftpServer = m_server_picker.SelectedItem != null ? m_server_picker.SelectedItem.ToString().Trim() : "";
			imp_.user = m_user.Text != null ? m_user.Text.Trim() : "";
			imp_.registro = m_reg_picker.SelectedItem != null ? m_reg_picker.SelectedItem.ToString().Trim() : "";

			imp_.printer = m_printer.Text != null ? m_printer.Text.Trim() : "";
			imp_.seconda_copia = m_copy_picker.SelectedIndex;

			try
			{
				imp_.age = m_age.Value == null ? 0 : Convert.ToInt32(m_age.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			try
			{
				imp_.dep = m_dep.Value == null ? 0 : Convert.ToInt32(m_dep.Value);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		void SetField()
		{
			if (imp_.ftpServer == "")
				m_server_picker.SelectedItem = null;
			else
				m_server_picker.SelectedItem = imp_.ftpServer;
			m_user.Text = imp_.user;
			m_age.Value = imp_.age;
			m_dep.Value = imp_.dep;
			if (imp_.registro == "")
				m_reg_picker.SelectedItem = null;
			else
				m_reg_picker.SelectedItem = imp_.registro;
			
			m_copy_picker.SelectedIndex = imp_.seconda_copia;
			m_printer.Text = imp_.printer;
		}

		async void OnClickedSalva(object sender, System.EventArgs e)
		{
			GetField();

			if (imp_.ftpServer == "")
			{
				m_server_picker.Focus();
				return;
			}
			if (imp_.user == "")
			{
				m_user.Focus();
				return;
			}
			if (imp_.age == 0)
			{
				m_age.Focus();
				return;
			}
			if (imp_.dep == 0)
			{
				m_dep.Focus();
				return;
			}
			if (imp_.registro == "")
			{
				m_reg_picker.Focus();
				return;
			}

			if (Device.RuntimePlatform == Device.Android)
			{
				if (imp_.printer != "")
				{
					bool test = true;
					if (imp_.printer.Length != 17) test = false;
					else if (imp_.printer.Substring(2, 1) != ":") test = false;
					else if (imp_.printer.Substring(5, 1) != ":") test = false;
					else if (imp_.printer.Substring(8, 1) != ":") test = false;
					else if (imp_.printer.Substring(11, 1) != ":") test = false;
					else if (imp_.printer.Substring(14, 1) != ":") test = false;
					if (!test)
					{
						await DisplayAlert("Attenzione!", "Indirizzo stampante non valido!", "OK");
						m_printer.Focus();
						return;
					}
				}
			}
			if (Device.RuntimePlatform == Device.iOS)
			{
				if (imp_.printer != "" && imp_.printer.Length != 14)
				{
					await DisplayAlert("Attenzione!", "Numero di serie stampante non valido!", "OK");
					m_printer.Focus();
					return;
				}
			}
			await dbcon_.UpdateAsync(imp_);
			await Navigation.PopAsync();
		}


		async void OnClickedSearch(object sender, System.EventArgs e)
		{

			var page = new SetupPrinter();
			page.PrnList.ItemDoubleTapped += (source, args) =>
			{
				var prn = (IDiscoveredPrinter)args.ItemData;
				imp_.printer = prn.Address;
				SetField();
				Navigation.PopAsync();
			};
			await Navigation.PushAsync(page);


		}

    }
}
