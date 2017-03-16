using System;

using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreAnimation;
using AVFoundation;
using CoreVideo;
using Foundation;
using UIKit;

using System.Drawing;

namespace ManateeShoppingCart.iOS.MWBarcodeScanner
{
	public class MWOverlay: NSObject
	{
		public MWOverlay ()
		{
		}

		private static float CHANGE_TRACKING_INTERVAL = 0.1f;

		private static CALayer viewportLayer;
		private static CALayer lineLayer;
		private static CALayer locationLayer;

		private static AVCaptureVideoPreviewLayer previewLayer;
		private static bool isAttached = false;
		private static bool isViewportVisible = true;
		private static bool isBlinkingLineVisible = true;


		private static MWOverlay instance = null;

		public static float viewportLineWidth = 3.0f;
		public static float blinkingLineWidth = 1.0f;
		public static float viewportAlpha = 0.5f;
		public static float viewportLineAlpha = 0.5f;
		public static float blinkingLineAlpha = 1.0f;
		public static float blinkingSpeed = 0.25f;
		public static int viewportLineColor = 0xff0000;
		public static int blinkingLineColor = 0xff0000;
		public static int locationLineColor = 0x00ff00;
		public static float locationLineWidth = 4.0f;

		private static int lastOrientation = -1;
		private static int lastMask = -1;
		private static float lastLeft = -1;
		private static float lastTop = -1;
		private static float lastWidth = -1;
		private static float lastHeight = -1;
		private static int imageWidth = 1;
		private static int imageHeight = 1;

		public static UIView overlayView;

		public static void addToPreviewLayer(AVCaptureVideoPreviewLayer videoPreviewLayer)
		{
			viewportLayer = new CALayer ();
			viewportLayer.Frame = new RectangleF(0, 0, (float) videoPreviewLayer.Frame.Size.Width, (float) videoPreviewLayer.Frame.Size.Height);

			lineLayer = new CALayer ();
			lineLayer.Frame = new RectangleF(0, 0, (float) videoPreviewLayer.Frame.Size.Width, (float) videoPreviewLayer.Frame.Size.Height);

			locationLayer = new CALayer ();
			locationLayer.Frame = new RectangleF(0, 0, (float) videoPreviewLayer.Frame.Size.Width, (float) videoPreviewLayer.Frame.Size.Height);


			if (videoPreviewLayer != null) {
				videoPreviewLayer.AddSublayer (viewportLayer);
				videoPreviewLayer.AddSublayer (lineLayer);
			

				previewLayer = videoPreviewLayer;

				isAttached = true;

				instance = new MWOverlay ();


				instance.PerformSelector (new ObjCRuntime.Selector ("checkForChanges"), null, CHANGE_TRACKING_INTERVAL); 

				instance.BeginInvokeOnMainThread (() => {
					MWOverlay.updateOverlay ();
				});
			} else {
				isAttached = false;
			}

		}
		public static void addViewToPreviewLayer(UIView videoView)
		{
			if (overlayView != null){
				overlayView.Tag = 158;
				if (videoView.ViewWithTag(158) == null) { 
					videoView.AddSubview(overlayView);
				}
			}
		}

		public static void removeFromPreviewLayer() {

			if (overlayView != null) {
				overlayView.RemoveFromSuperview();
			}


			if (!isAttached){
				return;
			}

			if (previewLayer != null){
				if (lineLayer != null){
					lineLayer.RemoveFromSuperLayer();
				}
				if (viewportLayer != null){
					viewportLayer.RemoveFromSuperLayer();
				}
				if (locationLayer != null){
					locationLayer.RemoveFromSuperLayer();
				}
			}

			isAttached = false;

		}

		[Export ("checkForChanges")]
		public void checkForChanges() {

			if (isAttached){
				instance.PerformSelector (new ObjCRuntime.Selector ("checkForChanges"), null, CHANGE_TRACKING_INTERVAL); 
			} else {
				return;
			}

			float left, top, width, height;
			int res = BarcodeConfig.MWB_getScanningRect(0, out left, out top, out width, out height);

			if (res == 0){

				int orientation = BarcodeConfig.MWB_getDirection();

				if (orientation != lastOrientation || left != lastLeft || top != lastTop || width != lastWidth || height != lastHeight) {

					instance.BeginInvokeOnMainThread (() => {
						MWOverlay.updateOverlay();
					});

				}

				lastOrientation = orientation;
				lastLeft = left;
				lastTop = top;
				lastWidth = width;
				lastHeight = height;

			}


		}


