/*
 
 Version 1.0
 
 The MWOverlay class serves to greatly simplify the addition of a dynamic viewfinder (similar to the one implemented 
 in Manatee Works Barcode Scanners application) to your own application.
 
 Minimum setup assumes:
 1. Add MWOverlay.java to your project;
 2. Put MWOverlay.addOverlay(this, surfaceView); after initialization of surface view;
 3. Put MWOverlay.removeOverlay(); on closing the activity;
 
 If all three steps are done correctly, you should be able to see a default red viewfinder with a blinking line, 
 capable of updating itself automatically after changing any of the scanning parameters (scanning direction, scanning
 rectangles and active barcode symbologies).
  
 The appearance of the viewfinder and the blinking line can be further customized by changing colors, line width, 
 transparencies and similar, by setting the following properties:
 
 	MWOverlay.isViewportVisible;
	MWOverlay.isBlinkingLineVisible;
	MWOverlay.viewportLineWidth;
	MWOverlay.blinkingLineWidth;
	MWOverlay.viewportAlpha;
	MWOverlay.viewportLineAlpha;
	MWOverlay.blinkingLineAlpha;
	MWOverlay.blinkingSpeed;
	MWOverlay.viewportLineColor;
	MWOverlay.blinkingLineColor;

*/

using Java.Util;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using System.Drawing;
using System;
using Android.Widget;

namespace ManateeShoppingCart.Droid.MWBarcodeScanner
{
	
	public class MWOverlay : View{
		
		public enum LayerType {
			LT_VIEWPORT,
			LT_LINE,
			LT_LOCATION
		}
		
		private static bool isAttached = false;
		public static bool isViewportVisible = true;
		public static bool isBlinkingLineVisible = true;
		
		public static float viewportLineWidth = 3.0f;
		public static float blinkingLineWidth = 1.0f;
		public static float locationLineWidth = 4.0f;
		
		public static float viewportAlpha = 0.5f;
		public static float viewportLineAlpha = 0.5f;
		public static float blinkingLineAlpha = 1.0f;
		public static float blinkingSpeed = 0.25f;
		public static int viewportLineColor = 0xff0000;
		public static int blinkingLineColor = 0xff0000;
		public static int locationLineColor = 0x00ff00;
		
		private static int lastOrientation = -1;
		private static int lastMask = -1;
		private static float lastLeft = -1;
		private static float lastTop = -1;
		private static float lastWidth = -1;
		private static float lastHeight = -1;
		private static float lastBLinkingSpeed = -1;
		
		private static float dpiCorrection = 1;
		
		private static MWOverlay viewportLayer;
		private static MWOverlay lineLayer;
		private static MWOverlay locationLayer;

		private Android.Graphics.PointF[] location = null;
		private int imageWidth = 0;
		private int imageHeight = 0;
		
		private LayerType layerType;

		public static View overlayView;
		
		private static System.Timers.Timer checkChangeTimer;
		
		private static Android.Content.Context mainContext = null;
		
		public static MWOverlay addOverlay (Context context, View previewLayer) {
			
			isAttached = true;
			mainContext = context;
			ViewGroup parent = (ViewGroup) previewLayer.Parent;
			
			DisplayMetrics metrics = context.Resources.DisplayMetrics;
			dpiCorrection = metrics.Density;
			
			viewportLayer = new MWOverlay(context);
			viewportLayer.layerType = LayerType.LT_VIEWPORT;
			
			lineLayer = new MWOverlay(context);
			lineLayer.layerType = LayerType.LT_LINE;
			
			locationLayer = new MWOverlay (context);
			locationLayer.layerType = LayerType.LT_LOCATION;
			
			
			viewportLayer.DrawingCacheEnabled = true;
			lineLayer.DrawingCacheEnabled = true;
			locationLayer.DrawingCacheEnabled = true;
			
			
			ViewGroup.LayoutParams rl;
			if (parent.Width + parent.Height > 0) {
				rl = new ViewGroup.LayoutParams (parent.Width, parent.Height);
			} else if (parent.LayoutParameters.Width + parent.LayoutParameters.Height > 0) {
				rl = new ViewGroup.LayoutParams (parent.LayoutParameters.Width, parent.LayoutParameters.Height);
			}else{
				rl = new ViewGroup.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			}
			
			parent.AddView(viewportLayer, rl);
			parent.AddView(lineLayer, rl);
			parent.AddView (locationLayer, rl);
			int totalCount = parent.ChildCount;
			
			for (int i = 0; i < totalCount; i++){
				
				View child = parent.GetChildAt(i);
				if (child.Equals(previewLayer) || child.Equals(viewportLayer) || child.Equals(lineLayer) || child.Equals(locationLayer)){
					
				} else {
					child.BringToFront();
					i--;
					totalCount--;
				}
				
			}
			
			checkChangeTimer = new System.Timers.Timer();
			checkChangeTimer.Interval = 200;
			checkChangeTimer.Elapsed += OnCheckChange;
			checkChangeTimer.Enabled = true;
			
			viewportLayer.PostInvalidate();
			lineLayer.PostInvalidate();
			
			updateAnimation();
			
			return viewportLayer;
			
		}

