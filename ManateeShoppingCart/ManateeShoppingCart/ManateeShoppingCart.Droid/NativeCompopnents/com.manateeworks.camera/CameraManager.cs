
using System;
using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Graphics;
using Android.Util;
using System.Threading;
using Java.Lang;

/**
 * This object wraps the Camera service object and expects to be the only one
 * talking to it. The implementation encapsulates the steps needed to take
 * preview-sized images, which are used for both preview and decoding.
 * 
 * @author dswitkin@google.com (Daniel Switkin)
 */

namespace ManateeShoppingCart.Droid.MWBarcodeScanner
{

	public class CameraManager {

		public static float currentFPS = 0f;
		public static int REFOCUSING_DELAY = 2000; 
		public static bool USE_SAMSUNG_FOCUS_ZOOM_PATCH = true;

		public static bool USE_FRONT_CAMERA = false;

		private static CameraManager cameraManager;

		private Context context;
		public CameraConfigurationManager configManager;
		public Android.Hardware.Camera camera;
		private bool initialized;
		public bool previewing;
		public static bool useOneShotPreviewCallback;
		public static bool useBufferedCallback = true;
//		private Android.Hardware.Camera.IPreviewCallback cb;

		public static int mDesiredWidth = 1280;
		public static int mDesiredHeight = 720;

		public Android.Views.ISurfaceHolder lastHolder;

		public Timer focusTimer;

		public static Android.Hardware.Camera.IAutoFocusCallback afCallback;
		public static bool refocusingActive = false;	

		public static bool DEBUG = false; 
		public static string TAG = "CameraManager";


		public static void setDesiredPreviewSize(int width, int height) {

			mDesiredWidth = width;
			mDesiredHeight = height;

		}
		
		public Point getMaxResolution() {
			
			if (camera != null)
				return CameraConfigurationManager.getMaxResolution(camera.GetParameters());
			
			else
				return null;

		}


		public Point getCurrentResolution() {

			if (camera != null){
				Android.Hardware.Camera.Parameters parms = camera.GetParameters();


				Point res = new Point(parms.PreviewSize.Width, parms.PreviewSize.Height);


				return res;
			}
			else
				return null;

		}

		public Point getNormalResolution(Point normalRes) {

			if (camera != null)
				return CameraConfigurationManager.getCameraResolution(camera.GetParameters(), normalRes);
			else
				return null;

		}
			


		/**
		 * Preview frames are delivered here, which we pass on to the registered
		 * handler. Make sure to clear the handler so it will only receive one
		 * message.
		 */
		public  PreviewCallback previewCallback;


		/**
		 * Initializes this static object with the Context of the calling Activity.
		 * 
		 * @param context
		 *            The Activity which wants to use the camera.
		 */

		public static void init(Context context) {
			if (cameraManager == null) {
				cameraManager = new CameraManager(context);
			}
		}


		/**
		 * Gets the CameraManager singleton instance.
		 * 
		 * @return A reference to the CameraManager singleton.
		 */
		public static CameraManager get() {
			return cameraManager;
		}


		private CameraManager(Context context) {

			this.context = context;
			this.configManager = new CameraConfigurationManager(context);
			useOneShotPreviewCallback = true;
			useBufferedCallback = true;

			previewCallback = new PreviewCallback(configManager);
		}

