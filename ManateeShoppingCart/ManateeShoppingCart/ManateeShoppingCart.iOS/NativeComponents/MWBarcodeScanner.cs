using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using UIKit;
using System.Threading;
using CoreGraphics;
using AVFoundation;

namespace ManateeShoppingCart.iOS.MWBarcodeScanner
{


    public class BarcodeConfig
    {

        /**
		* @name Basic return values for API functions
		* @{
		*/
        public static int MWB_RT_OK = 0;
        public static int MWB_RT_FAIL = -1;
        public static int MWB_RT_NOT_SUPPORTED = -2;
        public static int MWB_RT_BAD_PARAM = -3;

        /** @brief  Code39 decoder flags value: select VIN mode
		*/
        public static int MWB_CFG_CODE39_VIN_MODE = 0x1;

        /** @brief  Code39 decoder flags value: require checksum check
		*/
        public static int MWB_CFG_CODE39_REQUIRE_CHECKSUM = 0x2;
        /**/


        /** @brief  Code39 decoder flags value: don't require stop symbol - can lead to false results
		*/
        public static int MWB_CFG_CODE39_DONT_REQUIRE_STOP = 0x4;
        /**/

        /** @brief  Code39 decoder flags value: decode full ASCII
		*/
        public static int MWB_CFG_CODE39_EXTENDED_MODE = 0x8;
        /**/

        /** @brief  Code93 decoder flags value: decode full ASCII
		*/
        public static int MWB_CFG_CODE93_EXTENDED_MODE = 0x8;
        /**/

        /** @brief  UPC/EAN decoder disable addons detection
 		*/
        public static int MWB_CFG_EANUPC_DISABLE_ADDON = 0x1;
        /**/

        /** @brief  Code25 decoder flags value: require checksum check
		*/
        public static int MWB_CFG_CODE25_REQ_CHKSUM = 0x1;
        /**/

        /** @brief  Code11 decoder flags value: require checksum check
		*  MWB_CFG_CODE11_REQ_SINGLE_CHKSUM is set by default
		*/
        public static int MWB_CFG_CODE11_REQ_SINGLE_CHKSUM = 0x1;
        public static int MWB_CFG_CODE11_REQ_DOUBLE_CHKSUM = 0x2;
        /**/

        /** @brief  MSI Plessey decoder flags value: require checksum check
		*  MWB_CFG_MSI_REQ_10_CHKSUM is set by default
		*/
        public static int MWB_CFG_MSI_REQ_10_CHKSUM = 0x01;
        public static int MWB_CFG_MSI_REQ_1010_CHKSUM = 0x02;
        public static int MWB_CFG_MSI_REQ_11_IBM_CHKSUM = 0x04;
        public static int MWB_CFG_MSI_REQ_11_NCR_CHKSUM = 0x08;
        public static int MWB_CFG_MSI_REQ_1110_IBM_CHKSUM = 0x10;
        public static int MWB_CFG_MSI_REQ_1110_NCR_CHKSUM = 0x20;
        /**/

        /** @brief  Codabar decoder flags value: include start/stop symbols in result
		*/
        public static int MWB_CFG_CODABAR_INCLUDE_STARTSTOP = 0x1;
        /**/

        /** @brief  Barcode decoder param types
 		*/
        public static int MWB_PAR_ID_ECI_MODE = 0x08;
        public static int MWB_PAR_ID_RESULT_PREFIX = 0x10;
        /**/

        /** @brief  Barcode param values
 		*/

        public static int MWB_PAR_VALUE_ECI_DISABLED = 0x00; //default
        public static int MWB_PAR_VALUE_ECI_ENABLED = 0x01;

        public static int MWB_PAR_VALUE_RESULT_PREFIX_NEVER = 0x00; // default
        public static int MWB_PAR_VALUE_RESULT_PREFIX_ALWAYS = 0x01;
        public static int MWB_PAR_VALUE_RESULT_PREFIX_DEFAULT = 0x02;
        /**/

        /** @brief  Global decoder flags value: apply sharpening on input image
		*/
        public static int MWB_CFG_GLOBAL_HORIZONTAL_SHARPENING = 0x1;
        public static int MWB_CFG_GLOBAL_VERTICAL_SHARPENING = 0x2;
        public static int MWB_CFG_GLOBAL_SHARPENING = 0x3;

        /**
		* @name Bit mask identifiers for supported decoder types
		* @{ */
        public static int MWB_CODE_MASK_NONE = 0x00000000;
        public static int MWB_CODE_MASK_QR = 0x00000001;
        public static int MWB_CODE_MASK_DM = 0x00000002;
        public static int MWB_CODE_MASK_RSS = 0x00000004;
        public static int MWB_CODE_MASK_39 = 0x00000008;
        public static int MWB_CODE_MASK_EANUPC = 0x00000010;
        public static int MWB_CODE_MASK_128 = 0x00000020;
        public static int MWB_CODE_MASK_PDF = 0x00000040;
        public static int MWB_CODE_MASK_AZTEC = 0x00000080;
        public static int MWB_CODE_MASK_25 = 0x00000100;
        public static int MWB_CODE_MASK_93 = 0x00000200;
        public static int MWB_CODE_MASK_CODABAR = 0x00000400;
        public static int MWB_CODE_MASK_DOTCODE = 0x00000800;
        public static int MWB_CODE_MASK_11 = 0x00001000;
        public static int MWB_CODE_MASK_MSI = 0x00002000;
        public static int MWB_CODE_MASK_MAXICODE = 0x00004000;
        public static int MWB_CODE_MASK_POSTAL = 0x00008000;
        public static int MWB_CODE_MASK_ALL = 0x0fffffff;
        /** @} */



