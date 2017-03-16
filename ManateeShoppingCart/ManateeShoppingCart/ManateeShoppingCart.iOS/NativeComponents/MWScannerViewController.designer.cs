// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ManateeShoppingCart.iOS.MWBarcodeScanner
{
    [Register ("MWScannerViewController")]
    partial class MWScannerViewController
    {
        [Outlet]
        UIKit.UIImageView cameraOverlay { get; set; }


        [Outlet]
        UIKit.UIButton closeButton { get; set; }


        [Outlet]
        public UIKit.UIButton flashButton { get; set; }


        [Outlet]
        public UIKit.UIButton zoomButton { get; set; }


        [Action ("doClose:")]
        partial void doClose (Foundation.NSObject sender);


        [Action ("doFlashToggle:")]
        partial void doFlashToggle (Foundation.NSObject sender);


        [Action ("doZoomToggle:")]
        partial void doZoomToggle (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
            if (cameraOverlay != null) {
                cameraOverlay.Dispose ();
                cameraOverlay = null;
            }

            if (closeButton != null) {
                closeButton.Dispose ();
                closeButton = null;
            }

            if (flashButton != null) {
                flashButton.Dispose ();
                flashButton = null;
            }

            if (zoomButton != null) {
                zoomButton.Dispose ();
                zoomButton = null;
            }
        }
    }
}