		/**
		 * Opens the camera driver and initializes the hardware parameters.
		 * 
		 * @param holder
		 *            The surface object which the camera will draw preview frames
		 *            into.
		 * @throws IOException
		 *             Indicates the camera driver failed to open.
		 */
		public void openDriver(ISurfaceHolder holder, bool isPortrait) {

			if (camera == null) {
				camera = Android.Hardware.Camera.Open (USE_FRONT_CAMERA ? 1 : 0);
				Android.Hardware.Camera.Parameters cp = camera.GetParameters();

				if (camera == null) {
					Log.Info (TAG, "Camera opening...");
					camera = Android.Hardware.Camera.Open (0);
					if (camera == null) {
						if (DEBUG)
							Log.Info (TAG, "Secoond camera open failed");
						throw new Java.IO.IOException ();
					}
				}
				if (DEBUG)
					Log.Info (TAG, "Camera open success");

				if ((int)Android.OS.Build.VERSION.SdkInt >= 9) {
					setCameraDisplayOrientation (USE_FRONT_CAMERA ? 1 : 0, camera, isPortrait);
				} else {
					if (isPortrait)
						camera.SetDisplayOrientation (90);
				}




				if (holder != null) {
					lastHolder = holder;
					camera.SetPreviewDisplay (holder);
					if (DEBUG)
						Log.Info (TAG, "Set camera current holder");
				} else {
					camera.SetPreviewDisplay (lastHolder);
					if (DEBUG)
						Log.Info (TAG, "Set camera last holder");
					if (lastHolder == null) {
						if (DEBUG)
							Log.Info (TAG, "Camera last holder is NULL");
					} 
				}

				if (!initialized) {
					initialized = true;
					configManager.initFromCameraParameters (camera);
					if (DEBUG)
						Log.Info (TAG, "configManager initialized");
				}
				configManager.setDesiredCameraParameters (camera);

			} else {
				if (DEBUG)
					Log.Info(TAG, "Camera already opened");
			}
		}

		public int getMaxZoom() {

			if (camera == null)
				return -1;

			Android.Hardware.Camera.Parameters cp = camera.GetParameters();
			if (!cp.IsZoomSupported){
				return -1;
			}

			IList<Integer> zoomRatios =  cp.ZoomRatios;


			return (int) zoomRatios [zoomRatios.Count - 1];

		}

		public void setZoom(int zoom){

			if (camera == null)
				return;

			Android.Hardware.Camera.Parameters cp = camera.GetParameters();

			int minDist = 100000;
			int bestIndex = 0;

			if (zoom == -1) {
				int zoomIndex = cp.Zoom - 1;

				if (zoomIndex >= 0){
					zoom = (int) cp.ZoomRatios[zoomIndex];
				}

			}

			IList<Integer> zoomRatios =  cp.ZoomRatios;

			if (zoomRatios != null){


				for (int i = 0; i < zoomRatios.Count; i++){
					int z = (int) zoomRatios[i];

					if (System.Math.Abs(z - zoom) < minDist){
						minDist = System.Math.Abs(z - zoom);
						bestIndex = i;
					}
				}

				int fBestIndex = bestIndex;

				if (USE_SAMSUNG_FOCUS_ZOOM_PATCH){

					if (bestIndex > 10){

						//camera.cancelAutoFocus();
						stopFocusing();
						cp.Zoom = fBestIndex - 5;

						camera.SetParameters (cp);

						camera.AutoFocus (null);
						new Handler ().PostDelayed (new Action(() => {

							if (camera != null){
								camera.CancelAutoFocus();
								cp.Zoom = fBestIndex;
								camera.SetParameters(cp);
							}

							startFocusing();
						}),200);



					} else {
						stopFocusing();
						cp.Zoom = fBestIndex;
						camera.SetParameters(cp);
						startFocusing();
					}

				} else {
					stopFocusing();
					cp.Zoom = fBestIndex;
					camera.SetParameters(cp);
					startFocusing();
				}
			}

		}

		public bool isTorchAvailable() {

			if (camera == null)
				return false;

			Android.Hardware.Camera.Parameters cp = camera.GetParameters();
			IList<System.String> flashModes = cp.SupportedFlashModes;
			if (flashModes != null && flashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeTorch))
				return true;
			else
				return false;


		}