        public const int MWB_RTREG_OK = 0;
        public const int MWB_RTREG_INVALID_KEY = -1;
        public const int MWB_RTREG_INVALID_CHECKSUM = -2;
        public const int MWB_RTREG_INVALID_APPLICATION = -3;
        public const int MWB_RTREG_INVALID_SDK_VERSION = -4;
        public const int MWB_RTREG_INVALID_KEY_VERSION = -5;
        public const int MWB_RTREG_INVALID_PLATFORM = -6;
        public const int MWB_RTREG_KEY_EXPIRED = -7;



        /**
		* @name Bit mask identifiers for RSS decoder types
			* @{ */
        public static int MWB_SUBC_MASK_RSS_14 = 0x00000001;
        public static int MWB_SUBC_MASK_RSS_14_STACK = 0x00000002;
        public static int MWB_SUBC_MASK_RSS_LIM = 0x00000004;
        public static int MWB_SUBC_MASK_RSS_EXP = 0x00000008;
        /** @} */
        /**

		 * @name Bit mask identifiers for QR decoder types
		 * @{ */
        public static int MWB_SUBC_MASK_QR_STANDARD = 0x00000001;
        public static int MWB_SUBC_MASK_QR_MICRO = 0x00000002;
        /** @} */


        /**
        * @name Bit mask identifiers for Code 2 of 5 decoder types
            * @{ */
        public static int MWB_SUBC_MASK_C25_INTERLEAVED = 0x00000001;
        public static int MWB_SUBC_MASK_C25_STANDARD = 0x00000002;
        public static int MWB_SUBC_MASK_C25_ITF14 = 0x00000004;
        public static int MWB_SUBC_MASK_C25_IATA = 0x00000008;
        public static int MWB_SUBC_MASK_C25_MATRIX = 0x00000010;
        public static int MWB_SUBC_MASK_C25_COOP = 0x00000020;
        public static int MWB_SUBC_MASK_C25_INVERTED = 0x00000040;


        /** @} */

        /**
        * @name Bit mask identifiers for POSTAL decoder types
            * @{ */
        public static int MWB_SUBC_MASK_POSTAL_POSTNET = 0x00000001;
        public static int MWB_SUBC_MASK_POSTAL_PLANET = 0x00000002;
        public static int MWB_SUBC_MASK_POSTAL_IM = 0x00000004;
        public static int MWB_SUBC_MASK_POSTAL_ROYAL = 0x00000008;

        /** @} */


        /**
		* @name Bit mask identifiers for UPC/EAN decoder types
			* @{ */
        public static int MWB_SUBC_MASK_EANUPC_EAN_13 = 0x00000001;
        public static int MWB_SUBC_MASK_EANUPC_EAN_8 = 0x00000002;
        public static int MWB_SUBC_MASK_EANUPC_UPC_A = 0x00000004;
        public static int MWB_SUBC_MASK_EANUPC_UPC_E = 0x00000008;
        public static int MWB_SUBC_MASK_EANUPC_UPC_E1 = 0x00000010;
        /** @} */

        /**
        * @name Bit mask identifiers for 1D scanning direction 
            * @{ */
        public static int MWB_SCANDIRECTION_HORIZONTAL = 0x00000001;
        public static int MWB_SCANDIRECTION_VERTICAL = 0x00000002;
        public static int MWB_SCANDIRECTION_OMNI = 0x00000004;
        public static int MWB_SCANDIRECTION_AUTODETECT = 0x00000008;
        /** @} */

        public const int FOUND_NONE = 0;
        public const int FOUND_DM = 1;
        public const int FOUND_39 = 2;
        public const int FOUND_RSS_14 = 3;
        public const int FOUND_RSS_14_STACK = 4;
        public const int FOUND_RSS_LIM = 5;
        public const int FOUND_RSS_EXP = 6;
        public const int FOUND_EAN_13 = 7;
        public const int FOUND_EAN_8 = 8;
        public const int FOUND_UPC_A = 9;
        public const int FOUND_UPC_E = 10;
        public const int FOUND_128 = 11;
        public const int FOUND_PDF = 12;
        public const int FOUND_QR = 13;
        public const int FOUND_AZTEC = 14;
        public const int FOUND_25_INTERLEAVED = 15;
        public const int FOUND_25_STANDARD = 16;
        public const int FOUND_93 = 17;
        public const int FOUND_CODABAR = 18;
        public const int FOUND_DOTCODE = 19;
        public const int FOUND_128_GS1 = 20;
        public const int FOUND_ITF14 = 21;
        public const int FOUND_11 = 22;
        public const int FOUND_MSI = 23;
        public const int FOUND_25_IATA = 24;
        public const int FOUND_25_MATRIX = 25;
        public const int FOUND_25_COOP = 26;
        public const int FOUND_25_INVERTED = 27;
        public const int FOUND_QR_MICRO = 28;
        public const int FOUND_MAXICODE = 29;
        public const int FOUND_POSTNET = 30;
        public const int FOUND_PLANET = 31;
        public const int FOUND_IMB = 32;
        public const int FOUND_ROYALMAIL = 33;

