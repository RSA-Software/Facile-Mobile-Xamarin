using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Facile
{
	public partial class WebPage : ContentPage
	{
		public WebPage()
		{
			InitializeComponent();
			busyIndicator.IsBusy = true;
		}

		void Handle_Navigated(object sender, Xamarin.Forms.WebNavigatedEventArgs e)
		{
			busyIndicator.IsBusy = false;
		}
	}
}
