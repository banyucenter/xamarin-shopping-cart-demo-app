using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ManateeShoppingCart;
using ManateeShoppingCart.Droid;
using Android.App;
using Android.Hardware;
using Android.Views;
using Android.Graphics;
using Android.Widget;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ManateeShoppingCart.Droid.MWBarcodeScanner;
using System.Drawing;

[assembly: ExportRenderer(typeof(ScanPage), typeof(ScanPageRenderer))]
namespace ManateeShoppingCart.Droid
{
    public class ScanPageRenderer : PageRenderer
    {
        ObservableCollection<ItemModel> listItems;
        int editListIndex;
        int editItemIndex = -1;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            listItems = ((ScanPage)Element).AllItems;
            try { editListIndex = int.Parse(((ScanPage)Element).editListIndex.ToString()); } catch { }
            try { editItemIndex = int.Parse(((ScanPage)Element).editItemIndex.ToString()); } catch { }

            try
            {
                customDecoderInit();

                Scanner scanner = new Scanner(Forms.Context);
                scanner.setInterfaceOrientation("All");

                ScannerResult result = null;

                var scanIntent = new Android.Content.Intent(scanner.mContext, typeof(ScannerActivity));

                ScannerActivity.successCallback = null;
                ScannerActivity.OnScanCompleted += (ScannerResult sresult) =>
                {
                    result = sresult;

                    if (result != null)
                    {
                        try
                        {
                            if (editItemIndex >= 0)
                            {
                                listItems[editItemIndex].BarcodeResult = result.code;
                                listItems[editItemIndex].BarcodeType = result.type;
                            }
                            else
                            {
                                ItemModel itemResult = new ItemModel();

                                itemResult.ID = 1;
                                if (listItems.Count > 0)
                                    itemResult.ID = ((int)listItems.Max(x => x.ID)) + 1;

                                itemResult.Name = "";
                                itemResult.BarcodeResult = result.code;
                                itemResult.BarcodeType = result.type;

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
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }

                    if (Element != null)
                        Element.Navigation.PopModalAsync();
                };

                scanner.mContext.StartActivity(scanIntent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
            }
        }

        public static RectangleF RECT_LANDSCAPE_1D = new RectangleF(6, 20, 88, 60);
        public static RectangleF RECT_LANDSCAPE_2D = new RectangleF(20, 6, 60, 88);
        public static RectangleF RECT_PORTRAIT_1D = new RectangleF(20, 6, 60, 88);
        public static RectangleF RECT_PORTRAIT_2D = new RectangleF(20, 6, 60, 88);
        public static RectangleF RECT_FULL_1D = new RectangleF(6, 6, 88, 88);
        public static RectangleF RECT_FULL_2D = new RectangleF(20, 6, 60, 88);

        public static RectangleF RECT_DOTCODE = new RectangleF(30, 20, 40, 60);



        public void customDecoderInit()
        {

            Console.WriteLine("Decoder initialization");
            //register your copy of library with givern user/password
            int registrationResult = BarcodeConfig.MWB_registerSDK("key");

            switch (registrationResult)
            {
                case BarcodeConfig.MWB_RTREG_OK:
                    Console.WriteLine("Registration OK");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_KEY:
                    Console.WriteLine("Registration Invalid Key");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_CHECKSUM:
                    Console.WriteLine("Registration Invalid Checksum");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_APPLICATION:
                    Console.WriteLine("Registration Invalid Application");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_SDK_VERSION:
                    Console.WriteLine("Registration Invalid SDK Version");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_KEY_VERSION:
                    Console.WriteLine("Registration Invalid Key Version");
                    break;
                case BarcodeConfig.MWB_RTREG_INVALID_PLATFORM:
                    Console.WriteLine("Registration Invalid Platform");
                    break;
                case BarcodeConfig.MWB_RTREG_KEY_EXPIRED:
                    Console.WriteLine("Registration Key Expired");
                    break;
                default:
                    break;
            }

            // choose code type or types you want to search for

            // Our sample app is configured by default to search all supported barcodes...
            BarcodeConfig.MWB_setActiveCodes(
                BarcodeConfig.MWB_CODE_MASK_25 |
                BarcodeConfig.MWB_CODE_MASK_39 |
                BarcodeConfig.MWB_CODE_MASK_93 |
                BarcodeConfig.MWB_CODE_MASK_128 |
                BarcodeConfig.MWB_CODE_MASK_AZTEC |
                BarcodeConfig.MWB_CODE_MASK_DM |
                BarcodeConfig.MWB_CODE_MASK_EANUPC |
                BarcodeConfig.MWB_CODE_MASK_PDF |
                BarcodeConfig.MWB_CODE_MASK_QR |
                BarcodeConfig.MWB_CODE_MASK_RSS |
                BarcodeConfig.MWB_CODE_MASK_CODABAR |
                BarcodeConfig.MWB_CODE_MASK_DOTCODE |
                BarcodeConfig.MWB_CODE_MASK_11 |
                BarcodeConfig.MWB_CODE_MASK_MSI |
                BarcodeConfig.MWB_CODE_MASK_MAXICODE |
                BarcodeConfig.MWB_CODE_MASK_POSTAL
            );

            // But for better performance, only activate the symbologies your application requires...
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_25 );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_39 );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_93 );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_128 );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_AZTEC );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_DM );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_EANUPC );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_PDF );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_QR );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_RSS );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_CODABAR );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_DOTCODE );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_11 );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_MSI );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_MAXICODE );
            // BarcodeConfig.MWB_setActiveCodes( BarcodeConfig.MWB_CODE_MASK_POSTAL );


            // Our sample app is configured by default to search both directions...
            BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL | BarcodeConfig.MWB_SCANDIRECTION_VERTICAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM, RECT_FULL_2D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR, RECT_FULL_2D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);


            // But for better performance, set like this for PORTRAIT scanning...
            // BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_VERTICAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);


            // or like this for LANDSCAPE scanning - Preferred for dense or wide codes...
            // BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL);
            // set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE, RECT_DOTCODE);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI, RECT_FULL_1D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
            //BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);



            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_25, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_MSI, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_39, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_CODABAR, 5);
            BarcodeConfig.MWB_setMinLength(BarcodeConfig.MWB_CODE_MASK_11, 5);

            BarcodeConfig.MWPsetParserMask(BarcodeConfig.MWP_PARSER_MASK_AUTO);

            //BarcodeConfig.MWB_setDuplicatesTimeout (2);
            // set decoder effort level (1 - 5)
            // for live scanning scenarios, a setting between 1 to 3 will suffice
            // levels 4 and 5 are typically reserved for batch scanning
            BarcodeConfig.MWB_setLevel(2);

            //get and print Library version
            int ver = BarcodeConfig.MWB_getLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            String libVersion = v1.ToString() + "." + v2.ToString() + "." + v3.ToString();
            Console.WriteLine("Lib version: " + libVersion);
        }
    }
}