        /**
        * @name Identifiers for result types
        * @{ */

        public const int MWB_RESULT_TYPE_RAW = 0x00000001;
        public const int MWB_RESULT_TYPE_MW = 0x00000002;
        //public const int MWB_RESULT_TYPE_JSON =               0x00000003; // not supported yet

        /** @} */

        /**
        * @name Identifiers for result fields types
            * @{ */
        public const int MWB_RESULT_FT_BYTES = 0x00000001;
        public const int MWB_RESULT_FT_TEXT = 0x00000002;
        public const int MWB_RESULT_FT_TYPE = 0x00000003;
        public const int MWB_RESULT_FT_SUBTYPE = 0x00000004;
        public const int MWB_RESULT_FT_SUCCESS = 0x00000005;
        public const int MWB_RESULT_FT_ISGS1 = 0x00000006;
        public const int MWB_RESULT_FT_LOCATION = 0x00000007;
        public const int MWB_RESULT_FT_IMAGE_WIDTH = 0x00000008;
        public const int MWB_RESULT_FT_IMAGE_HEIGHT = 0x00000009;
        public const int MWB_RESULT_FT_PARSER_BYTES = 0x0000000A;

        public const int MWB_RESULT_FT_MODULES_COUNT_X = 0x0000000B;
        public const int MWB_RESULT_FT_MODULES_COUNT_Y = 0x0000000C;
        public const int MWB_RESULT_FT_MODULE_SIZE_X = 0x0000000D;
        public const int MWB_RESULT_FT_MODULE_SIZE_Y = 0x0000000E;
        public const int MWB_RESULT_FT_SKEW = 0x0000000F;
        public const int MWB_RESULT_FT_KANJI = 0x00000010;
        /** @} */

        /** @brief  Global decoder flags value: calculate location for 1D barcodeTypes (Code128, Code93, Code39 supported)
        */
        public const int MWB_CFG_GLOBAL_CALCULATE_1D_LOCATION = 0x10;
        /** @} */

        /** @brief  Global decoder flags value: fail 1D decode if result is not confirmed by location expanding (Code128, Code93, Code39 supported)
        */
        public const int MWB_CFG_GLOBAL_VERIFY_1D_LOCATION = 0x20;
        /** @} */


        /** @brief  Global decoder flags value: fail decode if result is not touching the center of viewfinder (2D + Code128, Code93, Code39 supported)
        * 1D locaiton flags will be enabled automatically with this one
        */
        public const int MWB_CFG_GLOBAL_USE_CENTRIC_SCANNING = 0x40;
        /** @} */

        /** @brief  Code39 decoder flags value: require checksum check
        */
        public const int MWB_CFG_CODE39_REQ_CHKSUM = 0x2;
        /**/


        /**
         * @name Bit mask identifiers for supported decoder types
         * @{ */
        public const int MWP_PARSER_MASK_NONE = 0x00000000;
        public const int MWP_PARSER_MASK_GS1 = 0x00000001;
        public const int MWP_PARSER_MASK_IUID = 0x00000002;
        public const int MWP_PARSER_MASK_ISBT = 0x00000004;
        public const int MWP_PARSER_MASK_AAMVA = 0x00000008;
        public const int MWP_PARSER_MASK_HIBC = 0x00000010;
        public const int MWP_PARSER_MASK_SCM = 0x00000020;
        public const int MWP_PARSER_MASK_AUTO = 0x0fffffff;

        /** @} */


        //UID PARSER ERROR CODES
        public const double UID_ERROR_INVALID_HEADER = -1;
        public const double UID_ERROR_INVALID_FORMAT = -2;
        public const double UID_ERROR_INVALID_EI = -3.0;
        public const double UID_ERROR_INVALID_CAGE = -3.1;
        public const double UID_ERROR_INVALID_DUNS = -3.2;
        public const double UID_ERROR_INVALID_DODAAC = -3.3;
        public const double UID_ERROR_INVALID_GS1COMP = -3.4;
        public const double UID_ERROR_INVALID_PN = -4;
        public const double UID_ERROR_INVALID_SN = -5;
        public const double UID_ERROR_INVALID_UII = -6;
        public const double UID_ERROR_INVALID_LOT = -7;
        public const double UID_ERROR_GS_MISSING = -8;     //GS Missing after Header
        public const double UID_ERROR_RS_MISSING = -9;
        public const double UID_ERROR_EOT_MISSING = -10;
        public const double UID_ERROR_NO_SN = -11;
        public const double UID_ERROR_NO_EI = -12;
        public const double UID_ERROR_NO_PN = -13;
        public const double UID_ERROR_NO_LOT = -14;
        public const double UID_ERROR_DUPLICATE_DQ = -15;
        public const double UID_ERROR_DUPLICATE_UII = -16;
        public const double UID_ERROR_DUPLICATE_LOT = -17;
        public const double UID_ERROR_DUPLICATE_SN = -18;
        public const double UID_ERROR_DUPLICATE_EI = -19;
        public const double UID_ERROR_LOT_PN_CONFLICT = -20;
        public const double UID_ERROR_MISSING_REQ = -21;
        public const double UID_ERROR_INVALID_IAC = -22;
        public const double UID_ERROR_INVALID_TEI = -23;

