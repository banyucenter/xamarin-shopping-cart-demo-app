using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ManateeShoppingCart.Droid.MWBarcodeScanner
{
	public class ScannerResult
	{
		public string code { get; set; }
		public string type { get; set; }
		public byte[] bytes { get; set; }
		public bool isGS1 { get; set; }
	}
	public interface IScanSuccessCallback{
		void barcodeDetected(MWResult result);
	}
	
	public interface IScanner
	{
		
		Task<ScannerResult>  Scan();
		bool ScanWithCallback(IScanSuccessCallback callback);
		bool ScanInView (Android.Content.Context context,RectangleF scanningRect);

		void setInterfaceOrientation (String orientation);
		void initDecoder ();
		void resumeScanning();
		void closeScanner();
		void flashOnByDefault(bool flashOn);
			
		bool useFlash { get; set; }
		bool enableShowLocation { get; set; }
		bool useFrontCamera { get; set; }


		void setZoomLevels (int zoomLevel1, int zoomLevel2, int initialZoomLevel);
		void setMaxThreads (int maxThreads);
		bool useZoom{get; set;}
		bool closeScannerOnDecode { get; set; }
		bool useHiRes { get; set; }
		ScannerActivity.OverlayMode overlayMode { get; set; }
		
		
		
	}
	
	
	
	public abstract class ScannerBase : Java.Lang.Object, IScanner
	{
		
		public static int OM_NONE = 0;
		public static int OM_MW = 1;
		public static int OM_IMAGE = 2;
		
		
		public abstract Task<ScannerResult> Scan();
		
		public abstract bool ScanWithCallback(IScanSuccessCallback callback);
		public abstract void setInterfaceOrientation (String orientation);
		public abstract bool ScanInView (Android.Content.Context context, RectangleF scanningRect);

		//			public abstract ScannerResult scanGrayscaleImage (byte[] gray, int width, int height);
		public abstract void initDecoder ();
		public abstract void resumeScanning ();
		public abstract void closeScanner();
		public abstract void flashOnByDefault(bool flashOn);
		public abstract void setZoomLevels (int zoomLevel1,int zoomLevel2, int initialZoomLevel);
		public abstract void setMaxThreads (int maxThreads);
		public abstract bool useZoom { get; set; }
		public abstract bool useFlash { get; set; }
		public abstract bool useFrontCamera { get; set; }

		public abstract bool useHiRes { get; set; }
		public abstract bool enableShowLocation { get; set; }
		public abstract bool closeScannerOnDecode { get; set; }
		public abstract ScannerActivity.OverlayMode overlayMode { get; set; }
	}
	
	
}
