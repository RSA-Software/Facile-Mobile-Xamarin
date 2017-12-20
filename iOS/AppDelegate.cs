using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Syncfusion.SfDataGrid.XForms.iOS;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.SfNumericTextBox.XForms.iOS;

namespace Facile.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
			SfDataGridRenderer.Init();
			SfListViewRenderer.Init();
			new SfNumericTextBoxRenderer();

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
    }
}