        //UID PARSER WARNING CODES
        public const double UID_WARN_EXTRA_CHARS = 91;      //characters after EOT
        public const double UID_WARN_UNNEEDED_DATA = 92;      //unneeded additional data
        public const double UID_WARN_SPACE_AROUND = 93;      //space at the beginning or end of the uid
        public const double UID_WARN_UNKNOWN_DQ = 94;
        public const double UID_WARN_OBSOLETE_FORMAT = 95;      //warning for DD


        //AAMVA PARSER ERROR CODES
        public const double AAMVA_ERROR_INVALID_FORMAT = -1;
        public const double AAMVA_ERROR_INVALID_HEADER = -2;
        public const double AAMVA_ERROR_INVALID_IIN = -3;
        public const double NOT_ENOUGHT_MEMORY = -4;
        public const double AAMVA_ERROR_INVALID_JN = -5;
        public const double AAMVA_ERROR_INVALID_NENTIRES = -6;


        //AAMVA PARSER WARNING CODES
        public const double AAMVA_WARNING_MISSING_MANDATORY_FIELDS = 1;


        //UPS/SCM PARSER ERROR CODES
        public const double SCM_ERROR_INVALID_FORMAT = -1;
        public const double SCM_ERROR_INVALID_CODE = -2;
        public const double SCM_ERROR_ELEMENT_NOT_FOUND = -3;
        public const double SCM_ERROR_CANT_ALLOCATE_MEMORY = -4;

        //UPS/SCM PARSER WARNING CODES
        public const double SCM_WARNING_LENGTH_OUT_OF_BOUNDS = 1;   //possible compression used by UPS
        public const double SCM_WARNING_FIELD_EXCEEDS_MAX_LENGTH = 2;   //possible compression used by UPS
        public const double SCM_WARNING_INVALID_TERMINATOR = 3; //possible compression used by UPS


        [DllImport("__Internal")]
        public static extern int MWB_getLibVersion();
        [DllImport("__Internal")]
        public static extern int MWB_getSupportedCodes();
        [DllImport("__Internal")]
        public static extern int MWB_setScanningRect(int codeMask, float left, float top, float width, float height);
        [DllImport("__Internal")]
        public static extern int MWB_getScanningRect(int codeMask, out float left, out float top, out float width, out float height);
        [DllImport("__Internal")]
        public static extern int MWB_registerSDK(string key);
        [DllImport("__Internal")]
        public static extern int MWB_setActiveCodes(int codeMask);
        //[DllImport("__Internal")]
        public static extern int MWB_setActiveSubcodes(int codeMask, int subMask);
        //[DllImport("__Internal")]
        public static extern int MWB_cleanupLib();
        //[DllImport("__Internal")]
        public static extern int MWB_getLastType();
        //[DllImport("__Internal")]
        public static extern int MWB_isLastGS1();
        [DllImport("__Internal")]
        public static extern int MWB_scanGrayscaleImage(byte[] gray, int width, int height, out IntPtr res);
        //[DllImport("__Internal")]
        public static extern int MWB_setFlags(int codeMask, int flags);
        [DllImport("__Internal")]
        public static extern int MWB_setLevel(int level);
        [DllImport("__Internal")]
        public static extern int MWB_setDirection(int direction);
        [DllImport("__Internal")]
        public static extern int MWB_getDirection();
        [DllImport("__Internal")]
        public static extern int MWB_setResultType(int resultType);
        [DllImport("__Internal")]
        public static extern int MWB_setDuplicatesTimeout(int timeout);
        [DllImport("__Internal")]
        public static extern int MWB_setDuplicate(byte[] barcode, int length);
        [DllImport("__Internal")]
        public static extern int MWB_setMinLength(int codeMask, int minLength);

        [DllImport("__Internal")]
        public static extern int MWP_getLibVersion();
        [DllImport("__Internal")]
        public static extern int MWP_getSupportedParsers();
        [DllImport("__Internal")]
        public static extern double MWP_getFormattedText(int parser_type, byte[] p_input, int inputLength, out byte[] pp_output);
        [DllImport("__Internal")]
        public static extern double MWP_getJSON(int parser_type, byte[] p_input, int inputLength, out byte[] pp_output);


        //[DllImport("__Internal")]
        public static extern void MWA_initialize(out char username, out char apiKey);
        //[DllImport("__Internal")]
        public static extern int MWA_sendReport(out char encryptedResult, out char resultType, out char tag);