		public static void addOverlayView(Context context, View previewLayer)
		{

			if (overlayView != null) { 
				
				ViewGroup parent = (ViewGroup)previewLayer.Parent;
				parent.AddView(overlayView);
				overlayView.BringToFront();

			}

		}



		private static void OnCheckChange(object sender, System.Timers.ElapsedEventArgs e)
		{
			checkChange ();
		}
		
		public static void removeOverlay () {
			if (overlayView != null && overlayView.Parent != null) {
				((ViewGroup)overlayView.Parent).RemoveView(overlayView);
			}

			if (!isAttached)
				return;
			
			if (lineLayer == null || viewportLayer == null)
				return;
			
			checkChangeTimer.Stop();
			Animation animation = lineLayer.Animation;
			if (animation != null){
				animation.Cancel();
				animation.Reset();
			}
			ViewGroup viewParent = (ViewGroup) lineLayer.Parent;
			
			if (viewParent != null) {
				viewParent.RemoveView(lineLayer);
				viewParent.RemoveView(viewportLayer);
				viewParent.RemoveView (locationLayer);
			}
			isAttached = false;
			
			
		}


		private static void checkChange() {
			
			RectangleF frame = BarcodeConfig.MWBgetScanningRect(0);
			int activeCodes = BarcodeConfig.MWB_getActiveCodes ();
			int orientation = BarcodeConfig.MWB_getDirection();
			
			if (orientation != lastOrientation || frame.Left != lastLeft || frame.Top != lastTop || frame.Right != lastWidth || frame.Bottom != lastHeight) {
				
				viewportLayer.PostInvalidate();
				lineLayer.PostInvalidate();
			}
			
			if (lastBLinkingSpeed != blinkingSpeed){
				updateAnimation();
			}
			
			if (isBlinkingLineVisible != (lineLayer.Visibility == ViewStates.Visible)){
				
				lineLayer.PostInvalidate();
			}
			
			if (isViewportVisible != (viewportLayer.Visibility == ViewStates.Visible)){
				viewportLayer.PostInvalidate();
			}
			
		}
		
		private static void updateAnimation() {
			
			AlphaAnimation animation = new AlphaAnimation(0, 1);
			animation.RepeatMode = RepeatMode.Reverse;
			animation.RepeatCount =  Animation.Infinite;
			animation.Duration = (long) (blinkingSpeed * 1000);
			lineLayer.StartAnimation(animation);
			
			lastBLinkingSpeed = blinkingSpeed;
			
		}
		
		public MWOverlay(Context context) : base(context){
			//base(context);
		}
		
		
		
