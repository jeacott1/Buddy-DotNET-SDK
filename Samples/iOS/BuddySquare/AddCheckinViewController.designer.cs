// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BuddySquare.iOS
{
	[Register ("AddCheckinViewController")]
	partial class AddCheckinViewController
	{
		[Outlet]
		UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MapKit.MKMapView mapView { get; set; }

		[Outlet]
		UIKit.UITableView tableLocations { get; set; }

		[Outlet]
		UIKit.UITextView txtComment { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mapView != null) {
				mapView.Dispose ();
				mapView = null;
			}

			if (tableLocations != null) {
				tableLocations.Dispose ();
				tableLocations = null;
			}

			if (txtComment != null) {
				txtComment.Dispose ();
				txtComment = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}
		}
	}
}