        public static void MWBsetScanningRect(int codeMask, CGRect rect)
        {
            MWB_setScanningRect(codeMask, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
        }

        public static CGRect MWBgetScanningRect(int codeMask)
        {
            float left, top, width, height;
            MWB_getScanningRect(codeMask, out left, out top, out width, out height);

            return new CGRect(left, top, width, height);
        }
        public static void MWBsetMaxThreads(int maxThreads)
        {
            MWScannerViewController.param_maxThreads = maxThreads;
        }
        public static void MWBenableZoom(bool enableZoom)
        {
            MWScannerViewController.param_EnableZoom = enableZoom;
        }
        public static void MWBsetZoomLevels(int zoomLevel1, int zoomLevel2, int initialZoomLevel)
        {
            MWScannerViewController.setZoomLevels(zoomLevel1, zoomLevel2, initialZoomLevel);
        }


        /*	static public byte[] zeroString(string str)
{
    string str0 = str + char.MinValue;
    //		return ASCIIEncoding.ASCII.GetBytes(str0);
}*/

        //[DllImport("__Internal", CallingConvention = CallingConvention.StdCall)]
        //public extern static int MWB_getLibVersion();


        public static CGRect RECT_LANDSCAPE_1D = new CGRect(6, 20, 88, 60);
        public static CGRect RECT_LANDSCAPE_2D = new CGRect(20, 6, 60, 88);
        public static CGRect RECT_PORTRAIT_1D = new CGRect(20, 6, 60, 88);
        public static CGRect RECT_PORTRAIT_2D = new CGRect(20, 6, 60, 88);
        public static CGRect RECT_FULL_1D = new CGRect(6, 6, 88, 88);
        public static CGRect RECT_FULL_2D = new CGRect(20, 6, 60, 88);
        public static CGRect RECT_DOTCODE = new CGRect(30, 20, 40, 60);


        public static void initDecoder()
        {


            // choose code type or types you want to search for

            // Our sample app is configured by default to search all supported barcodes...
            BarcodeConfig.MWB_setActiveCodes(BarcodeConfig.MWB_CODE_MASK_25 |
                                             BarcodeConfig.MWB_CODE_MASK_39 |
                                             BarcodeConfig.MWB_CODE_MASK_93 |
                                             BarcodeConfig.MWB_CODE_MASK_128 |
                                             BarcodeConfig.MWB_CODE_MASK_AZTEC |
                                             BarcodeConfig.MWB_CODE_MASK_DM |
                                             BarcodeConfig.MWB_CODE_MASK_EANUPC |
                                             BarcodeConfig.MWB_CODE_MASK_PDF |
                                             BarcodeConfig.MWB_CODE_MASK_QR |
                                             BarcodeConfig.MWB_CODE_MASK_CODABAR |
                                             BarcodeConfig.MWB_CODE_MASK_11 |
                                             BarcodeConfig.MWB_CODE_MASK_MSI |
                                             BarcodeConfig.MWB_CODE_MASK_RSS |
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



            // set decoder effort level (1 - 5)
            // for live scanning scenarios, a setting between 1 to 3 will suffice
            // levels 4 and 5 are typically reserved for batch scanning
            BarcodeConfig.MWB_setLevel(2);

            //get and print Library version
            int ver = BarcodeConfig.MWB_getLibVersion();
            int v1 = (ver >> 16);
            int v2 = (ver >> 8) & 0xff;
            int v3 = (ver & 0xff);
            //NSString *libVersion = [NSString stringWithFormat:@"%d.%d.%d", v1, v2, v3];
            //NSLog(@"Lib version: %@", libVersion);
        }

    }

    public class Scanner : ScannerBase
    {

        ManualResetEvent scanResultResetEvent = new ManualResetEvent(false);

        public MWScannerViewController viewController;
        public UIViewController appController;
        public UINavigationController navController;

        public Scanner()
        {
            appController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (appController is UINavigationController)
            {
                navController = (UINavigationController)appController;

            }
            else
            {
                navController = appController.NavigationController;
            }

            while (appController.PresentedViewController != null)
            {
                appController = appController.PresentedViewController;

                if (appController is UINavigationController)
                {
                    navController = (UINavigationController)appController;

                }
                else
                {
                    navController = appController.NavigationController;
                }
            }

            //navController = null;
        }

        public Scanner(UINavigationController nController)
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController != null)
                {
                    appController = window.RootViewController;

                    break;
                }
            }
            navController = nController;
        }

        override public void initDecoder()
        {
            BarcodeConfig.initDecoder();
        }

        override public bool useFlash
        {
            get { return MWScannerViewController.param_EnableFlash; }
            set { MWScannerViewController.param_EnableFlash = value; }
        }

        override public bool useZoom
        {
            get { return MWScannerViewController.param_EnableZoom; }
            set { MWScannerViewController.param_EnableZoom = value; }
        }
        override public bool enableShowLocation
        {
            get { return MWScannerViewController.param_EnableLocation; }
            set { MWScannerViewController.param_EnableLocation = value; }
        }
        override public bool useHiRes
        {
            get { return MWScannerViewController.param_EnableHiRes; }
            set { MWScannerViewController.param_EnableHiRes = value; }
        }

        override public int overlayMode
        {
            get { return MWScannerViewController.param_OverlayMode; }
            set { MWScannerViewController.param_OverlayMode = value; }
        }

