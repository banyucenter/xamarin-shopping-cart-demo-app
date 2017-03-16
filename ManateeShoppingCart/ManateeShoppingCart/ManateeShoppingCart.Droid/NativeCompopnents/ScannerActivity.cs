using Java.IO;
using System;
using Java.Lang;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using System.Threading.Tasks;
using System.Text;
using Android.Content.Res;
using Android.Hardware;
using System.Runtime.InteropServices;

namespace ManateeShoppingCart.Droid.MWBarcodeScanner
{
	
	public interface DecodeCallback{
		void decode (byte[] data, int width, int height);
	}
	
	
	[Activity (Label = "ScannerActivity", ConfigurationChanges= ConfigChanges.KeyboardHidden, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
	public class ScannerActivity: Activity, ISurfaceHolderCallback, DecodeCallback {
		
		public static int MAX_THREADS = Runtime.GetRuntime().AvailableProcessors();
		public static int param_maxThreads = 4;
		public static int activeThreads = 0;
		
		public enum OverlayMode
		{
			OM_NONE,
			OM_MW,
			OM_IMAGE,
			OM_VIEW
		}

		//		public static Handler handler;
		public const int MSG_DECODE = 1;
		public const  int MSG_AUTOFOCUS = 2;
		public const  int MSG_DECODE_SUCCESS = 3;
		public const  int MSG_DECODE_FAILED = 4;
		
		//	    private byte[] lastResult;
		public static bool hasSurface;
		public static SurfaceView surfaceView;

		public static ScreenOrientation param_Orientation = ScreenOrientation.Portrait;
		public static bool param_EnableHiRes = true;
		public static bool param_EnableFlash = true;
		public static bool param_EnableZoom = true;
		public static bool param_EnableLocation = true;
		
		public static bool param_CloseScannerOnSuccess = true;
		public static bool param_defaultFlashOn = false;
		
		public static int param_ZoomLevel1 = 0;
		public static int param_ZoomLevel2 = 0;
		public static int zoomLevel = 0;
		public static int firstZoom = 150;
		public static int secondZoom = 300;
		
		public static int parserMask = BarcodeConfig.MWP_PARSER_MASK_NONE;
		
		
		public static IScanSuccessCallback successCallback;
		
		public static OverlayMode param_OverlayMode = OverlayMode.OM_MW;
		
		private ImageView overlayImage;
		private ImageButton buttonFlash;
		private ImageButton buttonZoom;
		public static Activity activity;
		
		public enum State {
			STOPPED, PREVIEW, DECODING
		}
		
		public static State state = State.STOPPED;
		
		
		public static bool flashOn = false;
		
		public static event Action <ScannerResult> OnScanCompleted;
		private static bool requestingOrientation = false;

		public override void OnBackPressed ()
		{
			if (successCallback != null)
				successCallback.barcodeDetected (null);
			else
				OnScanCompleted (null);

            OnScanCompleted = null;

            Finish();
		}
		
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (param_Orientation != ScreenOrientation.Unspecified)
			{
				this.RequestedOrientation = param_Orientation;

				Android.Content.Res.Orientation currentOrientation = this.Resources.Configuration.Orientation;

				if (!((param_Orientation ==  ScreenOrientation.Landscape || param_Orientation == ScreenOrientation.ReverseLandscape)
				      && currentOrientation == Android.Content.Res.Orientation.Landscape)
				    && !(param_Orientation == ScreenOrientation.Portrait && currentOrientation == Android.Content.Res.Orientation.Portrait))
				{
					requestingOrientation = true;
					return;
				}
				else {
					requestingOrientation = false;
				}
			}

			if (!requestingOrientation)
			{
				
				state = State.STOPPED;
				activity = this;

				SetContentView(Resource.Layout.Scanner);


				if (overlayImage == null) { 
					overlayImage = (ImageView)FindViewById(Resource.Id.overlayImage);
				}

				if (buttonFlash == null) { 
					buttonFlash = (ImageButton)FindViewById(Resource.Id.flashButton);
					buttonFlash.Click += delegate
					{
						toggleFlash();
					};
				}

				if (buttonZoom == null) { 
					buttonZoom = (ImageButton)FindViewById(Resource.Id.zoomButton);
					if (param_EnableZoom)
						buttonZoom.Click += delegate
					{
						toggleZoom();
					};
					else
						buttonZoom.Visibility = ViewStates.Gone;
				}


				CameraManager.init(this);
			}
			
		}
		
		
		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			if (!requestingOrientation)
			{
				
				Display display = WindowManager.DefaultDisplay;
				SurfaceOrientation rotation = display.Rotation;

				CameraManager.get().updateCameraOrientation(rotation);

			}
			//this.Finish();
			
		} 
	
		protected override void OnResume()
		{
			base.OnResume();
			if (!requestingOrientation)
			{


				const int RequestLocationId = 0;

				Display display = WindowManager.DefaultDisplay;
				SurfaceOrientation rotation = display.Rotation;

				CameraManager.get().updateCameraOrientation(rotation);

				surfaceView = (SurfaceView)FindViewById(Resource.Id.preview_view);
				ISurfaceHolder surfaceHolder = surfaceView.Holder;

				if (param_OverlayMode == OverlayMode.OM_MW)
				{
					MWOverlay.addOverlay(this, surfaceView);
				}

				if (param_OverlayMode == OverlayMode.OM_IMAGE)
				{
					overlayImage.Visibility = ViewStates.Visible;
				}
				else {
					overlayImage.Visibility = ViewStates.Gone;
				}

				if (param_OverlayMode == OverlayMode.OM_VIEW) {
					MWOverlay.addOverlayView(this, surfaceView);
				}

				if (hasSurface)
				{
					// The activity was paused but not stopped, so the surface still
					// exists. Therefore
					// surfaceCreated() won't be called, so init the camera here.
					initCamera(surfaceHolder);
				}
				else
				{
					// Install the callback and wait for surfaceCreated() to init the
					// camera.
					surfaceHolder.AddCallback(this);
					surfaceHolder.SetType(SurfaceType.PushBuffers);
				}


				//			int ver = BarcodeConfig.MWB_getLibVersion();
				//			int v1 = (ver >> 16);
				//			int v2 = (ver >> 8) & 0xff;
				//			int v3 = (ver & 0xff);
				//			string libVersion = "Lib version: " + v1.ToString()+"."+v2.ToString()+"."+v3.ToString();


			}

		}

		protected override void OnPause()
		{
			base.OnPause();
			if (!requestingOrientation)
			{


				flashOn = false;
				updateFlash();
				if (param_OverlayMode == OverlayMode.OM_MW || param_OverlayMode == OverlayMode.OM_VIEW)
				{
					MWOverlay.removeOverlay();
				}

				CameraManager.get().stopPreview();

				CameraManager.get().closeDriver();
				state = State.STOPPED;
			}
		}
		
		private void toggleFlash() {
			flashOn = !flashOn;
			updateFlash();
		}
		
		private void updateFlash() {

			if (!CameraManager.get().isTorchAvailable() || !param_EnableFlash) { 
				buttonFlash.Visibility = ViewStates.Gone;
				return;
				
			} else {
				buttonFlash.Visibility = ViewStates.Visible;
			}
			
			if (flashOn) {
				buttonFlash.SetImageResource(Resource.Drawable.flashbuttonon);
			} else {
				buttonFlash.SetImageResource(Resource.Drawable.flashbuttonoff);
			}
			CameraManager.get().setTorch(flashOn);
			
			buttonFlash.PostInvalidate();
			
		}
		public void SurfaceChanged (ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height){
			
			if (!hasSurface)
			{
				hasSurface = true;
				initCamera(holder);
			}
			
		}
		
		
		
		public void SurfaceCreated(ISurfaceHolder holder) {
			
		}
		
		
		
		public void SurfaceDestroyed(ISurfaceHolder holder) {
			
			hasSurface = false;
			
		}
		
		public static void setParserMask(int parserMask){
			ScannerActivity.parserMask = parserMask;
		}
		
		
		private void initCamera(ISurfaceHolder surfaceHolder)
		{
			try
			{
				// Select desired camera resoloution. Not all devices supports all resolutions, closest available will be chosen
				// If not selected, closest match to screen resolution will be chosen
				// High resolutions will slow down scanning proccess on slower devices
				
				if (param_EnableHiRes){
					CameraManager.setDesiredPreviewSize(1280, 720);
				} else {
					CameraManager.setDesiredPreviewSize(800, 480);
				}

				CameraManager.get().openDriver(surfaceHolder, (this.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait));
				int maxZoom = CameraManager.get().getMaxZoom();
				if (maxZoom > 100) {
					if (param_EnableZoom) {
						updateZoom();
					}
				}
			}
			catch (IOException ioe)
			{
				DisplayFrameworkBugMessageAndExit(ioe.Message);
				return;
			}
			catch (System.Exception e)
			{
				// Barcode Scanner has seen crashes in the wild of this variety:
				// java.?lang.?RuntimeException: Fail to connect to camera service
				DisplayFrameworkBugMessageAndExit(e.Message);
				return;
			}

			/*
			if (handler == null)
		        {

				handler = new Handler(msg => {
					switch (msg.What) {
					case MSG_AUTOFOCUS:
						if (state == State.PREVIEW || state == State.DECODING) {
							CameraManager.get().requestAutoFocus(handler, MSG_AUTOFOCUS);
						}
						
						break;
					case MSG_DECODE:

						decode((byte[]) msg.Obj, msg.Arg1, msg.Arg2);
						break;
					case MSG_DECODE_FAILED:

						//						CameraManager.decoding = false;
						//CameraManager.get().requestPreviewFrame(handler, MSG_DECODE);
						break;
					case MSG_DECODE_SUCCESS:
						state = State.STOPPED;
						
						handleDecode((byte[]) msg.Obj);
						break;
						
					default:
						break;
					}
				});
				
			}
			*/

			//Fix for camera sensor rotation bug
			var cameraInfo = new Camera.CameraInfo();
			Camera.GetCameraInfo(0, cameraInfo);
			if (cameraInfo.Orientation == 270)
			{
				BarcodeConfig.MWB_setFlags(0, BarcodeConfig.MWB_CFG_GLOBAL_ROTATE180);

			}

			flashOn = param_defaultFlashOn;

			startScanning();
			
			updateFlash();		
			
		}
		
		
		private void DisplayFrameworkBugMessageAndExit(string message)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("MW Barcode Scanner");
			builder.SetMessage("Camera framework bug: "+message);
			builder.SetPositiveButton("OK", delegate{
				
			});
			builder.Show();
		}
		
		
		private void startScanning() {

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

			CameraManager.get().startPreview();
			state = State.PREVIEW;
			
			BarcodeConfig.MWB_setResultType (BarcodeConfig.MWB_RESULT_TYPE_MW);
			CameraManager.get().requestPreviewFrame(this);
			//	         CameraManager.get().requestAutoFocus();
		}

