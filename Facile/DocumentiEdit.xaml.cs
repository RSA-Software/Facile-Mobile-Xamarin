using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiEdit : TabbedPage
	{
		//protected readonly TipoDocumento tipoDoc_;
		//protected int numDoc_;
		//protected readonly string registro_;
		protected Fatture doc_;
		private readonly SQLiteAsyncConnection dbcon_;

		private NavigationPage headerPage_;
		private NavigationPage bodyPage_;
		private NavigationPage footerPage_;


		public DocumentiEdit(ref Fatture f)
		{
			//tipoDoc_ = t_doc;
			//registro_ = reg;
			//numDoc_ = n_doc;
			doc_ = f;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();
			switch (doc_.fat_tipo)
			{
				case (int)TipoDocumento.TIPO_DDT:
					Title = "Documento di Trasporto";
					break;

				case (int)TipoDocumento.TIPO_FAT:
					Title = "Fattura";
					break;

				case (int)TipoDocumento.TIPO_PRE:
					Title = "Preventivo";
					break;

				case (int)TipoDocumento.TIPO_ORD:
					Title = "Ordine";
					break;

				default:
					Title = "*** Documento Sconosciuto ***";
					break;
			}

			headerPage_ = new NavigationPage(new DocumentiHeader(ref doc_));
			headerPage_.Title = "Testata";
			headerPage_.Icon = "ic_alarm.png";

			bodyPage_ = new NavigationPage(new DocumentiBody(ref doc_));
			bodyPage_.Title = "Corpo";
			bodyPage_.Icon = "ic_cached.png";

			footerPage_ = new NavigationPage(new DocumentiFooter(ref doc_));
			footerPage_.Title = "Piede";
			footerPage_.Icon = "ic_group.png";

			Children.Add(headerPage_);
			Children.Add(bodyPage_);
			Children.Add(footerPage_);
		}

		//protected async override void OnAppearing()
		//{
		//	if (numDoc_ == 0)
		//	{
		//		await Blank();
		//	}
		//	else
		//	{
		//		try
		//		{
		//			fat = await dbcon_.GetAsync<Fatture>(numDoc_);
		//			Children.Add(bodyPage_);
		//			Children.Add(footerPage_);
		//		}
		//		catch (SQLiteException ex)
		//		{
		//			await DisplayAlert("Attenzione!", ex.Message, "OK");
		//			await Navigation.PopAsync();
		//		}
		//		catch (Exception ex)
		//		{
		//			await DisplayAlert("Attenzione!", ex.Message, "OK");
		//			await Navigation.PopAsync();
		//		}
		//	}
		//	//foreach(var child in Children)
		//	//{
		//	//}
		//	base.OnAppearing();
		//}

		//public async Task Blank()
		//{
		//	string sql = String.Format("SELECT * FROM fatture2 WHERE fat_registro = '{0}' ORDER BY fat_n_doc DESC LIMIT 1", registro_);

		//	numDoc_ = 1;
		//	var docList = await dbcon_.QueryAsync<Fatture>(sql);
		//	foreach (var doc in docList)
		//	{
		//		numDoc_ += doc.fat_n_doc;
		//		break;
		//	}
		//	fat = new Fatture();
		//	fat.fat_tipo = (int)tipoDoc_;
		//	fat.fat_registro = registro_;		

		//	// Completare l'inizializzazione
		//}
	}
}