		public void setTorch(bool enabled) {

			if (camera == null)
				return;

			try {
				Android.Hardware.Camera.Parameters cp = camera.GetParameters();

				IList<System.String> flashModes = cp.SupportedFlashModes;

				if (flashModes != null && flashModes.Contains(Android.Hardware.Camera.Parameters.FlashModeTorch)) {
					camera.CancelAutoFocus();

					new Handler ().PostDelayed (new Action(() => {
						
						if (enabled)
							cp.FlashMode = Android.Hardware.Camera.Parameters.FlashModeTorch;
						else
							cp.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;

						if (camera != null) 
							camera.SetParameters(cp);

					}),300);

				}
			} catch (System.Exception e) {
				
			}

		}

		public float[] getExposureCompensationRange(){

			if (camera == null)
				return null;

			try{
				Android.Hardware.Camera.Parameters cp = camera.GetParameters();

				float ecStep = cp.ExposureCompensationStep;
				float minEC = cp.MinExposureCompensation;
				float maxEC = cp.MaxExposureCompensation;

				float[] res = new float[3];
				res[0] = minEC;
				res[1] = maxEC;
				res[2] = ecStep;

				return res;

			} catch (System.Exception e) {

				return null;
			}

		}

		public void setExposureCompensation(float value) {

			if (camera == null)
				return;

			try{

				Android.Hardware.Camera.Parameters cp = camera.GetParameters();

				float ecStep = cp.ExposureCompensationStep;
				float minEC = cp.MinExposureCompensation;
				float maxEC = cp.MaxExposureCompensation;

				if (value > maxEC)
					value = maxEC;
				if (value < minEC)
					value = minEC;

				cp.ExposureCompensation = (int) value;

				camera.SetParameters(cp);

				//Log.d("exposure compensation", String.valueOf(value));

			} catch (System.Exception e) {
				//Log.d("exposure compensation", "failed to set");
			}

		}




		/**
		 * Closes the camera driver if still in use.
		 */
		public void closeDriver() {

			if (camera != null) {

				if (useBufferedCallback) {

				}

				camera.Release();
				camera = null;
			}
		}

		public void startFocusing(){

			if (refocusingActive){
				return;
			}
			refocusingActive = true;

			focusTimer = new System.Threading.Timer((o) =>
			{

			if (camera != null)
			{
					try
					{
						camera.AutoFocus(null);
						Console.WriteLine("refocusing");
					}catch(System.Exception e){
						
					}
				}
			}, null, 700, REFOCUSING_DELAY);
		}

		public void stopFocusing(){

			camera.CancelAutoFocus();
			if (!refocusingActive){
				return;
			}

			if (focusTimer != null){
				focusTimer.Dispose ();
			}

			refocusingActive = false;

		}


		/**
		 * Asks the camera hardware to begin drawing preview frames to the screen.
		 */
		public void startPreview() {

			if (camera != null && !previewing) {				
				Log.Info("preview", "preview started");
				camera.StartPreview();
				previewing = true;
				startFocusing ();
			}
		}

		/**
		 * Tells the camera to stop drawing preview frames.
		 */
		public void stopPreview() {
			if (camera != null && previewing) {

				if (useBufferedCallback) {
					previewCallback.setPreviewCallback (camera, null, 0, 0);
				}
				if (!useOneShotPreviewCallback) {
					camera.SetPreviewCallback (null);
				}
				stopFocusing ();
				camera.StopPreview();
//				previewCallback.setHandler(null, 0);
				previewCallback.setDecodeCallback (null);
				previewing = false;

			}
		}



		public void requestPreviewFrame(DecodeCallback decodeCallback) {

			if (camera != null && previewing) {

				previewCallback.setDecodeCallback(decodeCallback);
				if (useBufferedCallback) {
//					cb = new PreviewCallback(configManager);

					previewCallback.setPreviewCallback (camera, previewCallback, configManager.cameraResolution.X, configManager.cameraResolution.Y);
				}else if (useOneShotPreviewCallback) {
					camera.SetOneShotPreviewCallback(previewCallback);
				} else {
					camera.SetPreviewCallback(previewCallback);
				}

			}
		}

