using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;

using UIKit;
using Foundation;
using AVFoundation;
using System.Threading.Tasks;
using CoreLocation;

using System.Threading;
using System.Threading.Tasks;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using ObjCRuntime;

using System.Runtime.InteropServices;


namespace ManateeShoppingCart.iOS.MWBarcodeScanner
{

	public enum CameraState : int {
		NORMAL,
		LAUNCHING_CAMERA,
		CAMERA,
		CAMERA_DECODING,
		DECODE_DISPLAY,
		CANCELLING
	}




	public partial class MWScannerViewController : UIViewController
	{


		public bool stopped = true;

		public AVCaptureDevice device;
		public AVCaptureSession captureSession;
		public AVCaptureVideoPreviewLayer prevLayer;

		public SampleBufferDelegate sampleBufferDelegate;

		bool statusBarHidden;
		NSTimer focusTimer;
		public static CameraState state = CameraState.NORMAL;

		public static int parser_mask = BarcodeConfig.MWP_PARSER_MASK_NONE;

		public static UIInterfaceOrientationMask param_Orientation = UIInterfaceOrientationMask.LandscapeLeft;
		public static bool param_EnableHiRes = true;
		public static bool param_EnableFlash = true;
		public static bool param_Use60FPS = false;

		public static bool param_UseFrontCamera = false;

		public static int param_OverlayMode = Scanner.OM_MW;
		public static bool param_CloseScannerOnSuccess = true;
		public static bool param_EnableLocation = true;


		public static IScanSuccessCallback successCallback;

		public static bool param_EnableZoom = true;

		public static int param_ZoomLevel1 = 0;
		public static int param_ZoomLevel2 = 0;
		public static int zoomLevel = 0;

		public static int param_maxThreads = 2;
		public static int activeThreads = 0;
		public static int availableThreads = 0;
		public static bool videoZoomSupported;
		public float firstZoom;

		public float secondZoom;
		public event Action<ScannerResult> OnResult;

		public static CGRect zoomFrame;
		public static CGRect flashFrame;
		public static CGRect closeFrame = new CGRect (0,0,64,64);

		public static Scanner scannerInstance;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public MWScannerViewController ()
			: base ("MWScannerViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		

		}

		public void closeScanner(){
			base.DismissViewController (true, null);

		}





//		public static int totalFrames = 0;






		public override void ViewWillAppear (bool animated)
		{
			
			base.ViewWillAppear (animated);
			//Console.WriteLine("Will appear");

			statusBarHidden =  UIApplication.SharedApplication.StatusBarHidden;
			UIApplication.SharedApplication.SetStatusBarHidden(true, false);

			flashButton.Hidden = !param_EnableFlash;

			cameraOverlay.Hidden =  param_OverlayMode != ScannerBase.OM_IMAGE;


			if (initCapture ()) {
				startScanning ();
				updateTorch ();
			} else {
				this.device = null;
				string appName = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleName").ToString();
				new UIAlertView("Camera Unavailable", String.Format("The {0} has not been given a permission to your camera. Please check the Privacy Settings: Settings -> {0} -> Privacy -> Camera", appName, appName), null, "Close", null).Show();
			}

	

		}




		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (device != null) {
				if (param_OverlayMode == Scanner.OM_MW)
				{
					MWOverlay.addToPreviewLayer(this.prevLayer);
				}
				else if (param_OverlayMode == Scanner.OM_VIEW)
				{
					MWOverlay.addViewToPreviewLayer(this.View);
				}
			}
			closeButton.Frame = closeFrame;

			if (flashFrame.Width == 0) {
				flashButton.Frame = new CGRect(this.View.Frame.Size.Width-64,0,64,64);
			} else {
				flashButton.Frame = flashFrame;
			}

			if (zoomFrame.Width == 0) {
				zoomButton.Frame = new CGRect(this.View.Frame.Size.Width-64,this.View.Frame.Size.Height-64,64,64);
			}else{
				zoomButton.Frame = zoomFrame;
			}

		
		}

		public override void ViewWillDisappear (bool animated){
			base.ViewWillDisappear (animated);
			//Console.WriteLine("Will disappear");
			if (device != null) {
				stopScanning ();
				deinitCapture ();
				flashButton.Selected = false;
			}
		}