		public static void updateOverlay(){

			if (!isAttached || (previewLayer == null) || previewLayer.Connection == null){
				return;
			}

			float yScale = 1.0f;
			float yOffset = 0.0f;
			float xScale = 1.0f;
			float xOffset = 0.0f;

			if (previewLayer.RespondsToSelector (new ObjCRuntime.Selector ("captureDevicePointOfInterestForPoint:"))) {
				CGPoint p1 = previewLayer.CaptureDevicePointOfInterestForPoint(new PointF(0,0));
				
				
				yScale = (float)(-1.0f/(1 + (p1.Y - 1)*2));
				yOffset = (1.0f - yScale) / 2.0f * 100f;
				
				xScale = (float)(-1.0f/(1 + (p1.X - 1)*2));
				xOffset = (float)((1.0f - xScale) / 2.0f * 100f);
				
				
				if (previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.Portrait || previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.PortraitUpsideDown){
					
					yScale = (float)(-1.0f/(1 + (p1.X - 1)*2));
					yOffset = (float)((1.0f - yScale) / 2.0f * 100f);
					
					xScale = (float)(-1.0f/(1 + (p1.Y - 1)*2));
					xOffset = (float)((1.0f - xScale) / 2.0f * 100f);
					
				}
			}



			viewportLayer.Hidden = !isViewportVisible;
			lineLayer.Hidden = !isBlinkingLineVisible;


			int overlayWidth = (int) viewportLayer.Frame.Size.Width;
			int overlayHeight = (int) viewportLayer.Frame.Size.Height;

			CGRect cgRect = viewportLayer.Frame;


			UIGraphics.BeginImageContext(cgRect.Size);
			CGContext context = UIGraphics.GetCurrentContext();

			//UIGraphics.PushContext(context);

			float left, top, width, height;

			BarcodeConfig.MWB_getScanningRect(0, out left, out top, out width, out height);

			if (previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.Portrait || previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.PortraitUpsideDown) {
				float tmp = left;
				left = top;
				top = tmp;
				tmp = height;
				height = width;
				width = tmp;
			}

			if(previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.LandscapeLeft){
				left = 100 - width - left;
				top = 100 - height - top;
			}else if(previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.Portrait){
				left = 100 - width - left;
			}else if(previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.PortraitUpsideDown){
				top = 100 - height - top;
			}



			RectangleF rect = new RectangleF(xOffset + left * xScale, yOffset + top * yScale, width * xScale, height * yScale);

			if (rect.Width < 0){
				rect.Width = - rect.Width;
				rect.X = 100 - rect.X;
			}

			if (rect.Height < 0){
				rect.Height = - rect.Height;
				rect.Y = 100 - rect.Y;
			}


			rect.X *= overlayWidth;
			rect.X /= 100.0f;
			rect.Y *= overlayHeight;
			rect.Y /= 100.0f;
			rect.Width *= overlayWidth;
			rect.Width /= 100.0f;
			rect.Height *= overlayHeight;
			rect.Height /= 100.0f;

			context.SetFillColor(0, 0, 0, viewportAlpha);
			context.FillRect(new RectangleF(0,0,overlayWidth,overlayHeight));
			context.ClearRect(rect);

			float r = (viewportLineColor >> 16) / 255.0f;
			float g = ((viewportLineColor & 0x00ff00) >> 8) / 255.0f;
			float b = (viewportLineColor & 0x0000ff) / 255.0f;


			context.SetStrokeColor(r, g, b, 0.5f);
			context.StrokeRectWithWidth(rect, viewportLineWidth);




			UIImage cgImage = UIGraphics.GetImageFromCurrentImageContext();
			viewportLayer.Contents = cgImage.CGImage;

			//UIGraphics.PopContext();

			context.ClearRect(cgRect);

			r = (blinkingLineColor >> 16) / 255.0f;
			g = ((blinkingLineColor & 0x00ff00) >> 8) / 255.0f;
			b = (blinkingLineColor & 0x0000ff) / 255.0f;

			context.SetStrokeColor(r, g, b, 1);

			int orientation = BarcodeConfig.MWB_getDirection();

			if (previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.Portrait || previewLayer.Connection.VideoOrientation == AVCaptureVideoOrientation.PortraitUpsideDown){

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


			if (((orientation & BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL) > 0) || ((orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0) || ((orientation & BarcodeConfig.MWB_SCANDIRECTION_AUTODETECT) > 0)){
				context.SetLineWidth(blinkingLineWidth);
				context.MoveTo(rect.X, rect.Y + rect.Height / 2);
				context.AddLineToPoint(rect.X + rect.Width, rect.Y + rect.Height / 2);
				context.StrokePath();
			}

			if (((orientation & BarcodeConfig.MWB_SCANDIRECTION_VERTICAL) > 0) || ((orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0) || ((orientation & BarcodeConfig.MWB_SCANDIRECTION_AUTODETECT) > 0)){

				context.MoveTo(rect.X + rect.Width / 2, rect.Y);
				context.AddLineToPoint(rect.X + rect.Width / 2, rect.Y + rect.Height);
				context.StrokePath();
			}

			if (((orientation & BarcodeConfig.MWB_SCANDIRECTION_OMNI) > 0) || ((orientation & BarcodeConfig.MWB_SCANDIRECTION_AUTODETECT) > 0)){
				context.MoveTo(rect.X , rect.Y);
				context.AddLineToPoint(rect.X + rect.Width , rect.Y + rect.Height);
				context.StrokePath();

				context.MoveTo(rect.X + rect.Width, rect.Y);
				context.AddLineToPoint(rect.X , rect.Y + rect.Height);
				context.StrokePath();
			}

			lineLayer.Contents = UIGraphics.GetImageFromCurrentImageContext().CGImage;

			UIGraphics.EndImageContext();

			MWOverlay.startLineAnimation();


		}

		
		public static void showLocation(CoreGraphics.CGPoint[] points, int width, int height)
		{
			imageWidth = width;
			imageHeight = height;

			if (points == null) {
				return;
			}
			if (!isAttached || previewLayer == null) {
				return;
			}

			locationLayer.RemoveAllAnimations ();
			previewLayer.AddSublayer (locationLayer);

			CGRect cgRect = locationLayer.Frame;

			UIGraphics.BeginImageContext (cgRect.Size);
			var context = UIGraphics.GetCurrentContext ();
			UIGraphics.PushContext (context);

			context.ClearRect (cgRect);

			float r = (locationLineColor >> 16) / 255.0f;
			float g = ((locationLineColor & 0x00ff00) >> 8) / 255.0f;
			float b = (locationLineColor & 0x0000ff) / 255.0f;

			context.SetStrokeColor (r, g, b, 1);

			context.SetLineWidth (locationLineWidth);

			for (int i = 0; i < 4; i++){
				points[i].X /= (float)imageWidth;
				points[i].Y /= (float)imageHeight;

				points [i] = previewLayer.PointForCaptureDevicePointOfInterest (points [i]);
			}

			context.MoveTo (points [0].X , points [0].Y);

			for (int i = 1; i < 4; i++){
				context.AddLineToPoint (points [i].X, points [i].Y);
			}

			context.AddLineToPoint (points [0].X, points [0].Y);

			context.StrokePath ();

			UIGraphics.PopContext ();

			locationLayer.Contents = UIGraphics.GetImageFromCurrentImageContext ().CGImage;

			UIGraphics.EndImageContext ();

			CABasicAnimation animation = CABasicAnimation.FromKeyPath ("opacity");
			animation.SetFrom (NSNumber.FromFloat(1));
			animation.SetTo (NSNumber.FromFloat(0));
			animation.Duration = 0.5f;
			animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseOut);
			animation.FillMode =  CAFillMode.Forwards;
			animation.RemovedOnCompletion = false;
			locationLayer.AddAnimation (animation, "opacity");

		}

		public static void startLineAnimation() {
			lineLayer.RemoveAllAnimations();
			CABasicAnimation animation = CABasicAnimation.FromKeyPath("opacity");
			animation.From = NSObject.FromObject(blinkingLineAlpha);
			animation.To = NSObject.FromObject(0.0);
			animation.Duration = blinkingSpeed;
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.Linear);
			animation.AutoReverses = true;
			animation.RepeatCount = 99999999999;
			lineLayer.AddAnimation(animation,"opacity");
		}

	}
}
	