        override public bool useFrontCamera
        {
            get { return MWScannerViewController.param_UseFrontCamera; }
            set { MWScannerViewController.param_UseFrontCamera = value; }
        }
        override public void setZoomLevels(int zoomLevel1, int zoomLevel2, int initialZoomLevel)
        {
            MWScannerViewController.setZoomLevels(zoomLevel1, zoomLevel2, initialZoomLevel);
        }

        override public void setCloseButtonFrame(CGRect frame)
        {
            MWScannerViewController.closeFrame = frame;
        }
        override public void setZoomButtonFrame(CGRect frame)
        {
            MWScannerViewController.zoomFrame = frame;
        }
        override public void setFlashButtonFrame(CGRect frame)
        {
            MWScannerViewController.flashFrame = frame;
        }

        override public void setMaxThreads(int maxThreads)
        {
            MWScannerViewController.param_maxThreads = maxThreads;
        }
        override public void resumeScanning()
        {
            MWScannerViewController.state = CameraState.CAMERA;
        }
        override public void closeScanner()
        {

            if (vc != null && vc.View.ViewWithTag(9158436) != null)
            {
                vc.View.ViewWithTag(9158436).RemoveFromSuperview();
                scannerViewController.stopScanning();
                previewLayer = null;

                NSNotificationCenter.DefaultCenter.RemoveObserver(_rotationObserver);
                scannerViewController = null;
            }
            if (viewController != null)
            {
                this.appController.InvokeOnMainThread(() => viewController.DismissViewController(true, null));
            }

        }
        override public bool closeScannerOnDecode
        {
            get { return MWScannerViewController.param_CloseScannerOnSuccess; }
            set { MWScannerViewController.param_CloseScannerOnSuccess = value; }

        }

        override public bool use60fps
        {
            get { return MWScannerViewController.param_Use60FPS; }
            set { MWScannerViewController.param_Use60FPS = value; }

        }

        override public void setInterfaceOrientation(String orientation)
        {

            UIInterfaceOrientationMask interfaceOrientation = UIInterfaceOrientationMask.LandscapeLeft;

            if (orientation.Equals("Portrait"))
            {
                interfaceOrientation = UIInterfaceOrientationMask.Portrait;
            }
            if (orientation.Equals("LandscapeLeft"))
            {
                interfaceOrientation = UIInterfaceOrientationMask.LandscapeLeft;
            }
            if (orientation.Equals("LandscapeRight"))
            {
                interfaceOrientation = UIInterfaceOrientationMask.LandscapeRight;
            }
            if (orientation.Equals("All"))
            {
                interfaceOrientation = UIInterfaceOrientationMask.All;
            }
            if (orientation.Equals("AllButUpsideDown"))
            {
                interfaceOrientation = UIInterfaceOrientationMask.AllButUpsideDown;
            }
            MWScannerViewController.setInterfaceOrientation(interfaceOrientation);

        }

        override public Task<ScannerResult> Scan()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    ScannerResult result = null;

                    scanResultResetEvent.Reset();

                    this.appController.InvokeOnMainThread(() =>
                    {

                        viewController = new MWScannerViewController();
                        MWScannerViewController.successCallback = null;
                        viewController.OnResult += barcodeResult =>
                        {
                            ((UIViewController)viewController).InvokeOnMainThread(() =>
                            {
                                viewController.stopScanning();
                                ((UIViewController)viewController).DismissViewController(true, () =>
                                {

                                    result = barcodeResult;
                                    scanResultResetEvent.Set();

                                });
                            });
                        };
                        if (navController != null)
                        {

                            navController.PresentViewController((UIViewController)viewController, true, null);
                        }
                        else
                        {

                            appController.PresentViewController((UIViewController)viewController, true, null);
                        }

                    });