		public override bool ShouldAutorotate()
		{
			bool should = ((int)param_Orientation & (1 << (int)this.InterfaceOrientation)) != 0;
			return should;
		}
		bool rotating = false;

		public void rotationHandler(object source, System.Timers.ElapsedEventArgs e){
			aTimer.Stop ();

				InvokeOnMainThread(() => {
				if (!rotating && prevLayer != null) {
						rotating = true;
						MWOverlay.removeFromPreviewLayer();
						
						CGRect screenRect = UIScreen.MainScreen.Bounds;
						float screenWidth = (float)screenRect.Size.Width;
						float screenHeight = (float)screenRect.Size.Height;
						
						UIDeviceOrientation toOrientation = UIDevice.CurrentDevice.Orientation;
						
						if (toOrientation == UIDeviceOrientation.LandscapeRight){
							prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
							prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth, screenHeight));
						}
						if (toOrientation == UIDeviceOrientation.LandscapeLeft){
							prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
							prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth,screenHeight));
						}
						
						if (toOrientation == UIDeviceOrientation.Portrait) {
							prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
							prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
						}
						if (toOrientation == UIDeviceOrientation.PortraitUpsideDown) {
							prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
							prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
						}
						prevLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
						
						
						aTimer = new System.Timers.Timer();
						aTimer.Elapsed+= new System.Timers.ElapsedEventHandler(addLayerHandler);
						aTimer.Interval=50;
						aTimer.Enabled=true;
						rotating = false;
					}
				});

		}

		public void addLayerHandler(object source, System.Timers.ElapsedEventArgs e){
			aTimer.Stop ();

			if (device != null) {
				InvokeOnMainThread (() => {
				if (param_OverlayMode == Scanner.OM_MW) {
						if(prevLayer!= null){
							MWOverlay.addToPreviewLayer (this.prevLayer);
						}
						rotating = false;
					}else if (param_OverlayMode == Scanner.OM_VIEW)
					{
						MWOverlay.addViewToPreviewLayer(this.View);
					}
				});

			}
		} 
		System.Timers.Timer aTimer;


		public override void ViewWillTransitionToSize (CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
		{
			base.ViewWillTransitionToSize (toSize, coordinator);
			MWOverlay.removeFromPreviewLayer ();
			aTimer = new System.Timers.Timer();
			aTimer.Elapsed+= new System.Timers.ElapsedEventHandler(rotationHandler);
			aTimer.Interval=50;
			aTimer.Enabled=true;

		}

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)  
		{
			Console.WriteLine ("WillRotate");
			MWOverlay.removeFromPreviewLayer ();

			CGRect screenRect = UIScreen.MainScreen.Bounds;
			float screenWidth = (float)screenRect.Size.Width;
			float screenHeight = (float)screenRect.Size.Height;

			if (this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft){
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth, screenHeight));
			}
			if (this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight){
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth,screenHeight));
			}


			if (this.InterfaceOrientation == UIInterfaceOrientation.Portrait) {
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
			}
			if (this.InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
			}
			this.prevLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
			if (param_OverlayMode == Scanner.OM_MW)
			{
				MWOverlay.addToPreviewLayer(this.prevLayer);
			}else if (param_OverlayMode == Scanner.OM_VIEW)
			{
				MWOverlay.addViewToPreviewLayer(this.View);
			}
		}


		public AVCaptureVideoPreviewLayer generateLayerWithRect(CGPoint bottomRightPoint){

			/*We setup the input*/
			if (param_UseFrontCamera)
			{
				AVCaptureDevice[] devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				foreach (AVCaptureDevice tmpDevice in devices)
				{
					if (tmpDevice.Position == AVCaptureDevicePosition.Front)
					{
						this.device = tmpDevice;
						break;
					}
				}
				
			}
			else {
				this.device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			}			

			BarcodeConfig.MWB_setResultType (BarcodeConfig.MWB_RESULT_TYPE_MW);
			if (this.device == null) {
				return null;
			}

			AVCaptureDeviceInput captureInput = AVCaptureDeviceInput.FromDevice (this.device);


			if (captureInput == null){
				return null;
			}

			/*We setupt the output*/
			AVCaptureVideoDataOutput captureOutput = new AVCaptureVideoDataOutput () {
				//videoSettings
				//vidrec.WeakVideoSettings = new AVVideoSettings (CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange)
			};


			captureOutput.AlwaysDiscardsLateVideoFrames = true;
			//captureOutput.minFrameDuration = CMTimeMake(1, 10); Uncomment it to specify a minimum duration for each video frame

			DispatchQueue queue = new CoreFoundation.DispatchQueue("MWBQueue");

			sampleBufferDelegate = new SampleBufferDelegate ();
			sampleBufferDelegate.controllerInstance = this; 

			captureOutput.SetSampleBufferDelegate(sampleBufferDelegate, queue);
			// Set the video output to store frame in BGRA (It is supposed to be faster)

			//And we create a capture session
			captureSession = new AVCaptureSession ();

			float resX = 640;
			float resY = 480;
			if (param_EnableHiRes && captureSession.CanSetSessionPreset(AVCaptureSession.Preset640x480)){
				captureSession.SessionPreset = AVCaptureSession.Preset1280x720;
				resX = 1280;
				resY = 720;
			} else {
				captureSession.SessionPreset = AVCaptureSession.Preset640x480;
			}




			if (availableThreads == 0){
				availableThreads = (int)NSProcessInfo.ProcessInfo.ProcessorCount;
			}

			if (param_maxThreads > availableThreads){
				param_maxThreads = availableThreads;
			}



			captureSession.AddInput (captureInput);
			captureSession.AddOutput (captureOutput);

			if ((int)NSProcessInfo.ProcessInfo.ProcessorCount < 2) {

				if (Convert.ToInt16 (UIDevice.CurrentDevice.SystemVersion.Split ('.') [0]) >= 7) {
					Foundation.NSError err = new NSError ();
					this.device.LockForConfiguration (out err);

					this.device.ActiveVideoMinFrameDuration = new CMTime (1, 15);
					this.device.UnlockForConfiguration ();


				} else {

					AVCaptureConnection conn = captureOutput.ConnectionFromMediaType (AVMediaType.Video);
					conn.VideoMinFrameDuration = new CMTime (1, 15);
				}
			} else if(param_Use60FPS){

				foreach(AVCaptureDeviceFormat vFormat in this.device.Formats)
				{
					CMVideoFormatDescription description= (CMVideoFormatDescription)vFormat.FormatDescription;

					double maxrate = ((AVFrameRateRange)(vFormat.VideoSupportedFrameRateRanges[0])).MaxFrameRate;
					double minrate = ((AVFrameRateRange)(vFormat.VideoSupportedFrameRateRanges[0])).MinFrameRate;
					if (maxrate > 59 && description.MediaSubType == (uint)CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange && description.Dimensions.Width == resX && description.Dimensions.Height == resY) {
						NSError error;
						if (this.device.LockForConfiguration (out error)) {
							this.device.ActiveFormat = vFormat;
							this.device.ActiveVideoMinFrameDuration = new CMTime ((long)10,(int) minrate * 10);
							this.device.ActiveVideoMaxFrameDuration = new CMTime ((long)10.0, (int) 600);

							this.device.UnlockForConfiguration ();
						}
					}

				}
			}

			/*We add the preview layer*/

			AVCaptureVideoPreviewLayer theLayer = new AVCaptureVideoPreviewLayer(captureSession);



			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft){
				theLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
			}
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight){
				theLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
			}


			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait) {
				theLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
			}
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				theLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
			}

			theLayer.Frame = new CGRect (0, 0, bottomRightPoint.X, bottomRightPoint.Y);
			theLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;





			videoZoomSupported = false;

			if (this.device.RespondsToSelector(new Selector("setActiveFormat:"))&&this.device.ActiveFormat.RespondsToSelector(new Selector("videoMaxZoomFactor"))&&this.device.RespondsToSelector(new Selector("setVideoZoomFactor:"))) {
				float maxZoom = 0;
				if (this.device.ActiveFormat.RespondsToSelector (new Selector ("videoZoomFactorUpscaleThreshold"))) {

					maxZoom = (float)this.device.ActiveFormat.VideoZoomFactorUpscaleThreshold;
				} else {
					maxZoom = (float) this.device.ActiveFormat.VideoMaxZoomFactor;

				}




				float maxZoomTotal = (float)this.device.ActiveFormat.VideoMaxZoomFactor;
				if (maxZoomTotal > 1.1&&this.device.RespondsToSelector(new Selector("setVideoZoomFactor:")) ) {
					videoZoomSupported = true;
					if (param_ZoomLevel1 != 0 && param_ZoomLevel2 != 0) {

						if (param_ZoomLevel1 > maxZoomTotal * 100) {
							param_ZoomLevel1 = (int)(maxZoomTotal * 100);
						}
						if (param_ZoomLevel2 > maxZoomTotal * 100) {
							param_ZoomLevel2 = (int)(maxZoomTotal * 100);
						}

						firstZoom = (float)0.01 * param_ZoomLevel1;
						secondZoom =(float) 0.01 * param_ZoomLevel2;
					} else {
						if (maxZoomTotal > 2){

							if (maxZoom > 1.0 && maxZoom <= 2.0){
								firstZoom = maxZoom;
								secondZoom = maxZoom * 2;
							} else
								if (maxZoom > 2.0){
									firstZoom = (float)2.0;
									secondZoom = (float)4.0;
								}

						}
					}


				}

			}



			activeThreads = 0;
			return theLayer;

		}



		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return param_Orientation;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return ((int)param_Orientation & (1 << (int)toInterfaceOrientation)) != 0;
		}



		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		public static void setInterfaceOrientation (UIInterfaceOrientationMask interfaceOrientation) {

			param_Orientation = interfaceOrientation;

		}

		public static void enableHiRes (bool hiRes) {

			param_EnableHiRes = hiRes;

		}

		public static void setActiveParserMask(int parserMask){
			parser_mask = parserMask;
		}

		public static void enableFlash (bool flash) {
			param_EnableFlash = flash;
		}


		public static void setZoomLevels(int zoomLevel1,int zoomLevel2, int initialZoomLevel){
			

				param_ZoomLevel1 = zoomLevel1;
				param_ZoomLevel2 = zoomLevel2;
				zoomLevel = initialZoomLevel;
				if (zoomLevel > 2){
					zoomLevel = 2;
				}
				if (zoomLevel < 0){
					zoomLevel = 0;
				}


		}

		public void  updateDigitalZoom() {

			if (videoZoomSupported){
				NSError error;
				this.device.LockForConfiguration(out error);


				switch (zoomLevel) {
				case 0:
					this.device.VideoZoomFactor = 1; /*rampToVideoZoomFactor:1 withRate:4*/
					break;
				case 1:
					this.device.VideoZoomFactor = firstZoom;
					break;
				case 2:
					this.device.VideoZoomFactor = secondZoom;
					break;

				default:
					break;
				}
				this.device.UnlockForConfiguration();

				zoomButton.Hidden = !param_EnableZoom;
			} else {
				zoomButton.Hidden = true;
			}
		}
		public static void setOverlayMode (int overlayMode) {

			param_OverlayMode = overlayMode;

		}

		public static void setMaxThreads(int maxThreads)
		{
			if (availableThreads == 0){
				availableThreads = (int)NSProcessInfo.ProcessInfo.ProcessorCount;
			}


			param_maxThreads = maxThreads;
			if (param_maxThreads > availableThreads){
				param_maxThreads = availableThreads;
			}

		}


		public void deinitCapture() {
			if (this.captureSession != null){
				if (param_OverlayMode == Scanner.OM_MW){
					MWOverlay.removeFromPreviewLayer();
				}

				this.prevLayer.RemoveFromSuperLayer();
				this.prevLayer = null;

			}
		}

		public bool initCapture()
		{
			/*We setup the input*/
			if (param_UseFrontCamera) {
				AVCaptureDevice[] devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
				foreach (AVCaptureDevice tmpDevice in devices) {
					if (tmpDevice.Position == AVCaptureDevicePosition.Front) {
						this.device = tmpDevice;
						break;
					}
				}

			} else {
				this.device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			}

			BarcodeConfig.MWB_setResultType (BarcodeConfig.MWB_RESULT_TYPE_MW);
			if (this.device == null) {
				return false;
			}

			AVCaptureDeviceInput captureInput = AVCaptureDeviceInput.FromDevice(this.device);


			if (captureInput == null){

				return false;
			}

			/*We setupt the output*/
			AVCaptureVideoDataOutput captureOutput = new AVCaptureVideoDataOutput () {
				//videoSettings
				//vidrec.WeakVideoSettings = new AVVideoSettings (CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange)
			};

				
			captureOutput.AlwaysDiscardsLateVideoFrames = true;
			//captureOutput.minFrameDuration = CMTimeMake(1, 10); Uncomment it to specify a minimum duration for each video frame

			DispatchQueue queue = new CoreFoundation.DispatchQueue("MWBQueue");

			sampleBufferDelegate = new SampleBufferDelegate ();
			sampleBufferDelegate.controllerInstance = this; 

			captureOutput.SetSampleBufferDelegate(sampleBufferDelegate, queue);
			// Set the video output to store frame in BGRA (It is supposed to be faster)

			//And we create a capture session
			captureSession = new AVCaptureSession ();

			float resX = 640;
			float resY = 480;
			if (param_EnableHiRes && captureSession.CanSetSessionPreset(AVCaptureSession.Preset640x480)){
				captureSession.SessionPreset = AVCaptureSession.Preset1280x720;
				resX = 1280;
				resY = 720;
			} else {
				captureSession.SessionPreset = AVCaptureSession.Preset640x480;
			}




			if (availableThreads == 0){
				availableThreads = (int)NSProcessInfo.ProcessInfo.ProcessorCount;
			}

			if (param_maxThreads > availableThreads){
				param_maxThreads = availableThreads;
			}
				


			captureSession.AddInput (captureInput);
			captureSession.AddOutput (captureOutput);

			if ((int)NSProcessInfo.ProcessInfo.ProcessorCount < 2) {

				if (Convert.ToInt16 (UIDevice.CurrentDevice.SystemVersion.Split ('.') [0]) >= 7) {
					Foundation.NSError err = new NSError ();
					this.device.LockForConfiguration (out err);

					this.device.ActiveVideoMinFrameDuration = new CMTime (1, 15);
					this.device.UnlockForConfiguration ();


				} else {

					AVCaptureConnection conn = captureOutput.ConnectionFromMediaType (AVMediaType.Video);
					conn.VideoMinFrameDuration = new CMTime (1, 15);
				}
			} else if(param_Use60FPS){

				foreach(AVCaptureDeviceFormat vFormat in this.device.Formats)
				{
					CMVideoFormatDescription description= (CMVideoFormatDescription)vFormat.FormatDescription;

					double maxrate = ((AVFrameRateRange)(vFormat.VideoSupportedFrameRateRanges[0])).MaxFrameRate;
					double minrate = ((AVFrameRateRange)(vFormat.VideoSupportedFrameRateRanges[0])).MinFrameRate;
					if (maxrate > 59 && description.MediaSubType == (uint)CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange && description.Dimensions.Width == resX && description.Dimensions.Height == resY) {
						NSError error;
						if (this.device.LockForConfiguration (out error)) {
							this.device.ActiveFormat = vFormat;
							this.device.ActiveVideoMinFrameDuration = new CMTime ((long)10,(int) minrate * 10);
							this.device.ActiveVideoMaxFrameDuration = new CMTime ((long)10.0, (int) 600);

							this.device.UnlockForConfiguration ();
						}
					}

				}
			}

			/*We add the preview layer*/

			prevLayer = new AVCaptureVideoPreviewLayer(captureSession);

			CGRect screenRect = UIScreen.MainScreen.Bounds;
			float screenWidth = (float)screenRect.Size.Width;
			float screenHeight = (float)screenRect.Size.Height;

		

			if (this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft){
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth, screenHeight));
			}
			if (this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight){
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Max(screenWidth,screenHeight), Math.Min(screenWidth,screenHeight));
			}


			if (this.InterfaceOrientation == UIInterfaceOrientation.Portrait) {
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
			}
			if (this.InterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown) {
				this.prevLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
				this.prevLayer.Frame = new RectangleF(0, 0, Math.Min(screenWidth,screenHeight), Math.Max(screenWidth,screenHeight));
			}

			this.prevLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;

			this.View.Layer.AddSublayer(prevLayer);




			videoZoomSupported = false;
			
			if (this.device.RespondsToSelector(new Selector("setActiveFormat:"))&&this.device.ActiveFormat.RespondsToSelector(new Selector("videoMaxZoomFactor"))&&this.device.RespondsToSelector(new Selector("setVideoZoomFactor:"))) {
				float maxZoom = 0;
				if (this.device.ActiveFormat.RespondsToSelector (new Selector ("videoZoomFactorUpscaleThreshold"))) {
				
					maxZoom = (float)this.device.ActiveFormat.VideoZoomFactorUpscaleThreshold;
				} else {
					maxZoom = (float) this.device.ActiveFormat.VideoMaxZoomFactor;

				}




				float maxZoomTotal = (float)this.device.ActiveFormat.VideoMaxZoomFactor;
				if (maxZoomTotal > 1.1&&this.device.RespondsToSelector(new Selector("setVideoZoomFactor:")) ) {
					videoZoomSupported = true;
					if (param_ZoomLevel1 != 0 && param_ZoomLevel2 != 0) {
						
						if (param_ZoomLevel1 > maxZoomTotal * 100) {
							param_ZoomLevel1 = (int)(maxZoomTotal * 100);
						}
						if (param_ZoomLevel2 > maxZoomTotal * 100) {
							param_ZoomLevel2 = (int)(maxZoomTotal * 100);
						}
						
						firstZoom = (float)0.01 * param_ZoomLevel1;
						secondZoom =(float) 0.01 * param_ZoomLevel2;
					} else {
						if (maxZoomTotal > 2){
							
							if (maxZoom > 1.0 && maxZoom <= 2.0){
								firstZoom = maxZoom;
								secondZoom = maxZoom * 2;
							} else
								if (maxZoom > 2.0){
									firstZoom = (float)2.0;
									secondZoom = (float)4.0;
								}
							
						}
					}
					
					
				}

			}


			if (!videoZoomSupported){
				zoomButton.Hidden = true;
			} else {
				updateDigitalZoom ();
			}



			if (!device.IsTorchModeSupported(AVCaptureTorchMode.On)) {
				flashButton.Hidden = true;
			}


			this.View.BringSubviewToFront (cameraOverlay);
			this.View.BringSubviewToFront (closeButton);
			this.View.BringSubviewToFront (flashButton);
			this.View.BringSubviewToFront (zoomButton);

			closeButton.TouchUpInside += delegate {
				if(successCallback!=null){
					successCallback.barcodeDetected(null);
					
				}else{
					handleResult(null);
				}
				DismissViewController(true,null);	
			};

			//			self.focusTimer = [NSTimer scheduledTimerWithTimeInterval:1.5 target:self selector:@selector(reFocus) userInfo:nil repeats:YES];

			//this.captureSession.StartRunning();
			//	this.prevLayer.Hidden = false;
			activeThreads = 0;

			return true;

		}


		public void startScanning() {
			state = CameraState.LAUNCHING_CAMERA;
			this.captureSession.StartRunning();
			this.prevLayer.Hidden = false;
			state = CameraState.CAMERA;

			this.focusTimer = NSTimer.CreateRepeatingScheduledTimer(1.5,  delegate{reFocus();} );

			stopped = false;

		}





		public void stopScanning() {

			if (stopped)
				return;

			//Console.WriteLine("Stopping...");

			if (this.focusTimer != null) {
				this.focusTimer.Invalidate ();
				this.focusTimer = null;
			}

			if (state == CameraState.CAMERA_DECODING) {
				state = CameraState.CANCELLING;
			}


			if (sampleBufferDelegate != null)
				sampleBufferDelegate.CancelTokenSource.Cancel();

			//Try removing all existing outputs prior to closing the session
			try
			{
				while (this.captureSession.Outputs.Length > 0)
					this.captureSession.RemoveOutput (this.captureSession.Outputs [0]);
			}
			catch { }

			//Try to remove all existing inputs prior to closing the session
			try
			{
				while (this.captureSession.Inputs.Length > 0)
					this.captureSession.RemoveInput (this.captureSession.Inputs [0]);
			}
			catch { }

			if (this.captureSession.Running)
				this.captureSession.StopRunning();

			stopped = true;

			state = CameraState.NORMAL;
		}


		partial void doClose (Foundation.NSObject sender){

		}

		partial void doFlashToggle (Foundation.NSObject sender){

			toggleTorch();

		}
		public void doZoomTooggleView(){
			zoomLevel++;
			if (zoomLevel > 2){
				zoomLevel = 0;
			}
				
			updateDigitalZoom();
		}

		partial void doZoomToggle (Foundation.NSObject sender){
			zoomLevel++;
			if (zoomLevel > 2){
				zoomLevel = 0;
			}
				
			updateDigitalZoom();
		}

		public void handleResult( ScannerResult result){
			if (this.OnResult != null)
				this.OnResult (result);

		}




		public void reFocus() {
			//Console.WriteLine("Refocusing");

			NSError error;
			if (this.device.LockForConfiguration(out error)) {

				if (this.device.FocusPointOfInterestSupported){
					this.device.FocusPointOfInterest = new PointF(0.49f,0.49f);
					this.device.FocusMode = AVCaptureFocusMode.AutoFocus;
				}
				this.device.UnlockForConfiguration();

			}
		}


		public void updateTorch() {

			if (param_EnableFlash && this.device.IsTorchModeSupported(AVCaptureTorchMode.On)) {

				flashButton.Hidden = false;

			} else {
				flashButton.Hidden = true;
			}

		}

		public void toggleTorch()
		{
			if (this.device.IsTorchModeSupported(AVCaptureTorchMode.On)) {
				NSError error = new NSError();

				if (this.device.LockForConfiguration(out error)) {
					if (this.device.TorchMode == AVCaptureTorchMode.On){
						this.device.TorchMode = AVCaptureTorchMode.Off;
						flashButton.Selected = false;
					}
					else {
						this.device.TorchMode = AVCaptureTorchMode.On;
						flashButton.Selected = true;
					}

					if(this.device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
						this.device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;

					this.device.UnlockForConfiguration();
				} else {
					

				}
			}
		}



	}

	public class SampleBufferDelegate : AVCaptureVideoDataOutputSampleBufferDelegate 
	{


	
		volatile bool working = false;

		public CancellationTokenSource CancelTokenSource = new CancellationTokenSource();
		public MWScannerViewController controllerInstance;

		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
//			MWScannerViewController.totalFrames++;

			if (MWScannerViewController.state != CameraState.CAMERA && MWScannerViewController.state != CameraState.CAMERA_DECODING) {
				try 
				{

					sampleBuffer.Dispose ();
					sampleBuffer = null;

				} catch (Exception e){
					Console.WriteLine (e);
				}
				return;
			}

			if (MWScannerViewController.activeThreads >= MWScannerViewController.param_maxThreads){
				try 
				{

					sampleBuffer.Dispose ();
					sampleBuffer = null;

				} catch (Exception e){
					Console.WriteLine (e);
				}
				return;
			}

			MWScannerViewController.state = CameraState.CAMERA_DECODING;

			MWScannerViewController.activeThreads++;



		
			//Console.WriteLine("SAMPLE");

			//lastAnalysis = DateTime.UtcNow;

			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer)
			{

				// Lock the base address
				pixelBuffer.Lock (CVPixelBufferLock.None);

				// Get the number of bytes per row for the pixel buffer
				var baseAddress = pixelBuffer.GetBaseAddress(0);
				int width = (int)pixelBuffer.Width;
				int height = (int)pixelBuffer.Height;
				int len = width*height;

				int pixelFormat = (int) pixelBuffer.PixelFormatType;


				byte[] frameBuffer = null;

				switch (pixelFormat) {
				case (int) CVPixelFormatType.CV420YpCbCr8BiPlanarVideoRange:
					frameBuffer = new byte[len];
					Marshal.Copy(baseAddress, frameBuffer, 0, len);
					break;
				case (int) CVPixelFormatType.CV422YpCbCr8:
					frameBuffer = new byte[len * 2];
					Marshal.Copy(baseAddress, frameBuffer, 0, len * 2);
					int dstpos=1;
					for (int i=0;i<len;i++){
						frameBuffer[i]=frameBuffer[dstpos];
						dstpos+=2;
					}

					break;
				default:

					break;
				}


				pixelBuffer.Unlock (CVPixelBufferLock.None);

				Task.Factory.StartNew (() => {



					IntPtr pp_data;

					int resLength = BarcodeConfig.MWB_scanGrayscaleImage (frameBuffer,width,height, out pp_data);
					frameBuffer = null;

					MWResults mwResults = null;
					MWResult mwResult = null;
					if (resLength > 0){

						byte[] pResult = new byte[resLength];
						byte[] rawResultTmp = new byte[resLength];
						Marshal.Copy(pp_data, pResult, 0, resLength);
						Marshal.FreeHGlobal(pp_data);

						if (MWScannerViewController.state == CameraState.NORMAL){
							resLength = 0;
						} else {
							
							mwResults = new MWResults(pResult);
							
							if (mwResults!=null && mwResults.count > 0){
								
								mwResult = mwResults.resultAtIndex(0);
							}
							
						}
						pResult = null;
					}
					
					//CVPixelBufferUnlockBaseAddress(imageBuffer,0);
					
					//ignore results less than 4 characters - probably false detection
					if (mwResult != null)
					{
						BarcodeConfig.MWB_setDuplicate(mwResult.bytes, mwResult.bytesLength);
						MWScannerViewController.state = CameraState.NORMAL;



						BeginInvokeOnMainThread( () => {

							if(MWScannerViewController.param_OverlayMode == Scanner.OM_MW && mwResult!=null && mwResult.locationPoints!=null && MWScannerViewController.param_EnableLocation){
								MWOverlay.showLocation(mwResult.locationPoints.points,mwResult.imageWidth,mwResult.imageHeight);
							}

							if(MWScannerViewController.parser_mask != BarcodeConfig.MWP_PARSER_MASK_NONE && !(MWScannerViewController.parser_mask == BarcodeConfig.MWP_PARSER_MASK_GS1 && !mwResult.isGS1)){
								
								byte[] parserResult = new byte[10000];
								double parserRes = -1;

								parserRes = BarcodeConfig.MWP_getJSON(MWScannerViewController.parser_mask, mwResult.encryptedResult, mwResult.bytesLength, out parserResult);



								if(parserRes >= 0){

									mwResult.text =  Encoding.UTF8.GetString(parserResult);


									int index = mwResult.text.IndexOf('\0');
									if (index >= 0)
										mwResult.text = mwResult.text.Remove(index);
									
									switch(MWScannerViewController.parser_mask){
									
									case BarcodeConfig.MWP_PARSER_MASK_AAMVA:
										mwResult.typeName = mwResult.typeName + " (AAMVA)";
										break;
									case BarcodeConfig.MWP_PARSER_MASK_IUID:
										mwResult.typeName = mwResult.typeName + " (IUID)";
										break;
									case BarcodeConfig.MWP_PARSER_MASK_ISBT:
										mwResult.typeName = mwResult.typeName + " (ISBT)";
										break;
									case BarcodeConfig.MWP_PARSER_MASK_HIBC:
										mwResult.typeName = mwResult.typeName + " (HIBC)";
										break;
									case BarcodeConfig.MWP_PARSER_MASK_SCM:
										mwResult.typeName = mwResult.typeName + " (SCM)";
										break;
										
									default:
										break;

									}

								}

							}

							if(MWScannerViewController.successCallback!=null)
								MWScannerViewController.successCallback.barcodeDetected (mwResult);
							else
							{

								ScannerResult result = new ScannerResult();

								result.type = mwResult.typeName;
								result.code = mwResult.text;
								result.isGS1 = mwResult.isGS1;
								result.bytes = mwResult.bytes;

								controllerInstance.handleResult (result);
							}


							if(MWScannerViewController.param_CloseScannerOnSuccess || MWScannerViewController.successCallback == null){
								controllerInstance.DismissViewController(true, null);
								if(MWScannerViewController.scannerInstance != null){
									MWScannerViewController.scannerInstance.closeScanner();
								}

							}
							
						});
						
					}
					else if(MWScannerViewController.state!= CameraState.NORMAL)
					{
						MWScannerViewController.state = CameraState.CAMERA;
					}
					
					
					


				
					MWScannerViewController.activeThreads--;
				});
			}


			try 
			{

				sampleBuffer.Dispose ();
				sampleBuffer = null;

			} catch (Exception e){
				Console.WriteLine (e);
			}

		}


	}


}