		public static void showLocation(Android.Graphics.PointF[] points, int imageWidth, int imageHeight)
		{
			locationLayer.imageWidth = imageWidth;
			locationLayer.imageHeight = imageHeight;
			
			locationLayer.Visibility = Android.Views.ViewStates.Visible;
			
			Android.Hardware.Camera.CameraInfo camInfo = new Android.Hardware.Camera.CameraInfo ();
			Android.Hardware.Camera.GetCameraInfo (0, camInfo);
			
			SurfaceOrientation rotation = ((Android.App.Activity)mainContext).WindowManager.DefaultDisplay.Rotation;
			locationLayer.location = points;
			
			if (rotation == SurfaceOrientation.Rotation180) {
				for (int i = 0; i < 4; i++) {
					locationLayer.location [i].X = imageWidth - points [i].X;
					locationLayer.location [i].Y = imageHeight - points [i].Y;
				}
			} else if (rotation == SurfaceOrientation.Rotation270) {
				for (int i = 0; i < 4; i++){
					locationLayer.location[i].X = imageWidth - points[i].X;
					locationLayer.location[i].Y = imageHeight - points[i].Y;
				}
			}
			
			Animation animationOld = locationLayer.Animation;
			if (animationOld != null) {
				animationOld.Cancel();
				animationOld.Reset ();
			}
			
			AlphaAnimation animation = new AlphaAnimation (1, 0);
			animation.Duration = (long)(0.5f * 1000);
			animation.FillAfter = true;
			animation.AnimationEnd += delegate {
				locationLayer.Visibility = Android.Views.ViewStates.Invisible;
			};
			locationLayer.StartAnimation (animation);
			locationLayer.PostInvalidate ();
		}