		void DecodeCallback.decode(byte[] data, int width, int height) {
			
			
			if (param_maxThreads > MAX_THREADS) {
				param_maxThreads = MAX_THREADS;
			}
			
			if (activeThreads >= param_maxThreads || state == State.STOPPED) {
				return;
			}	
			
			activeThreads++;
			Task.Factory.StartNew (() => {
				
				MWResult mwResult = null;
                IntPtr pp_data;
                int resLen = 0;
				
				resLen = BarcodeConfig.MWB_scanGrayscaleImage(data, width, height, out pp_data);

               
                if (state == State.STOPPED) {
					activeThreads--;
					return;
				}
				
				if (resLen > 0 && BarcodeConfig.MWB_getResultType() == BarcodeConfig.MWB_RESULT_TYPE_MW) {
                    byte[] rawResultTmp = new byte[resLen];
                    Marshal.Copy(pp_data, rawResultTmp, 0, resLen);
                    Marshal.FreeHGlobal(pp_data);

                    MWResults results = new MWResults(rawResultTmp);
					
					if (results.count > 0) {
						mwResult = results.getResult(0);
					}
                    rawResultTmp = null;

                }


                if (resLen>0 && mwResult != null)
				{
					BarcodeConfig.MWB_setDuplicate(mwResult.bytes,mwResult.bytesLength);
					
					if (state == State.STOPPED) { 
						activeThreads--;	
						return;
					}
					
					state = State.STOPPED;
					
					
					RunOnUiThread(() => 
					              handleDecode (mwResult)
					             );
				}
				
				activeThreads--;
				
				
			});
			
			
		}
		
		
		public void handleDecode(MWResult result)
		{
			byte[] rawResult = null;
			
			if (result != null && result.bytes != null) {
				rawResult = result.bytes;
			}
			
			string s = "";
			
			try {
				System.Text.UTF8Encoding encoding=new System.Text.UTF8Encoding();
				s = encoding.GetString(rawResult);
			} catch (UnsupportedEncodingException e) {
				
				s = "";
				for (int i = 0; i < rawResult.Length; i++)
					s = s + (char) rawResult[i];	
			}
			
			int bcType = result.type;
			string typeName = BarcodeConfig.getBarcodeName (bcType);

			result.typeText = typeName;
			
			if (ScannerActivity.param_EnableLocation && result.locationPoints != null && CameraManager.get().getCurrentResolution() != null && param_OverlayMode == OverlayMode.OM_MW) {
				MWOverlay.showLocation(result.locationPoints.points, result.imageWidth, result.imageHeight);
			}
			
			if(ScannerActivity.parserMask != BarcodeConfig.MWP_PARSER_MASK_NONE && !(ScannerActivity.parserMask == BarcodeConfig.MWP_PARSER_MASK_GS1 && !result.isGS1)){
				byte[] parserResult = new byte[10000];
				double parserRes = -1;

				try
				{
					parserRes = BarcodeConfig.MWP_getJSON(ScannerActivity.parserMask, result.encryptedResult.GetBytes(), result.bytesLength, out parserResult);
				}
				catch (NullReferenceException e)
				{

				}

				if (parserRes >= 0) {
					
					s = Encoding.UTF8.GetString (parserResult);
					result.text = new Java.Lang.String (s);
					
					int index = result.text.IndexOf ('\0');
					if (index >= 0) {
						s = s.Remove (index);
						result.text = new Java.Lang.String (s);
					}
					
					switch (ScannerActivity.parserMask) {
					
					case BarcodeConfig.MWP_PARSER_MASK_AAMVA:
						result.typeText = result.typeText + " (AAMVA)";
						break;
					case BarcodeConfig.MWP_PARSER_MASK_IUID:
						result.typeText = result.typeText + " (IUID)";
						break;
					case BarcodeConfig.MWP_PARSER_MASK_ISBT:
						result.typeText = result.typeText + " (ISBT)";
						break;
					case BarcodeConfig.MWP_PARSER_MASK_HIBC:
						result.typeText = result.typeText + " (HIBC)";
						break;
					case BarcodeConfig.MWP_PARSER_MASK_SCM:
						result.typeText = result.typeText + " (SCM)";
						break;
					default:
						break;
						
					}
					typeName = result.typeText;

				} else {
						
				}
			}
			
			if (successCallback == null) {
				Intent data = new Intent();
				data.PutExtra("code", s);
				data.PutExtra("type", typeName);
				data.PutExtra("isGS1", result.isGS1);
				data.PutExtra("bytes", rawResult);
				this.SetResult((Android.App.Result) 1, data);
				ScannerResult res = new ScannerResult ();
				res.bytes = rawResult;
				res.code = s;
				res.type = typeName;
				res.isGS1 = result.isGS1;
				OnScanCompleted (res);
				
			} else {
				successCallback.barcodeDetected(result);
			}

            if (param_CloseScannerOnSuccess || successCallback == null)
            {
                OnScanCompleted = null;
                Finish();
            }
			
		}
		

		public static void toggleZoom() {
			
			zoomLevel++;
			if (zoomLevel > 2) {
				zoomLevel = 0;
			}
			
			updateZoom();
		}
		
		public static void updateZoom() {
			
			if (param_ZoomLevel1 == 0 || param_ZoomLevel2 == 0) {
				firstZoom = 150;
				secondZoom = 300;
			} else {
				firstZoom = param_ZoomLevel1;
				secondZoom = param_ZoomLevel2;
				
				int maxZoom = CameraManager.get().getMaxZoom();
				
				if (maxZoom < secondZoom) {
					secondZoom = maxZoom;
				}
				if (maxZoom < firstZoom) {
					firstZoom = maxZoom;
				}
				
			}
			
			switch (zoomLevel) {
			case 0:
				CameraManager.get().setZoom(100);
				break;
			case 1:
				CameraManager.get().setZoom(firstZoom);
				break;
			case 2:
				CameraManager.get().setZoom(secondZoom);
				break;
				
			default:
				break;
			}
		}
		
		
		
	}
}