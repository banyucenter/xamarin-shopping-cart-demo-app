using System;
using System.Threading.Tasks;
using CoreGraphics;

namespace ManateeShoppingCart.iOS.MWBarcodeScanner
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
		
		
		bool ScanWithCallback(IScanSuccessCallback callback);
		void ScanInView (IScanSuccessCallback callback, CGRect scanningRect);
		Task<ScannerResult>  Scan();
		
		void resumeScanning();
		void closeScanner();
		bool closeScannerOnDecode { get; set; }
		bool use60fps { get; set; }
		
		void setInterfaceOrientation (String orientation);
		void setZoomLevels (int zoomLevel1, int zoomLevel2, int initialZoomLevel);
		void setMaxThreads (int maxThreads);
		void initDecoder ();
		bool useFlash { get; set; }
		bool useFrontCamera { get; set; }
		bool useZoom{get; set;}
		bool enableShowLocation{get; set;}
		
		bool useHiRes { get; set; }
		int overlayMode { get; set; }
		
	}
	
	public abstract class ScannerBase : IScanner
	{
		
		public static int OM_NONE = 0;
		public static int OM_MW = 1;
		public static int OM_IMAGE = 2;
		public static int OM_VIEW = 3;
		public abstract Task<ScannerResult> Scan();
		public abstract bool ScanWithCallback(IScanSuccessCallback callback);
		public abstract void ScanInView (IScanSuccessCallback callback, CGRect scanningRect);
		
		public abstract bool closeScannerOnDecode { get; set; }
		public abstract bool use60fps { get; set; }
		
		public abstract void resumeScanning ();
		public abstract void closeScanner();
		public abstract void setInterfaceOrientation (String orientation);
		public abstract void setZoomLevels (int zoomLevel1,int zoomLevel2, int initialZoomLevel);
		public abstract void setMaxThreads (int maxThreads);
		public abstract void setFlashButtonFrame (CGRect frame);
		public abstract void setZoomButtonFrame (CGRect frame);
		public abstract void setCloseButtonFrame (CGRect frame);
		
		public abstract void initDecoder ();
		public abstract bool useFlash { get; set; }
		public abstract bool useZoom { get; set; }
		public abstract bool enableShowLocation { get; set; }
		public abstract bool useFrontCamera { get; set; }

		public abstract bool useHiRes { get; set; }
		public abstract int overlayMode { get; set; }
		
	}
	

}

