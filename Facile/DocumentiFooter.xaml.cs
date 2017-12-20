using System;
using System.Collections.Generic;
using Facile.Models;
using Xamarin.Forms;

namespace Facile
{
	public partial class DocumentiFooter : ContentPage
	{
		public Fatture fat { get; set; }

		public DocumentiFooter(ref Fatture f)
		{
			//fat = f;
			InitializeComponent();
		}
	}
}
