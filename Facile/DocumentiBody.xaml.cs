using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public DocumentiBody(ref Fatture f)
		{
			doc_ = f;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();
			InitializeComponent();
			SetField();
			dataGrid.ColumnSizer = Syncfusion.SfDataGrid.XForms.ColumnSizer.LastColumnFill;
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
			rigCollection = new ObservableCollection<FatRow>(rigList);
			dataGrid.ItemsSource = rigCollection;

			base.OnAppearing();
		}

	}
}