		/**
		 * Asks the camera hardware to perform an autofocus.
		 * 
		 * @param handler
		 *            The Handler to notify when the autofocus completes.
		 * @param message
		 *            The message to deliver.
		 */
		/*
		public void requestAutoFocus(Handler handler, int message) {

			if (camera != null && previewing) {
				afCallback.setHandler(handler, message);
				// Log.Debug(TAG, "Requesting auto-focus callback");
				camera.AutoFocus(afCallback);
			}

		}
		*/

		public Android.Content.Res.Orientation getDeviceDefaultOrientation() {

			IWindowManager windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();


			Android.Content.Res.Configuration config = context.Resources.Configuration;

			SurfaceOrientation rotation = windowManager.DefaultDisplay.Rotation;

			if ( ((rotation == SurfaceOrientation.Rotation0 || rotation == SurfaceOrientation.Rotation180) &&
				config.Orientation == Android.Content.Res.Orientation.Landscape)
				|| ((rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270) &&    
					config.Orientation == Android.Content.Res.Orientation.Portrait)) {
				return Android.Content.Res.Orientation.Landscape;
			}
			else{ 
				return Android.Content.Res.Orientation.Portrait;
			}
		}

		public void updateCameraOrientation(SurfaceOrientation rotation){

			if (camera == null) {
				return;
			}

			Android.Content.Res.Orientation deviceOrientation = getDeviceDefaultOrientation();

			if (deviceOrientation == Android.Content.Res.Orientation.Portrait){
				switch (rotation) {
				case SurfaceOrientation.Rotation0:
					camera.SetDisplayOrientation(90);
					break;
				case SurfaceOrientation.Rotation180:
					camera.SetDisplayOrientation(270);
					break;
				case SurfaceOrientation.Rotation270:
					camera.SetDisplayOrientation(180);
					break;
				case SurfaceOrientation.Rotation90:
					camera.SetDisplayOrientation(0);
					break;

				default:
					break;
				} 
			}else {

				switch (rotation) {
				case SurfaceOrientation.Rotation0:
					camera.SetDisplayOrientation(0);
					break;
				case SurfaceOrientation.Rotation180:
					camera.SetDisplayOrientation(180);
					break;
				case SurfaceOrientation.Rotation270:
					camera.SetDisplayOrientation(90);
					break;
				case SurfaceOrientation.Rotation90:
					camera.SetDisplayOrientation(270);
					break;

				default:
					break;	
				}

			}

		}


		public void setCameraDisplayOrientation(int cameraId, Android.Hardware.Camera camera, bool isPortrait) {
			Android.Hardware.Camera.CameraInfo info =
				new Android.Hardware.Camera.CameraInfo();
			Android.Hardware.Camera.GetCameraInfo(cameraId, info);


//			IWindowManager windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

			Display d = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>().DefaultDisplay;


			SurfaceOrientation rotation = d.Rotation;

			int degrees = 0;
			switch (rotation) {
			case SurfaceOrientation.Rotation0: degrees = 0; break;
			case SurfaceOrientation.Rotation90: degrees = 90; break;
			case SurfaceOrientation.Rotation180: break;
			case SurfaceOrientation.Rotation270: degrees = 270; break;
			}

			int result;
			if (info.Facing == Android.Hardware.Camera.CameraInfo.CameraFacingFront) {
				result = (info.Orientation + degrees) % 360;
				result = (360 - result) % 360;  // compensate the mirror
			} else {  // back-facing
				result = (info.Orientation - degrees + 360) % 360;
			}
			camera.SetDisplayOrientation(result);
		}
			


		public Bitmap renderCroppedGreyscaleBitmap(byte[] data, int width, int height) {
			int[] pixels = new int[width * height];
			byte[] yuv = data;
			int row = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					int grey = yuv [row + x] & 255;
					const uint u = 0xFF000000;
					pixels [row + x] = (int)(u | (uint)(grey * 65793));
				}
				row += width;
			}

			Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
			bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);
			return bitmap;
		}








