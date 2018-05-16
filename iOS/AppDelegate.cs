using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Syncfusion.SfNumericTextBox.XForms.iOS;
using Syncfusion.SfDataGrid.XForms.iOS;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.XForms.iOS.MaskedEdit;
using Syncfusion.SfCarousel.XForms.iOS;

namespace Facile.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("it-IT");
            global::Xamarin.Forms.Forms.Init();

			new SfNumericTextBoxRenderer();

			SfDataGridRenderer.Init();
			SfListViewRenderer.Init();
			SfMaskedEditRenderer.Init();


			new SfBusyIndicatorRenderer();
			new SfCarouselRenderer();

			ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
    }
}
