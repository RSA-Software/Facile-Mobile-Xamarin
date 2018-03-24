using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facile.Interfaces;
using Facile.Models;
using SQLite;
using Xamarin.Forms;
using static Facile.Extension.FattureExtensions;

namespace Facile
{
	public partial class DocumentiEdit : TabbedPage
	{
		protected Fatture doc_;
		protected bool nuova_;
		protected bool editable_;
		private readonly SQLiteAsyncConnection dbcon_;
		private ContentPage headerPage_;
		private NavigationPage bodyPage_;
		private NavigationPage footerPage_;

		public DocumentiEdit(ref Fatture f, ref bool nuova, ref bool editable)
		{
			doc_ = f;
			nuova_ = nuova;
			editable_ = editable;
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

			headerPage_ = new DocumentiHeader(ref doc_, ref nuova_, ref editable_);
			headerPage_.Title = "Testata";
			headerPage_.Icon = "ic_perm_identity_white.png";

			bodyPage_ = new NavigationPage(new DocumentiBody(ref doc_));
			bodyPage_.Title = "Corpo";
			bodyPage_.Icon = "ic_view_headline_white.png";

			footerPage_ = new NavigationPage(new DocumentiFooter(ref doc_));
			footerPage_.Title = "Piede";
			footerPage_.Icon = "ic_euro_symbol_white.png";

			Children.Add(headerPage_);
			Children.Add(bodyPage_);
			Children.Add(footerPage_);

		}

void OnCurrentPageChanged(object sender, System.EventArgs e)
		{
			if (nuova_ && (CurrentPage == bodyPage_ || CurrentPage == footerPage_))
			{
				Device.BeginInvokeOnMainThread(() => {
					CurrentPage = Children[0];
				});
			}
		}
	}
}