//		/**
//		 * Calculates the framing rect which the UI should draw to show the user
//		 * where to place the barcode. This target helps with alignment as well as
//		 * forces the user to hold the device far enough away to ensure the image
//		 * will be in focus.
//		 * 
//		 * @return The rectangle to draw on screen in window coordinates.
//		 */
//
//		public Rect getFramingRect() {
//			Point screenResolution = configManager.getScreenResolution();
//			if (framingRect == null) {
//				if (camera == null) {
//					return null;
//				}
//
//				int width, height;
//
//				width = screenResolution.Y;
//				height = screenResolution.Y;
//
//				int leftOffset = (screenResolution.X - width) / 2;
//				int topOffset = (screenResolution.Y - height) / 2;
//				framingRect = new Rect(leftOffset, topOffset, leftOffset + width, topOffset + height);
//				Log.Debug(TAG, "Calculated framing rect: " + framingRect);
//			}
//			return framingRect;
//		}
//
//		/**
//		 * Like {@link #getFramingRect} but coordinates are in terms of the preview
//		 * frame, not UI / screen.
//		 */
//		public Rect getFramingRectInPreview() {
//			if (framingRectInPreview == null) {
//				Rect rect = new Rect(getFramingRect());
//				Point cameraResolution = configManager.getCameraResolution();
//				Point screenResolution = configManager.getScreenResolution();
//				rect.Left = rect.Left * cameraResolution.X / screenResolution.X;
//				rect.Right = rect.Right * cameraResolution.X / screenResolution.X;
//				rect.Top = rect.Top * cameraResolution.Y / screenResolution.Y;
//				rect.Bottom = rect.Bottom * cameraResolution.Y / screenResolution.Y;
//				framingRectInPreview = rect;
//			}
//			return framingRectInPreview;
//		}
//
//
//
//		public int getFrameWidth() {
//			Rect rect = getFramingRect();
//			return rect.Right - rect.Left + 1;
//		}
//
//		public int getFrameHeight() {
//			Rect rect = getFramingRect();
//			return rect.Bottom - rect.Top + 1;
//		}
//
	}
	public class CameraConfigurationManager {

		private static string TAG = "CameraConfigurationManager";

		private Context context;
		public static Point screenResolution;
		public Point cameraResolution;
		private ImageFormatType previewFormat;
		private string previewFormatString;

		public CameraConfigurationManager(Context context) {
			this.context = context;
		}

		/**
		* Reads, one time, values from the camera that are needed by the app.
		*/
		public void initFromCameraParameters(Android.Hardware.Camera camera) {

			Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
			previewFormat = parameters.PreviewFormat;
			previewFormatString = parameters.Get("preview-format");
			Log.Debug(TAG, "Default preview format: " + previewFormat + '/' + previewFormatString);

			IWindowManager manager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			Display display = manager.DefaultDisplay;
			screenResolution = new Point(display.Width, display.Height);
			Log.Debug(TAG, "Screen resolution: " + screenResolution);
			cameraResolution = getCameraResolution(parameters, screenResolution);
			Log.Debug(TAG, "Camera resolution: " + screenResolution);
		}



		public void setDesiredCameraParameters(Android.Hardware.Camera camera) {
			Android.Hardware.Camera.Parameters parameters = camera.GetParameters();
			cameraResolution = getCameraResolution(parameters, new Point(CameraManager.mDesiredWidth, CameraManager.mDesiredHeight));
			Log.Debug(TAG, "Setting preview size: " + cameraResolution);
			parameters.SetPreviewSize(cameraResolution.X, cameraResolution.Y);

			/*try {
			String vs =  parameters.get("anti-shake");
			if (vs != null) {
				parameters.set("anti-shake", "1");
			}
		} catch (Exception e){
		}*/
			try {
				string vss = parameters.Get("video-stabilization-supported");
				if (vss != null && vss.Equals("true")) {

					string vs = parameters.Get("video-stabilization");
					if (vs != null) {
						parameters.Set("video-stabilization", "true");
					}
				}

			} catch (System.Exception e) {
			}

			

			/*try {
				string vs = parameters.Get("video-stabilization-ocr");
				if (vs != null) {
					parameters.Set("video-stabilization-ocr", "true");
				}
			} catch (System.Exception e){
			}


			try {
				string vs = parameters.Get("touch-af-aec-values");
				if (vs != null) {
					parameters.Set("touch-af-aec-values", "touch-on");
				}
			} catch (System.Exception e){
			}*/

			string focusMode = parameters.FocusMode;

			try{
				parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeAuto;			
				//parameters.setFocusMode("macro");
				camera.SetParameters(parameters);
			} catch (System.Exception e){

				try{
					parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeAuto;
					camera.SetParameters(parameters);
				} catch (System.Exception e2){
					parameters.FocusMode = focusMode;			
				}

			}

			try {

				IList<int[]> supportedFPS = parameters.SupportedPreviewFpsRange;

				int maxFps = -1;
				int maxFpsIndex = -1;
				for (int i = 0; i < supportedFPS.Count; i++){
					int[] sr = supportedFPS[i];
					if (sr[1] > maxFps){
						maxFps = sr[1];
						maxFpsIndex = i;
					}
				}

				parameters.SetPreviewFpsRange(supportedFPS[maxFpsIndex][0], supportedFPS[maxFpsIndex][1]);

			} catch (System.Exception e){
			}


			Log.Debug(TAG, "Camera parameters flat: " + parameters.Flatten());
			camera.SetParameters(parameters);
		}

		public Point getCameraResolution() {
			return cameraResolution;
		}

		public Point getScreenResolution() {
			return screenResolution;
		}

		public int getPreviewFormat() {
			return (int) previewFormat;
		}

		public string getPreviewFormatString() {
			return previewFormatString;
		}

		public static Point getMaxResolution(Android.Hardware.Camera.Parameters parameters) {

			IList<Android.Hardware.Camera.Size> sizes = parameters.SupportedPreviewSizes;

			int minIndex = -1;
			int maxSize = 0;

			for (int i = 0; i < sizes.Count; i++) {
				int size = sizes[i].Width * sizes[i].Height; 
				if (size > maxSize) {
					maxSize = size;
					minIndex = i;
				}
			}

			return new Point(sizes[minIndex].Width, sizes[minIndex].Height);
		}

		public static Point getCameraResolution(Android.Hardware.Camera.Parameters parameters, Point desiredResolution) {

			System.String previewSizeValueString = parameters.Get("preview-size-values");
			// saw this on Xperia
			if (previewSizeValueString == null) {
				previewSizeValueString = parameters.Get("preview-size-value");
			}

			Point cameraResolution = null;

//			if (CameraManager.mDesiredWidth == 0)
//				CameraManager.mDesiredWidth = screenResolution.X;
//			if (CameraManager.mDesiredHeight == 0)
//				CameraManager.mDesiredHeight = screenResolution.Y;

			IList<Android.Hardware.Camera.Size> sizes = parameters.SupportedPreviewSizes;

			int minDif = 99999;
			int minIndex = -1;

			float X = CameraConfigurationManager.screenResolution.X;
			float Y = CameraConfigurationManager.screenResolution.Y;
			float screenAR = ((float)(X > Y ? X : Y)) / (X < Y ? X : Y);

			for (int i = 0; i < sizes.Count; i++) {

				int dif = System.Math.Abs(sizes[i].Width - CameraManager.mDesiredWidth) + 
					System.Math.Abs(sizes[i].Height - CameraManager.mDesiredHeight);

				if (dif < minDif) {
					minDif = dif;
					minIndex = i;
				}
			}

			float desiredTotalSize = desiredResolution.X * desiredResolution.Y;
			float bestARdifference = 100;


			for (int i = 0; i < sizes.Count; i++) {

				float resAR = ((float) sizes[i].Width) / sizes[i].Height;

				float totalSize = sizes[i].Width * sizes[i].Height;

				float difference;

				if (totalSize >= desiredTotalSize) {
					difference = totalSize / desiredTotalSize;
				} else {
					difference = desiredTotalSize / totalSize;
				}

				float ARdifference;

				if (resAR >= screenAR) {
					ARdifference = resAR / screenAR;
				} else {
					ARdifference = screenAR / resAR;
				}

				if (difference < 1.1 && ARdifference < bestARdifference) {
					bestARdifference = ARdifference;
					minIndex = i;
				}

			}

			cameraResolution = new Point(sizes[minIndex].Width, sizes[minIndex].Height);

			return cameraResolution;
		}


	}

	public class PreviewCallback:Java.Lang.Object,Android.Hardware.Camera.IPreviewCallback {

		int fpscount;
		long lasttime = 0;
		private  CameraConfigurationManager configManager;
//		public Android.OS.Handler previewHandler;
		public int previewMessage;
		private DecodeCallback decodeCallback;
		public byte[][] frameBuffers;
		public int fbCounter = 0;
		public bool callbackActive = false;

		public PreviewCallback(CameraConfigurationManager configManager) {
			this.configManager = configManager;
		}
		/*
		public void setHandler(Android.OS.Handler previewHandler, int previewMessage) {
			this.previewHandler = previewHandler;
			this.previewMessage = previewMessage;
		}
		*/
		public void setDecodeCallback(DecodeCallback decodeCallback) {
			this.decodeCallback = decodeCallback;
		}
		public virtual void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera) {
			updateFps();

			Point cameraResolution = configManager.getCameraResolution();

			if (CameraManager.useBufferedCallback) {
				// camera.addCallbackBuffer(frameBuffers[fbCounter]);
				// fbCounter = 1 - fbCounter;
				setPreviewCallback(camera, this, cameraResolution.X, cameraResolution.Y);
			}

		/*	if (previewHandler != null) {
				Message message = previewHandler.ObtainMessage(previewMessage, cameraResolution.X, cameraResolution.Y, data);
				message.SendToTarget();*/
			if (decodeCallback != null) {
				decodeCallback.decode (data, cameraResolution.X, cameraResolution.Y); 
			}
				/*if (!CameraManager.useBufferedCallback) {
					previewHandler = null;
				}
			}*/
		}

		public int setPreviewCallback(Android.Hardware.Camera camera, Android.Hardware.Camera.IPreviewCallback callback, int width, int height) {

			if (callback != null) {
				if (frameBuffers == null) {
					// add 10% additional space for any case
					frameBuffers = new byte[2][];
					frameBuffers [0]=new byte[width * height * 2 * 110 / 100];
					frameBuffers [1] = new byte[width * height * 2 * 110 / 100];
					fbCounter = 0;
					Log.Info("preview resolution",width+ "x" + height);

				}
				if (!callbackActive) {
					camera.SetPreviewCallbackWithBuffer(callback);
					callbackActive = true;
				}

				// CameraDriver.bufferProccessed = -1;
				camera.AddCallbackBuffer(frameBuffers[fbCounter]);
				fbCounter = 1 - fbCounter;
			} else {
				camera.SetPreviewCallbackWithBuffer(callback);
				callbackActive = false;
			}

			if (callback == null) {
				frameBuffers = null;
				JavaSystem.Gc ();
			}

			return 0;
		}
			

		private void updateFps() {
			if (lasttime == 0) {
				lasttime = JavaSystem.CurrentTimeMillis();
				fpscount = 0;
				CameraManager.currentFPS = 0;
			} else {
				long delay = JavaSystem.CurrentTimeMillis() - lasttime;
				if (delay > 2000) {
					lasttime = JavaSystem.CurrentTimeMillis();
					CameraManager.currentFPS = fpscount * 10000 / delay;
					CameraManager.currentFPS /= 10;
					fpscount = 0;
				}
			}
			fpscount++;
		}



	}

}
