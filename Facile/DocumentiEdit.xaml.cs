using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public Fatture doc;
		public bool nuova;
		private readonly SQLiteAsyncConnection dbcon_;
		private ContentPage headerPage_;
		private NavigationPage bodyPage_;
		private NavigationPage footerPage_;
		private DocumentiBody body_;

		private int last_num_;

		public DocumentiEdit(ref Fatture f, ref bool nuova)
		{
			this.doc = f;
			this.nuova = nuova;
			last_num_ = 0;
			dbcon_ = DependencyService.Get<ISQLiteDb>().GetConnection();

			InitializeComponent();
			switch (this.doc.fat_tipo)
			{
				case (int)DocTipo.TIPO_DDT:
					Title = "Documento di Trasporto";
					break;

				case (int)DocTipo.TIPO_FAT:
					Title = "Fattura";
					break;

				case (int)DocTipo.TIPO_PRE:
					Title = "Preventivo";
					break;

				case (int)DocTipo.TIPO_ORD:
					Title = "Ordine";
					break;

				default:
					Title = "*** Documento Sconosciuto ***";
					break;
			}
			ChildAdded += OnChildAdded;


			headerPage_ = new DocumentiHeader(this);
			headerPage_.Title = "Testata";
			headerPage_.Icon = "ic_perm_identity_white.png";

			body_ = new DocumentiBody(this);
			bodyPage_ = new NavigationPage(body_);
			bodyPage_.Title = "Corpo";
			bodyPage_.Icon = "ic_view_headline_white.png";

			footerPage_ = new NavigationPage(new DocumentiFooter(ref doc));
			footerPage_.Title = "Piede";
			footerPage_.Icon = "ic_euro_symbol_white.png";

			Children.Add(headerPage_);
			Children.Add(bodyPage_);
			Children.Add(footerPage_);
		}

		void OnChildAdded (object sender, ElementEventArgs e)
		{
			e.Element.Parent = this;
		}

		async void OnCurrentPageChanged(object sender, System.EventArgs e)
		{
			if (nuova && (CurrentPage == bodyPage_ || CurrentPage == footerPage_))
			{
				Device.BeginInvokeOnMainThread(() => {
					CurrentPage = Children[0];
				});
			}
			if (!nuova && CurrentPage == bodyPage_)
			{
				if (last_num_ == 0 || last_num_ != doc.fat_n_doc)
				{
					last_num_ = doc.fat_n_doc;
					await body_.SetItemSource();
				}
			}
		}
	}
}