                    scanResultResetEvent.WaitOne();
                    ((UIViewController)viewController).Dispose();

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    return null;
                }
            });

        }
        MWScannerViewController scannerViewController;

        float leftP;
        float topP;
        float widthP;
        float heightP;
        AVCaptureVideoPreviewLayer previewLayer;
        UIInterfaceOrientation currentOrientation;
        UIImageView overlayImage;
        public bool useAutoRect = true;
        bool useFCamera = false;
        NSMutableDictionary recgtVals;
        static CGRect[] rects;
        UIViewController vc;
        private NSObject _rotationObserver;

        override public void ScanInView(IScanSuccessCallback callback, CGRect scanningRect)
        {

            this.appController.InvokeOnMainThread(() =>
            {
                try
                {

                    vc = UIApplication.SharedApplication.KeyWindow.RootViewController;

                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }
                    if (vc is UINavigationController)
                    {
                        vc = ((UINavigationController)vc).TopViewController;
                    }

                    if (vc.View.ViewWithTag(9158436) == null)
                    {
                        recgtVals = null;
                        Console.WriteLine("PASSED");


                        currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                        scannerViewController = new MWScannerViewController();
                        scannerViewController.View = NSBundle.MainBundle.LoadNib("MWScannerViewController", scannerViewController.Self, null).GetItem<UIView>(0);

                        MWScannerViewController.successCallback = callback;
                        MWScannerViewController.scannerInstance = this;

                        _rotationObserver = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), (observed) =>
                        {

                            if (vc.View.ViewWithTag(9158436) != null
                                || currentOrientation != UIApplication.SharedApplication.StatusBarOrientation
                                && (int)UIDevice.CurrentDevice.Orientation <= 4
                                && (int)UIDevice.CurrentDevice.Orientation == (int)UIApplication.SharedApplication.StatusBarOrientation)
                            {
                                currentOrientation = UIApplication.SharedApplication.StatusBarOrientation;
                                var scannerView = vc.View.ViewWithTag(9158436);

                                float oX = (float)(leftP / 100 * UIScreen.MainScreen.Bounds.Size.Width);
                                float oY = (float)(topP / 100 * UIScreen.MainScreen.Bounds.Size.Height);

                                float oWidth = (float)(widthP / 100 * UIScreen.MainScreen.Bounds.Size.Width);
                                float oHeight = (float)(heightP / 100 * UIScreen.MainScreen.Bounds.Size.Height);

                                scannerView.Frame = new CGRect(oX, oY, oWidth, oHeight);
                                previewLayer.Frame = new CGRect(0, 0, oWidth, oHeight);

                                if ((int)currentOrientation == (int)UIDeviceOrientation.LandscapeLeft)
                                {
                                    previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeRight;
                                }
                                else if ((int)currentOrientation == (int)UIDeviceOrientation.LandscapeRight)
                                {
                                    previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.LandscapeLeft;
                                }
                                else if ((int)currentOrientation == (int)UIDeviceOrientation.Portrait)
                                {
                                    previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.Portrait;
                                }
                                else if ((int)currentOrientation == (int)UIDeviceOrientation.PortraitUpsideDown)
                                {
                                    previewLayer.Connection.VideoOrientation = AVCaptureVideoOrientation.PortraitUpsideDown;
                                }

                                setAutoRect(previewLayer);

                                if (overlayMode == 1)
                                {
                                    MWOverlay.removeFromPreviewLayer();
                                    MWOverlay.addToPreviewLayer(previewLayer);
                                }
                                else if (overlayMode == 2)
                                {
                                    overlayImage.Frame = previewLayer.Frame;
                                }

                                if (MWScannerViewController.param_EnableFlash && scannerViewController.flashButton != null)
                                {
                                    scannerViewController.flashButton.Frame = new CGRect(scannerView.Frame.Size.Width - 10 - 60, 10, 60, 60);
                                }
                            }

                        });

                        this.leftP = (float)scanningRect.X;
                        this.topP = (float)scanningRect.Y;
                        this.widthP = (float)scanningRect.Width;
                        this.heightP = (float)scanningRect.Height;

                        float x = (float)(leftP / 100 * UIScreen.MainScreen.Bounds.Size.Width);
                        float y = (float)(topP / 100 * UIScreen.MainScreen.Bounds.Size.Height);

                        float width = (float)(widthP / 100 * UIScreen.MainScreen.Bounds.Size.Width);
                        float height = (float)(heightP / 100 * UIScreen.MainScreen.Bounds.Size.Height);

                        UIView view = new UIView(new CGRect(x, y, width, height));
                        view.Tag = 9158436;

                        previewLayer = scannerViewController.generateLayerWithRect(new CGPoint(width, height));


                        view.Layer.AddSublayer(previewLayer);

                        vc.View.AddSubview(view);
                        vc.View.BringSubviewToFront(view);

                        MWScannerViewController.state = CameraState.LAUNCHING_CAMERA;
                        scannerViewController.captureSession.StartRunning();
                        MWScannerViewController.state = CameraState.CAMERA;
                        setAutoRect(previewLayer);

                        if (MWScannerViewController.param_OverlayMode == 1)
                        {
                            MWOverlay.addToPreviewLayer(previewLayer);
                        }
                        else if (MWScannerViewController.param_OverlayMode == 2)
                        {
                            overlayImage = new UIImageView(new CGRect(0, 0, width, height));
                            overlayImage.ContentMode = UIViewContentMode.ScaleToFill;
                            overlayImage.Image = UIImage.FromBundle("overlay.png");
                            view.AddSubview(overlayImage);
                        }

                        if (MWScannerViewController.param_EnableFlash)
                        {
                            scannerViewController.flashButton = new UIButton(new CGRect(view.Frame.Size.Width - 10 - 60, 10, 60, 60));
                            scannerViewController.flashButton.SetImage(UIImage.FromBundle("flashbuttonoff.png"), UIControlState.Normal);
                            scannerViewController.flashButton.SetImage(UIImage.FromBundle("flashbuttonon.png"), UIControlState.Selected);
                            scannerViewController.flashButton.Selected = false;
                            scannerViewController.flashButton.Hidden = false;
                            scannerViewController.flashButton.SetBackgroundImage(null, UIControlState.Normal);
                            scannerViewController.flashButton.SetBackgroundImage(null, UIControlState.Selected);
                            scannerViewController.flashButton.TouchUpInside += delegate
                            {
                                scannerViewController.toggleTorch();
                            };
                            view.AddSubview(scannerViewController.flashButton);
                        }

                        if (MWScannerViewController.param_EnableZoom)
                        {
                            scannerViewController.zoomButton = new UIButton(new CGRect(10, 10, 60, 60));
                            scannerViewController.zoomButton.SetImage(UIImage.FromBundle("zoom.png"), UIControlState.Normal);
                            scannerViewController.zoomButton.Hidden = false;
                            scannerViewController.zoomButton.SetBackgroundImage(null, UIControlState.Normal);
                            scannerViewController.zoomButton.TouchUpInside += delegate
                            {
                                scannerViewController.doZoomTooggleView();
                            };
                            view.AddSubview(scannerViewController.zoomButton);
                        }

                        if (leftP == 0 && topP == 0 && widthP == 1 && heightP == 1)
                        {
                            view.Hidden = true;
                        }

                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                }
            });

        }


        public void toggleFlash()
        {
            if (vc.View.ViewWithTag(9158436) != null)
            {
                scannerViewController.toggleTorch();
            }
        }
        public void toggleZoom()
        {
            if (vc.View.ViewWithTag(9158436) != null)
            {
                MWScannerViewController.zoomLevel++;
                if (MWScannerViewController.zoomLevel > 2)
                {
                    MWScannerViewController.zoomLevel = 0;
                }
                scannerViewController.updateDigitalZoom();
            }
        }



        override public bool ScanWithCallback(IScanSuccessCallback callback)
        {

            try
            {


                scanResultResetEvent.Reset();

                this.appController.InvokeOnMainThread(() =>
                {

                    viewController = new MWScannerViewController();
                    MWScannerViewController.successCallback = callback;

                    if (navController != null)
                    {
                        navController.PresentModalViewController((UIViewController)viewController, true);
                        //Console.WriteLine("navController ");

                    }
                    else
                    {
                        //											appController.PresentViewController((UIViewController)viewController, true, null);
                        appController.PresentModalViewController((UIViewController)viewController, true);
                        //Console.WriteLine("appController ");

                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return false;
            }

        }


        private void setAutoRect(AVCaptureVideoPreviewLayer layer)
        {
            CGPoint p1 = layer.CaptureDevicePointOfInterestForPoint(new CGPoint(0, 0));
            CGPoint p2 = layer.CaptureDevicePointOfInterestForPoint(new CGPoint(layer.Frame.Size.Width, layer.Frame.Size.Height));

            if (p1.X > p2.X)
            {
                float tmp = (float)p1.X;
                p1.X = p2.X;
                p2.X = tmp;
            }
            if (p1.Y > p2.Y)
            {
                float tmp = (float)p1.Y;
                p1.Y = p2.Y;
                p2.Y = tmp;
            }


            int[] masks = new int[] {
                                        BarcodeConfig.MWB_CODE_MASK_128,
                                        BarcodeConfig.MWB_CODE_MASK_25,
                                        BarcodeConfig.MWB_CODE_MASK_39,
                                        BarcodeConfig.MWB_CODE_MASK_93,
                                        BarcodeConfig.MWB_CODE_MASK_AZTEC,
                                        BarcodeConfig.MWB_CODE_MASK_DM,
                                        BarcodeConfig.MWB_CODE_MASK_EANUPC,
                                        BarcodeConfig.MWB_CODE_MASK_PDF,
                                        BarcodeConfig.MWB_CODE_MASK_QR,
                                        BarcodeConfig.MWB_CODE_MASK_RSS,
                                        BarcodeConfig.MWB_CODE_MASK_CODABAR,
                                        BarcodeConfig.MWB_CODE_MASK_DOTCODE,
                                        BarcodeConfig.MWB_CODE_MASK_11,
                                        BarcodeConfig.MWB_CODE_MASK_MSI,
                                        BarcodeConfig.MWB_CODE_MASK_MAXICODE,
                                        BarcodeConfig.MWB_CODE_MASK_POSTAL
                                    };

            if (useAutoRect)
            {

                p1.X += 0.02f;
                p1.Y += 0.02f;
                p2.X -= 0.02f;
                p2.Y -= 0.02f;

                for (int i = 0; i < masks.Length; i++)
                {
                    BarcodeConfig.MWB_setScanningRect(masks[i], (float)p1.X * 100, (float)p1.Y * 100, (float)(p2.X - p1.X) * 100, (float)(p2.Y - p1.Y) * 100);
                }
            }
            else
            {

                if (rects == null)
                {
                    rects = new CGRect[14];

                    for (int i = 0; i < masks.Length; i++)
                    {
                        rects[i] = BarcodeConfig.MWBgetScanningRect(masks[i]);
                    }

                }
                else
                {
                    for (int i = 0; i < masks.Length; i++)
                    {
                        BarcodeConfig.MWBsetScanningRect(masks[i], rects[i]);
                    }
                }

                for (int i = 0; i < masks.Length; i++)
                {
                    CGRect tmpRect = BarcodeConfig.MWBgetScanningRect(masks[i]);
                    BarcodeConfig.MWB_setScanningRect(
                        masks[i],
                        (float)(p1.X + (1 - p1.X * 2) * (tmpRect.X / 100)) * 100,
                        (float)(p1.Y + (1 - p1.Y * 2) * (tmpRect.Y / 100)) * 100,
                        (float)((p2.X - p1.X) * (tmpRect.Width / 100) * 100),
                        (float)((p2.Y - p1.Y) * (tmpRect.Height / 100) * 100));

                }


            }


        }


    }
}