		override protected void OnDraw(Canvas canvas)
		{
			if (viewportLayer == null) {
				return;
			}

			RectangleF frame = BarcodeConfig.MWBgetScanningRect(0);

			SurfaceOrientation rotation = ((Android.App.Activity)mainContext).WindowManager.DefaultDisplay.Rotation;


			Android.Hardware.Camera.CameraInfo camInfo = new Android.Hardware.Camera.CameraInfo();
			Android.Hardware.Camera.GetCameraInfo(0, camInfo);

			if (rotation == SurfaceOrientation.Rotation0)
			{
				frame = new RectangleF(100 - frame.Y - frame.Height, frame.X, frame.Height, frame.Width);

			}
			else if (rotation == SurfaceOrientation.Rotation180)
			{
				frame = new RectangleF(frame.X, 100 - frame.Y - frame.Height, frame.Width, frame.Height);
			}
			else if (rotation == SurfaceOrientation.Rotation90)
			{

			}
			else {
				frame = new RectangleF(100 - frame.X - frame.Width, 100 - frame.Y - frame.Height, frame.Width, frame.Height);
			}

			/*if (this.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait){
				frame = new RectangleF(frame.Top, frame.Left, frame.Height, frame.Width);
			}*/

			lastLeft = frame.Left;
			lastTop = frame.Top;
			lastWidth = frame.Right;
			lastHeight = frame.Bottom;

			int width = canvas.Width;
			int height = canvas.Height;

			float rectLeft = frame.Left * width / 100.0f;
			float rectTop = frame.Top * height / 100.0f;
			float rectWidth = frame.Width * width / 100.0f;
			float rectHeight = frame.Height * height / 100.0f;

			frame = new RectangleF(rectLeft, rectTop, rectWidth, rectHeight);

			Paint paint = new Paint();

			if (layerType == LayerType.LT_LOCATION && location != null && locationLayer.location != null)
			{

				paint.Color = Android.Graphics.Color.Rgb(locationLineColor >> 16, (locationLineColor & 0xff00) >> 8, locationLineColor & 0xff);
				paint.Alpha = 255;
				paint.StrokeWidth = locationLineWidth * dpiCorrection;

				Android.Graphics.PointF[] correctedLocation = new Android.Graphics.PointF[4];

				float xScale = (float)width / imageWidth;
				float yScale = (float)height / imageHeight;

				if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
				{
					xScale = (float)width / imageHeight;
					yScale = (float)height / imageWidth;
				}

				for (int i = 0; i < 4; i++)
				{
					correctedLocation[i] = new Android.Graphics.PointF();

					if (Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
					{
						correctedLocation[i].X = width - locationLayer.location[i].Y * yScale;
						correctedLocation[i].Y = locationLayer.location[i].X * xScale;
					}
					else {
						correctedLocation[i].X = locationLayer.location[i].X * xScale;
						correctedLocation[i].Y = locationLayer.location[i].Y * yScale;
					}

				}

				canvas.DrawLine(correctedLocation[0].X, correctedLocation[0].Y, correctedLocation[1].X, correctedLocation[1].Y, paint);
				canvas.DrawLine(correctedLocation[1].X, correctedLocation[1].Y, correctedLocation[2].X, correctedLocation[2].Y, paint);
				canvas.DrawLine(correctedLocation[2].X, correctedLocation[2].Y, correctedLocation[3].X, correctedLocation[3].Y, paint);
				canvas.DrawLine(correctedLocation[3].X, correctedLocation[3].Y, correctedLocation[0].X, correctedLocation[0].Y, paint);


			}
			else if (layerType == LayerType.LT_VIEWPORT)
			{

				if (isViewportVisible != (viewportLayer.Visibility == ViewStates.Visible))
				{
					if (isViewportVisible)
					{
						viewportLayer.Visibility = ViewStates.Visible;
					}
					else {
						viewportLayer.Visibility = ViewStates.Invisible;
					}
				}

				paint.Color = Android.Graphics.Color.Argb(0xff, 0, 0, 0);
				paint.Alpha = ((int)(viewportAlpha * 255));


				canvas.DrawRect(0, 0, width, frame.Top, paint);
				canvas.DrawRect(0, frame.Top, frame.Left, frame.Bottom + 1, paint);
				canvas.DrawRect(frame.Right + 1, frame.Top, width, frame.Bottom + 1, paint);
				canvas.DrawRect(0, frame.Bottom + 1, width, height, paint);

				paint.Color = Android.Graphics.Color.Rgb(viewportLineColor >> 16, (viewportLineColor & 0xff00) >> 8, viewportLineColor & 0xff);

				paint.Alpha = (int)(viewportLineAlpha * 255);
				paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
				paint.StrokeWidth = (viewportLineWidth * dpiCorrection);

				canvas.DrawRect(frame.Left, frame.Top, frame.Right, frame.Bottom, paint);

			}
			else {

				if (isBlinkingLineVisible != (lineLayer.Visibility == ViewStates.Visible))
				{
					if (isBlinkingLineVisible)
					{
						lineLayer.Visibility = ViewStates.Visible;
						updateAnimation();
					}
					else {

						Animation animation = lineLayer.Animation;
						if (animation != null)
						{
							animation.Cancel();
							animation.Reset();
						}
						lineLayer.Visibility = ViewStates.Invisible;
					}
				}

				paint.Color = Android.Graphics.Color.Rgb(blinkingLineColor >> 16, (blinkingLineColor & 0xff00) >> 8, blinkingLineColor & 0xff);
				paint.StrokeWidth = (blinkingLineWidth * dpiCorrection);

				long curTime = Java.Lang.JavaSystem.CurrentTimeMillis() % 10000000;
				double position = ((double)curTime) / 1000.0d * 3.14d / blinkingSpeed;

				float lineAlpha = (float)(Math.Abs(Math.Sin(position)));

				paint.Alpha = ((int)(blinkingLineAlpha * lineAlpha * 255));
				paint.Alpha = ((int)(blinkingLineAlpha * 255));

				int orientation = BarcodeConfig.MWB_getDirection();

				if (this.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait)
				{

					double pos1f = Math.Log(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL) / Math.Log(2);
					double pos2f = Math.Log(BarcodeConfig.MWB_SCANDIRECTION_VERTICAL) / Math.Log(2);

					int pos1 = (int)(pos1f + 0.01);
					int pos2 = (int)(pos2f + 0.01);

					int bit1 = (orientation >> pos1) & 1;// bit at pos1
					int bit2 = (orientation >> pos2) & 1;// bit at pos2
					int mask = (bit2 << pos1) | (bit1 << pos2);
					orientation = orientation & 0xc;
					orientation = orientation | mask;

				}

				lastOrientation = orientation;

				if ((orientation & BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL) > 0 || (orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0)
				{

					float middle = frame.Height / 2 + frame.Top;
					canvas.DrawLine(frame.Left, middle, frame.Right, middle, paint);
				}
				if ((orientation & BarcodeConfig.MWB_SCANDIRECTION_VERTICAL) > 0 || (orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0)
				{

					float middle = frame.Width / 2 + frame.Left;
					canvas.DrawLine(middle, frame.Top, middle, frame.Bottom - 1, paint);
				}

				if ((orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0)
				{

					canvas.DrawLine(frame.Left + 2, frame.Top + 2, frame.Right - 2, frame.Bottom - 2, paint);
					canvas.DrawLine(frame.Right - 2, frame.Top + 2, frame.Left + 2, frame.Bottom - 2, paint);

				}
			}

		}

	}
}