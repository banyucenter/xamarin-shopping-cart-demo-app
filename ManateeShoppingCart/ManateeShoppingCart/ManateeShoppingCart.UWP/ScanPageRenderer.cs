using ManateeShoppingCart;
using ManateeShoppingCart.UWP;
using MWBarcodeScanner;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Xamarin.Forms.Platform.UWP;


[assembly: ExportRenderer(typeof(ScanPage), typeof(ScanPageRenderer))]
namespace ManateeShoppingCart.UWP
{
    public class ScanPageRenderer : PageRenderer
    {
        ScannerPage scanner;

        int editListIndex;
        int editItemIndex = -1;
        ObservableCollection<ItemModel> listItems;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            listItems = ((ScanPage)Element).AllItems;
            try { editListIndex = int.Parse(((ScanPage)Element).editListIndex.ToString()); } catch { }
            try { editItemIndex = int.Parse(((ScanPage)Element).editItemIndex.ToString()); } catch { }

            try
            {
                customDecoderInit();

                scanner = new ScannerPage();

                /*
			    * Customize the scanner by using options below
			    */

                /* Select desired scanner interface orientation
			    * Available options are: LandscapeLeft, LandscapeRight, Portrait, All; Default: LandscapeLeft
			    */
                //scanner.setInterfaceOrientation("All");

                /* Toggle visibility of Flash button
			    * Available options are: true, false; Default: true
			    */
                //scanner.useFlash = false;

                /* The initial state of the camera flash
			    * Available options are: true, false; Default: false
			    */
                //scanner.flashOnByDefault(true);

                /* Choose between overlay types
			    * Available options are: OM_NONE, OM_MWOVERLAY (dynamic viewfinder), OM_IMAGE (static image overlay); Default: OM_MWOVERLAY
			    */
                //scanner.overlayMode = (int)ScannerPage.OverlayMode.OM_NONE;

                /* Toggle visibility of Zoom button
			    * Available options are: true, false; Default: true
			    */
                //scanner.useZoom = false;

                /* Set the number of CPU cores to be used
			    * Available options are: 1,2,3..; Max = Environment.ProcessorCount; Default: Max;
			    */
                //scanner.setMaxThreads(2);

                /* Close scanner after successful scan 
			    * Available options are: true, false (continuous scanning if set to false); Default: true; 
			    * if set to false:
			    * 		Use scanner.resumeScanning() - to resume after successful scan
			    * 		Use scanner.closeScanner()   - to close the scanner
			    */
                //scanner.closeScannerOnDecode = false;

                /* Show on screen location of scanned code
			    * Available options are: true, false; Default: true;
			    * 
			    * Customise line width and line color with the provided params below
			    */
                //scanner.enableShowLocation = false;
                //MWOverlay.locationLineColor = 0x008000;
                //MWOverlay.locationLineWidth = 2;

                /* 
			    * Use the front camera of the device
			    * 
			    * Available options are: true, false; Default: false;
			    * 
			    */
                //scanner.useFrontCamera = true;

                ScannerPage.OnScanCompleted += SaveScanResult;

                SetNativeControl(scanner);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"      ERROR: ", ex.Message);
            }
        }

        private async void SaveScanResult(MWResult result)
        {
            try
            {
                if (result != null)
                {
                    if (editItemIndex >= 0)
                    {
                        listItems[editItemIndex].BarcodeResult = result.text;
                        listItems[editItemIndex].BarcodeType = BarcodeHelper.getBarcodeName(result.type);
                    }
                    else
                    {
                        ItemModel itemResult = new ItemModel();

                        itemResult.ID = 1;
                        if (listItems.Count > 0)
                            itemResult.ID = ((int)listItems.Max(x => x.ID)) + 1;

                        itemResult.Name = "";
                        itemResult.BarcodeResult = result.text;
                        itemResult.BarcodeType = BarcodeHelper.getBarcodeName(result.type);

                        listItems.Insert(0, itemResult);
                    }

                    if (Xamarin.Forms.Application.Current.Properties.ContainsKey("AllLists"))
                    {
                        string jsonList = Xamarin.Forms.Application.Current.Properties["AllLists"].ToString();
                        ObservableCollection<ListsModel> tempList = JsonConvert.DeserializeObject<ObservableCollection<ListsModel>>(jsonList);

                        tempList[editListIndex].Items = listItems;

                        Xamarin.Forms.Application.Current.Properties["AllLists"] = JsonConvert.SerializeObject(tempList);
                    }
                }

                //On Xamarin navigation should go here
                if (Element.Navigation.ModalStack.Count > 0)
                    await Element.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            try
            {
                scanner.Arrange(new Windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArrangeOverride Error: " + ex.Message);
            }
            return finalSize;
        }

        public static Rect RECT_LANDSCAPE_1D = new Rect(6, 20, 88, 60);
        public static Rect RECT_LANDSCAPE_2D = new Rect(20, 6, 60, 88);
        public static Rect RECT_PORTRAIT_1D = new Rect(20, 6, 60, 88);
        public static Rect RECT_PORTRAIT_2D = new Rect(20, 6, 60, 88);
        public static Rect RECT_FULL_1D = new Rect(6, 6, 88, 88);
        public static Rect RECT_FULL_2D = new Rect(20, 6, 60, 88);
        public static Rect RECT_DOTCODE = new Rect(30, 20, 40, 60);

        public static void customDecoderInit()
        {
            Debug.WriteLine("Decoder initialization");

            //register your copy of library with given SDK key
            int registerResult = BarcodeHelper.MWBregisterSDK("SDKkey");

            if (registerResult == BarcodeHelper.MWB_RTREG_OK)
            {
                Debug.WriteLine("Registration OK");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_KEY)
            {
                Debug.WriteLine("Registration Invalid Key");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_CHECKSUM)
            {
                Debug.WriteLine("Registration Invalid Checksum");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_APPLICATION)
            {
                Debug.WriteLine("Registration Invalid Application");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_SDK_VERSION)
            {
                Debug.WriteLine("Registration Invalid SDK Version");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_KEY_VERSION)
            {
                Debug.WriteLine("Registration Invalid Key Version");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_INVALID_PLATFORM)
            {
                Debug.WriteLine("Registration Invalid Platform");
            }
            else
                if (registerResult == BarcodeHelper.MWB_RTREG_KEY_EXPIRED)
            {
                Debug.WriteLine("Registration Key Expired");
            }

            // choose code type or types you want to search for

            // Our sample app is configured by default to search all supported barcodes...
            BarcodeHelper.MWBsetActiveCodes(BarcodeHelper.MWB_CODE_MASK_25 |
                BarcodeHelper.MWB_CODE_MASK_39 |
                BarcodeHelper.MWB_CODE_MASK_93 |
                BarcodeHelper.MWB_CODE_MASK_128 |
                BarcodeHelper.MWB_CODE_MASK_AZTEC |
                BarcodeHelper.MWB_CODE_MASK_DM |
                BarcodeHelper.MWB_CODE_MASK_EANUPC |
                BarcodeHelper.MWB_CODE_MASK_PDF |
                BarcodeHelper.MWB_CODE_MASK_QR |
                BarcodeHelper.MWB_CODE_MASK_CODABAR |
                BarcodeHelper.MWB_CODE_MASK_RSS |
                BarcodeHelper.MWB_CODE_MASK_MAXICODE |
                BarcodeHelper.MWB_CODE_MASK_DOTCODE |
                BarcodeHelper.MWB_CODE_MASK_11 |
                BarcodeHelper.MWB_CODE_MASK_MSI |
                BarcodeHelper.MWB_CODE_MASK_POSTAL);

            // But for better performance, only activate the symbologies your application requires...
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_25 );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_39 );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_93 );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_128 );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_AZTEC );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_DM );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_EANUPC );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_PDF );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_QR );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_CODABAR );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_RSS );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_MAXICODE );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_DOTCODE );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_11 );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_MSI );
            // BarcodeHelper.MWB_setActiveCodes( BarcodeHelper.MWB_CODE_MASK_POSTAL );


            // Our sample app is configured by default to search both directions...
            BarcodeHelper.MWBsetDirection((uint)BarcodeHelper.MWB_SCANDIRECTION_HORIZONTAL | (uint)BarcodeHelper.MWB_SCANDIRECTION_VERTICAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_25, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_39, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_93, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_128, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DM, RECT_FULL_2D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_QR, RECT_FULL_2D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_11, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);

            // But for better performance, set like this for PORTRAIT scanning...
            // BarcodeHelper.MWB_setDirection(BarcodeHelper.MWB_SCANDIRECTION_VERTICAL);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_25, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_39, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_93, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_128, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DM, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_QR, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_11, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);


            // or like this for LANDSCAPE scanning - Preferred for dense or wide codes...
            // BarcodeHelper.MWB_setDirection(BarcodeHelper.MWB_SCANDIRECTION_HORIZONTAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_25, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_39, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_93, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_128, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DM, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_QR, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_11, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            //BarcodeHelper.MWBsetScanningRect(BarcodeHelper.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);

            BarcodeHelper.MWBsetMinLength(BarcodeHelper.MWB_CODE_MASK_25, 5);
            BarcodeHelper.MWBsetMinLength(BarcodeHelper.MWB_CODE_MASK_MSI, 5);
            BarcodeHelper.MWBsetMinLength(BarcodeHelper.MWB_CODE_MASK_39, 5);
            BarcodeHelper.MWBsetMinLength(BarcodeHelper.MWB_CODE_MASK_CODABAR, 5);
            BarcodeHelper.MWBsetMinLength(BarcodeHelper.MWB_CODE_MASK_11, 5);

            BarcodeHelper.MWBsetResultType(BarcodeHelper.MWB_RESULT_TYPE_MW);


            // set decoder effort level (1 - 5)
            // for live scanning scenarios, a setting between 1 to 3 will suffice
            // levels 4 and 5 are typically reserved for batch scanning
            BarcodeHelper.MWBsetLevel(2);

            //get and print Library version
            int ver = BarcodeHelper.MWBgetLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            String libVersion = v1.ToString() + "." + v2.ToString() + "." + v3.ToString();
            Debug.WriteLine("Lib version: " + libVersion);
        }
    }
}
