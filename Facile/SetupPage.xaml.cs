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

        public SetupPage( )
        {
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

			SetField();
        }

		protected override async  void OnAppearing()
		{
			imp_ = await dbcon_.GetAsync<LocalImpo>(1);
			SetField();
			base.OnAppearing();
		}

		void GetField()
		{
			imp_.ftpServer = m_server_picker.SelectedItem != null ? m_server_picker.SelectedItem.ToString().Trim() : "";
			imp_.user = m_user.Text != null ? m_user.Text.Trim() : "";
			imp_.registro = m_reg_picker.SelectedItem != null ? m_reg_picker.SelectedItem.ToString().Trim() : "";
			imp_.age = Int32.Parse(m_age.Value.ToString());
		}

		void SetField()
		{
			if (imp_.ftpServer == "")
				m_server_picker.SelectedItem = null;
			else
				m_server_picker.SelectedItem = imp_.ftpServer;
			m_user.Text = imp_.user;
			m_age.Value = imp_.age;
			if (imp_.registro == "")
				m_reg_picker.SelectedItem = null;
			else
				m_reg_picker.SelectedItem = imp_.registro;
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
			if (imp_.registro == "")
			{
				m_reg_picker.Focus();
				return;
			}
			await dbcon_.UpdateAsync(imp_);
			await Navigation.PopAsync();
		}
    }
}
