using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using Android.Content;
using Android.Views;
using Android.Runtime;
using Java.IO;
using System.Collections.Generic;
using Java.Lang;
using Android.Widget;
using Android.Util;
using Android.App;

namespace ManateeShoppingCart.Droid.MWBarcodeScanner
{
	
	
	public class BarcodeConfig {
		
		/**
		* @name Basic return values for API functions
		* @{
		*/
		public static int MWB_RT_OK = 0;
		public static int MWB_RT_FAIL = -1;
		public static int MWB_RT_NOT_SUPPORTED = -2;
		public static int MWB_RT_BAD_PARAM = -3;
		
		
		/** @brief  Code39 decoder flags value: require checksum check
		*/
		public static int MWB_CFG_CODE39_REQUIRE_CHECKSUM = 0x2;
		/**/
		
		/** @brief  Code39 decoder flags value: don't require stop symbol - can lead to false results
		*/
		public static int  MWB_CFG_CODE39_DONT_REQUIRE_STOP = 0x4;
		/**/
		
		/** @brief  Code39 decoder flags value: decode full ASCII
		*/
		public static int MWB_CFG_CODE39_EXTENDED_MODE =      0x8;
		/**/
		
		/** @brief  Code93 decoder flags value: decode full ASCII
		*/
		public static int MWB_CFG_CODE93_EXTENDED_MODE =      0x8;
		/**/

		/** @brief  UPC/EAN decoder disable addons detection
 		*/
		public static int MWB_CFG_EANUPC_DISABLE_ADDON = 	   0x1;
		/**/

		/** @brief  Code25 decoder flags value: require checksum check
		*/
		public static int  MWB_CFG_CODE25_REQ_CHKSUM =        0x1;
		/**/
		
		/** @brief  Code11 decoder flags value: require checksum check
		*  MWB_CFG_CODE11_REQ_SINGLE_CHKSUM is set by default
		*/
		public static int  MWB_CFG_CODE11_REQ_SINGLE_CHKSUM =  0x1;
		public static int  MWB_CFG_CODE11_REQ_DOUBLE_CHKSUM =  0x2;
		/**/
		
		/** @brief  MSI Plessey decoder flags value: require checksum check
		*  MWB_CFG_MSI_REQ_10_CHKSUM is set by default
		*/
		public static int  MWB_CFG_MSI_REQ_10_CHKSUM =                 0x01;
		public static int  MWB_CFG_MSI_REQ_1010_CHKSUM =               0x02;
		public static int  MWB_CFG_MSI_REQ_11_IBM_CHKSUM =             0x04;
		public static int  MWB_CFG_MSI_REQ_11_NCR_CHKSUM =             0x08;
		public static int  MWB_CFG_MSI_REQ_1110_IBM_CHKSUM =           0x10;
		public static int  MWB_CFG_MSI_REQ_1110_NCR_CHKSUM =           0x20;
		/**/
		
		/** @brief  Codabar decoder flags value: include start/stop symbols in result
		*/
		public static int  MWB_CFG_CODABAR_INCLUDE_STARTSTOP =        0x1;
		/**/

		/** @brief  Barcode decoder param types
 		*/
		public static int MWB_PAR_ID_ECI_MODE = 				0x08;
		public static int MWB_PAR_ID_RESULT_PREFIX = 			0x10;
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
		public static int MWB_CODE_MASK_NONE =   0x00000000;
		public static int MWB_CODE_MASK_QR =     0x00000001;
		public static int MWB_CODE_MASK_DM =     0x00000002;
		public static int MWB_CODE_MASK_RSS =    0x00000004;
		public static int MWB_CODE_MASK_39 =     0x00000008;
		public static int MWB_CODE_MASK_EANUPC = 0x00000010;
		public static int MWB_CODE_MASK_128 =    0x00000020;
		public static int MWB_CODE_MASK_PDF =    0x00000040;
		public static int MWB_CODE_MASK_AZTEC =  0x00000080;
		public static int MWB_CODE_MASK_25 =     0x00000100;
		public static int MWB_CODE_MASK_93 =     0x00000200;
		public static int MWB_CODE_MASK_CODABAR =0x00000400;
		public static int MWB_CODE_MASK_DOTCODE =0x00000800;
		public static int MWB_CODE_MASK_11 =	 0x00001000;
		public static int MWB_CODE_MASK_MSI =	 0x00002000;
		public static int MWB_CODE_MASK_MAXICODE=0x00004000;
		public static int MWB_CODE_MASK_POSTAL = 0x00008000;
		public static int MWB_CODE_MASK_ALL =    0x0fffffff;
		/** @} */
		
		
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
			public static int MWB_SUBC_MASK_QR_STANDARD	 = 0x00000001;
			public static int MWB_SUBC_MASK_QR_MICRO	 = 0x00000002;
		/** @} */

			
			/**
			* @name Bit mask identifiers for Code 2 of 5 decoder types
				* @{ */
				public static int MWB_SUBC_MASK_C25_INTERLEAVED = 0x00000001;
				public static int MWB_SUBC_MASK_C25_STANDARD 	= 0x00000002;
				public static int MWB_SUBC_MASK_C25_ITF14 		= 0x00000004;
				public static int MWB_SUBC_MASK_C25_IATA 		= 0x00000008;
				public static int MWB_SUBC_MASK_C25_MATRIX 		= 0x00000010;
				public static int MWB_SUBC_MASK_C25_COOP		= 0x00000020;
				public static int MWB_SUBC_MASK_C25_INVERTED 	= 0x00000040;


			/** @} */

					/**
					* @name Bit mask identifiers for POSTAL decoder types
						* @{ */
						public static int MWB_SUBC_MASK_POSTAL_POSTNET	= 0x00000001;
						public static int MWB_SUBC_MASK_POSTAL_PLANET 	= 0x00000002;
						public static int MWB_SUBC_MASK_POSTAL_IM 		= 0x00000004;
						public static int MWB_SUBC_MASK_POSTAL_ROYAL 	= 0x00000008;
						
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
						* @name Identifiers for result fields types
						* @{ */
						public const  int MWB_RESULT_FT_BYTES =                0x00000001;
						public const  int MWB_RESULT_FT_TEXT =                 0x00000002;
						public const  int MWB_RESULT_FT_TYPE =                 0x00000003;
						public const  int MWB_RESULT_FT_SUBTYPE =              0x00000004;
						public const  int MWB_RESULT_FT_SUCCESS =              0x00000005;
						public const  int MWB_RESULT_FT_ISGS1 =                0x00000006;
						public const  int MWB_RESULT_FT_LOCATION =             0x00000007;
						public const  int MWB_RESULT_FT_IMAGE_WIDTH =          0x00000008;
						public const  int MWB_RESULT_FT_IMAGE_HEIGHT =         0x00000009;
						public const  int MWB_RESULT_FT_PARSER_BYTES =		   0x0000000A;

						public const int MWB_RESULT_FT_MODULES_COUNT_X = 		0x0000000B;
						public const int MWB_RESULT_FT_MODULES_COUNT_Y = 		0x0000000C;
						public const int MWB_RESULT_FT_MODULE_SIZE_X = 			0x0000000D;
						public const int MWB_RESULT_FT_MODULE_SIZE_Y = 			0x0000000E;
						public const int MWB_RESULT_FT_SKEW = 					0x0000000F;
						public const int MWB_RESULT_FT_KANJI =					0x00000010;

						/**
						* @brief Global decoder flags value: apply rotation on input image
						*/
						public const int MWB_CFG_GLOBAL_ROTATE90 = 0x04;
						public const int MWB_CFG_GLOBAL_ROTATE180 = 0x08;


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
						
						
						public const int MWB_RTREG_OK = 0;
						public const int MWB_RTREG_INVALID_KEY = -1;
						public const int MWB_RTREG_INVALID_CHECKSUM = -2;
						public const int MWB_RTREG_INVALID_APPLICATION = -3;
						public const int MWB_RTREG_INVALID_SDK_VERSION = -4;
						public const int MWB_RTREG_INVALID_KEY_VERSION = -5;
						public const int MWB_RTREG_INVALID_PLATFORM = -6;
						public const int MWB_RTREG_KEY_EXPIRED = -7;
						
						
						
						
						/**
						* @name Bit mask identifiers for supported decoder types
						* @{ */
						public const int MWP_PARSER_MASK_NONE =       0x00000000;
						public const int MWP_PARSER_MASK_GS1 =        0x00000001; 
						public const int MWP_PARSER_MASK_IUID =       0x00000002;
						public const int MWP_PARSER_MASK_ISBT =       0x00000004;
						public const int MWP_PARSER_MASK_AAMVA =      0x00000008;
						public const int MWP_PARSER_MASK_HIBC =       0x00000010;
						public const int MWP_PARSER_MASK_SCM =		  0x00000020;
						public const int MWP_PARSER_MASK_AUTO =       0x0fffffff;
						
						/** @} */
						
						
						//UID PARSER ERROR CODES
						public const double UID_ERROR_INVALID_HEADER =   -1;
						public const double UID_ERROR_INVALID_FORMAT =   -2;
						public const double UID_ERROR_INVALID_EI =       -3.0;
						public const double UID_ERROR_INVALID_CAGE =     -3.1;
						public const double UID_ERROR_INVALID_DUNS =     -3.2;
						public const double UID_ERROR_INVALID_DODAAC =   -3.3;
						public const double UID_ERROR_INVALID_GS1COMP =  -3.4;
						public const double UID_ERROR_INVALID_PN =       -4;
						public const double UID_ERROR_INVALID_SN =       -5;
						public const double UID_ERROR_INVALID_UII =      -6;
						public const double UID_ERROR_INVALID_LOT =      -7;
						public const double UID_ERROR_GS_MISSING =       -8;     //GS Missing after Header
						public const double UID_ERROR_RS_MISSING =       -9;
						public const double UID_ERROR_EOT_MISSING =      -10;
						public const double UID_ERROR_NO_SN =            -11;
						public const double UID_ERROR_NO_EI =            -12;
						public const double UID_ERROR_NO_PN =            -13;
						public const double UID_ERROR_NO_LOT =           -14;
						public const double UID_ERROR_DUPLICATE_DQ =     -15;
						public const double UID_ERROR_DUPLICATE_UII =    -16;
						public const double UID_ERROR_DUPLICATE_LOT =    -17;
						public const double UID_ERROR_DUPLICATE_SN =     -18;
						public const double UID_ERROR_DUPLICATE_EI =     -19;
						public const double UID_ERROR_LOT_PN_CONFLICT =  -20;
						public const double UID_ERROR_MISSING_REQ =      -21;
						public const double UID_ERROR_INVALID_IAC =      -22;
						public const double UID_ERROR_INVALID_TEI =      -23;
						
						//UID PARSER WARNING CODES
						public const double UID_WARN_EXTRA_CHARS =       91;      //characters after EOT
						public const double UID_WARN_UNNEEDED_DATA =     92;      //unneeded additional data
						public const double UID_WARN_SPACE_AROUND =      93;      //space at the beginning or end of the uid
						public const double UID_WARN_UNKNOWN_DQ =        94;
						public const double UID_WARN_OBSOLETE_FORMAT =   95;      //warning for DD
						
						
						//AAMVA PARSER ERROR CODES
						public const double AAMVA_ERROR_INVALID_FORMAT  =         -1;
						public const double AAMVA_ERROR_INVALID_HEADER  =  	      -2;
						public const double AAMVA_ERROR_INVALID_IIN     = 	      -3;
						public const double NOT_ENOUGHT_MEMORY           =        -4;
						public const double AAMVA_ERROR_INVALID_JN      =	      -5;
						public const double AAMVA_ERROR_INVALID_NENTIRES  =	      -6;


						//AAMVA PARSER WARNING CODES
						public const double AAMVA_WARNING_MISSING_MANDATORY_FIELDS  = 1;


						//UPS/SCM PARSER ERROR CODES
						public const double SCM_ERROR_INVALID_FORMAT		=	-1;
							public const double SCM_ERROR_INVALID_CODE		=	-2;
						public const double SCM_ERROR_ELEMENT_NOT_FOUND		=	-3;
						public const double SCM_ERROR_CANT_ALLOCATE_MEMORY	=	-4;

						//UPS/SCM PARSER WARNING CODES
						public const double SCM_WARNING_LENGTH_OUT_OF_BOUNDS	=	1;	//possible compression used by UPS
						public const double SCM_WARNING_FIELD_EXCEEDS_MAX_LENGTH=	2;	//possible compression used by UPS
						public const double SCM_WARNING_INVALID_TERMINATOR		=	3;	//possible compression used by UPS





						
						/**
						* @name Identifiers for result types
						* @{ */
						
						public const int MWB_RESULT_TYPE_RAW  =               0x00000001;
						public const int MWB_RESULT_TYPE_MW =                 0x00000002;
						//public const int MWB_RESULT_TYPE_JSON =               0x00000003; // not supported yet

						/** @} */



						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getLibVersion();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getSupportedCodes();
						[DllImport("libBarcodeScannerLib.so") ]
						public static extern int MWB_setScanningRect(int codeMask, float left, float top, float width, float height);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getScanningRect(int codeMask, out float left, out float top, out float width, out float height);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_registerSDK(string key);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setActiveCodes(int codeMask);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getActiveCodes();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setActiveSubcodes(int codeMask, int subMask);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_cleanupLib();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getLastType();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_isLastGS1();
						
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_scanGrayscaleImage(byte[] gray, int width, int height, out IntPtr pp_data);
						
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setFlags(int codeMask, int flags);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setLevel(int level);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setDirection(int direction);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getDirection();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_getResultType ();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setResultType (int resultType);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setDuplicatesTimeout (int timeout);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern void MWB_setDuplicate (byte[] barcode, int length);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWB_setMinLength (int codeMask, int minLength);
						
						
						
						
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWP_getLibVersion();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern int MWP_getSupportedParsers();
						[DllImport("libBarcodeScannerLib.so")]
						public static extern double MWP_getFormattedText(int parser_type, byte[] p_input,int inputLength, out byte[] pp_output);
						[DllImport("libBarcodeScannerLib.so")]
						public static extern double MWP_getJSON(int parser_type, byte[] p_input, int inputLength, out byte[] pp_output);
						
						
						
						public static void MWBsetScanningRect(int codeMask, RectangleF rect)
						{
							MWB_setScanningRect(codeMask, (float)rect.Left, (float)rect.Top, (float)rect.Width, (float)rect.Height);
						}
						
						public static RectangleF MWBgetScanningRect(int codeMask)
						{
							float left, top, width, height;
							MWB_getScanningRect(0, out left, out top, out width, out height);
							return new RectangleF(left, top, width, height);
						}
						
						public static string getBarcodeName(int bcType)
						{
							string typeName = "Unknown";
							if (bcType == FOUND_128) typeName = "Code 128";
							if (bcType == FOUND_39) typeName = "Code 39";
							if (bcType == FOUND_DM) typeName = "Datamatrix";
							if (bcType == FOUND_EAN_13) typeName = "EAN 13";
							if (bcType == FOUND_EAN_8) typeName = "EAN 8";
							if (bcType == FOUND_NONE) typeName = "None";
							if (bcType == FOUND_RSS_14) typeName = "Databar 14";
							if (bcType == FOUND_RSS_14_STACK) typeName = "Databar 14 Stacked";
							if (bcType == FOUND_RSS_EXP) typeName = "Databar Expanded";
							if (bcType == FOUND_RSS_LIM) typeName = "Databar Limited";
							if (bcType == FOUND_UPC_A) typeName = "UPC A";
							if (bcType == FOUND_UPC_E) typeName = "UPC E";
							if (bcType == FOUND_PDF) typeName = "PDF417";
							if (bcType == FOUND_QR) typeName = "QR";
							if (bcType == FOUND_AZTEC) typeName = "Aztec";
							if (bcType == FOUND_25_INTERLEAVED) typeName = "Code 25 Interleaved";
							if (bcType == FOUND_25_STANDARD) typeName = "Code 25 Standard";
							if (bcType == FOUND_93) typeName = "Code 93";
							if (bcType == FOUND_CODABAR) typeName = "Codabar";
							if (bcType == FOUND_DOTCODE) typeName = "Dotcode";
							if (bcType == FOUND_128_GS1) typeName = "Code 128 GS1";
							if (bcType == FOUND_ITF14) typeName = "ITF 14";
							if (bcType == FOUND_11) typeName = "Code 11";
							if (bcType == FOUND_MSI) typeName = "MSI Plessey";
							if (bcType == FOUND_25_IATA) typeName = "25 IATA";
							if (bcType == FOUND_25_MATRIX) typeName = "25 Matrix";
							if (bcType == FOUND_25_COOP) typeName = "25 Coop";
							if (bcType == FOUND_25_INVERTED) typeName = "25 Inverted";
							if (bcType == FOUND_QR_MICRO) typeName = "QR Micro";
							if (bcType == FOUND_MAXICODE) typeName = "Maxicode";
							if (bcType == FOUND_POSTNET) typeName = "Postnet";
							if (bcType == FOUND_PLANET) typeName = "Planet";
							if (bcType == FOUND_IMB) typeName = "IMB";
							if (bcType == FOUND_ROYALMAIL) typeName = "Royal Mail";

							
							return typeName;
						}
						
						
						public static void MWPsetParserMask(int parserMask){
							ScannerActivity.setParserMask (parserMask);
							
						}
						
						public static void MWBsetMaxThreads(int maxThreads)
						{
							ScannerActivity.param_maxThreads = maxThreads;
						}
						public static void MWBenableZoom(bool enableZoom)
						{
							ScannerActivity.param_EnableZoom = enableZoom;
						}
						public static void MWBsetZoomLevels(int zoomLevel1, int zoomLevel2,
						                                    int initialZoomLevel) {
							ScannerActivity.param_ZoomLevel1 = zoomLevel1;
							ScannerActivity.param_ZoomLevel2 = zoomLevel2;
							ScannerActivity.zoomLevel = zoomLevel2;
							if (ScannerActivity.zoomLevel > 2) {
								ScannerActivity.zoomLevel = 2;
							}
							if (ScannerActivity.zoomLevel < 0) {
								ScannerActivity.zoomLevel = 0;
							}
						}
						
						
						public static RectangleF RECT_LANDSCAPE_1D = new RectangleF(6, 20, 88, 60);
						public static RectangleF RECT_LANDSCAPE_2D = new RectangleF(20, 6, 60, 88);
						public static RectangleF RECT_PORTRAIT_1D = new RectangleF(20, 6, 60, 88);
						public static RectangleF RECT_PORTRAIT_2D = new RectangleF(20, 6, 60, 88);
						public static RectangleF RECT_FULL_1D = new RectangleF(6, 6, 88, 88);
						public static RectangleF RECT_FULL_2D = new RectangleF(20, 6, 60, 88);
						public static RectangleF RECT_DOTCODE = new RectangleF(30, 20, 40, 60);
						
						
						public static void initDecoder() {
							
							System.Console.WriteLine("Decoder initializing");
							
							
							
							// choose code type or types you want to search for
							
							// Our sample app is configured by default to search all supported barcodes...
							BarcodeConfig.MWB_setActiveCodes(BarcodeConfig.MWB_CODE_MASK_25    |
							                                 BarcodeConfig.MWB_CODE_MASK_39     |
							                                 BarcodeConfig.MWB_CODE_MASK_93     |
							                                 BarcodeConfig.MWB_CODE_MASK_128    |
							                                 BarcodeConfig.MWB_CODE_MASK_AZTEC  |
							                                 BarcodeConfig.MWB_CODE_MASK_DM     |
							                                 BarcodeConfig.MWB_CODE_MASK_EANUPC |
							                                 BarcodeConfig.MWB_CODE_MASK_PDF    |
							                                 BarcodeConfig.MWB_CODE_MASK_QR     |
							                                 BarcodeConfig.MWB_CODE_MASK_CODABAR|
							                                 BarcodeConfig.MWB_CODE_MASK_11     |
							                                 BarcodeConfig.MWB_CODE_MASK_MSI    |
							                                 BarcodeConfig.MWB_CODE_MASK_RSS	|
									                         BarcodeConfig.MWB_CODE_MASK_MAXICODE |
									                         BarcodeConfig.MWB_CODE_MASK_POSTAL
									                        );
							
						
							
							
							// Our sample app is configured by default to search both directions...
							BarcodeConfig.MWB_setDirection(BarcodeConfig.MWB_SCANDIRECTION_HORIZONTAL | BarcodeConfig.MWB_SCANDIRECTION_VERTICAL);
							// set the scanning rectangle based on scan direction(format in pct: x, y, width, height)
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_25,     RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_39,     RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_93,     RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_128,    RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_AZTEC,  RECT_FULL_2D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DM,     RECT_FULL_2D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_EANUPC, RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_PDF,    RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_QR,     RECT_FULL_2D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_RSS,    RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_CODABAR,RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_DOTCODE,RECT_DOTCODE);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_11,     RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MSI,    RECT_FULL_1D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_MAXICODE, RECT_FULL_2D);
							BarcodeConfig.MWBsetScanningRect(BarcodeConfig.MWB_CODE_MASK_POSTAL, RECT_FULL_1D);

							
							// set decoder effort level (1 - 5)
							// for live scanning scenarios, a setting between 1 to 3 will suffice
							// levels 4 and 5 are typically reserved for batch scanning
							BarcodeConfig.MWB_setLevel(2);
							
							//get and print Library version
							//								int ver = BarcodeConfig.MWB_getLibVersion();
							//								int v1 = (ver >> 16);
							//								int v2 = (ver >> 8) & 0xff;
							//								int v3 = (ver & 0xff);
							//NSString *libVersion = [NSString stringWithFormat:@"%d.%d.%d", v1, v2, v3];
							//NSLog(@"Lib version: %@", libVersion);
						}
						
					}
					
					public class Scanner: ScannerBase, Android.Views.ISurfaceHolderCallback, DecodeCallback
					{
						static SurfaceView surfaceView;
						static RelativeLayout rlSurfaceContainer;
						static RelativeLayout rlFullScreen;
						static ScrollView scrollView;
						static ImageView overlayImage;
						static ProgressBar pBar;
						public bool useAutoRect = true;
						static RectangleF[] rects;
						private static ImageButton flashButton;
						private static ImageButton zoomButton;
						public static double widthP;
						public static double heightP;
						public static double xP;
						public static double yP;
						private static int previousWidth = 0;
						
						ManualResetEvent scanResultResetEvent = new ManualResetEvent(false);
						
						
						public Scanner (Context context)
						{
							this.mContext = context;
						}
						
						public Context mContext { get; private set; }
						
						
						override public void initDecoder (){
							BarcodeConfig.initDecoder ();
						}
						
						override public bool useFlash
						{
							get { return ScannerActivity.param_EnableFlash; }
							set { ScannerActivity.param_EnableFlash = value; }
						}
						
						override public bool useFrontCamera
						{
							get { return CameraManager.USE_FRONT_CAMERA; }
							set { CameraManager.USE_FRONT_CAMERA = value; }
						}
						
						override public bool useHiRes
						{
							get { return ScannerActivity.param_EnableHiRes; }
							set { ScannerActivity.param_EnableHiRes = value; }
							
						}
						override public bool closeScannerOnDecode
						{
							get { return ScannerActivity.param_CloseScannerOnSuccess; }
							set { ScannerActivity.param_CloseScannerOnSuccess = value; }
							
						}
						
						override public ScannerActivity.OverlayMode overlayMode
						{
							get { return ScannerActivity.param_OverlayMode; }
							set { ScannerActivity.param_OverlayMode = value; }
							
						}
						override public bool enableShowLocation
						{
							get { return ScannerActivity.param_EnableLocation; }
							set { ScannerActivity.param_EnableLocation = value; }
						}
						
						override public bool useZoom
						{
							get { return ScannerActivity.param_EnableZoom; }
							set { ScannerActivity.param_EnableZoom = value; }
						}
						override public void setZoomLevels(int zoomLevel1, int zoomLevel2, int initialZoomLevel){
							ScannerActivity.param_ZoomLevel1 = zoomLevel1;
							ScannerActivity.param_ZoomLevel2 = zoomLevel2;
							ScannerActivity.zoomLevel = zoomLevel2;
							if (ScannerActivity.zoomLevel > 2) {
								ScannerActivity.zoomLevel = 2;
							}
							if (ScannerActivity.zoomLevel < 0) {
								ScannerActivity.zoomLevel = 0;
							}
						}
						
						override public void resumeScanning(){
							ScannerActivity.state = ScannerActivity.State.PREVIEW;
						}
						override public void flashOnByDefault(bool onByDefault){
							ScannerActivity.param_defaultFlashOn = onByDefault;	
						}
						
						override public void closeScanner(){
							
							closeScannerView ();
							
							if(ScannerActivity.activity!=null)
								ScannerActivity.activity.OnBackPressed();
						}
						
						override public void setMaxThreads(int maxThreads){
							ScannerActivity.param_maxThreads = maxThreads;
						}
						override public void setInterfaceOrientation (string orientation)
						{
							
							Android.Content.PM.ScreenOrientation interfaceOrientation = Android.Content.PM.ScreenOrientation.Landscape;
							
							if (orientation.Equals("Portrait")){
								interfaceOrientation = Android.Content.PM.ScreenOrientation.Portrait;
							}
							if (orientation.Equals("LandscapeLeft")){
								interfaceOrientation = Android.Content.PM.ScreenOrientation.Landscape;
							}
							if (orientation.Equals("LandscapeRight")){
								interfaceOrientation = Android.Content.PM.ScreenOrientation.ReverseLandscape;
							}
							if (orientation.Equals("All")){
								interfaceOrientation = Android.Content.PM.ScreenOrientation.Unspecified;
							}
							
							ScannerActivity.param_Orientation = interfaceOrientation;
							
						}
						
						override public bool ScanWithCallback (IScanSuccessCallback callback)
						{
							try
							{
								
								scanResultResetEvent.Reset();
								var waitScanResetEvent = new System.Threading.ManualResetEvent(false);
								var scanIntent = new Intent(this.mContext, typeof(ScannerActivity));
								ScannerActivity.successCallback = callback;
								
								
								this.mContext.StartActivity(scanIntent);
								
								return true;
							}
							catch (System.Exception ex)
							{
								System.Console.WriteLine(ex);
								
								return false;
							}
							
						}
						
						
						override public bool ScanInView (Context context, RectangleF scanningRect)
						{
							try
							{
								
								if(surfaceView == null){
									
									CameraManager.init(mContext);
									
									ViewGroup rootView = (ViewGroup) ((Android.App.Activity)context).FindViewById(Android.Resource.Id.Content);
									
									
									xP = scanningRect.X;
									yP = scanningRect.Y;
									widthP = scanningRect.Width;
									heightP = scanningRect.Height;
									
									IWindowManager wm = (IWindowManager) mContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
									Display display = wm.DefaultDisplay;
									
									Android.Graphics.Point size = new Android.Graphics.Point();
									display.GetSize(size);
									
									int w = rootView.Width;
									int h = rootView.Height;
									
									float AR = (float) size.Y / (float) size.X;
									
									double x = (double)xP / 100 * w;
									double y = (double)yP / 100 * h;
									double width = (double)widthP / 100 * w;
									double height = (double)heightP / 100 * h;
									
									rlFullScreen = new RelativeLayout(mContext);
									rlSurfaceContainer = new RelativeLayout(mContext);
									scrollView = new ScrollView(mContext);
									scrollView.VerticalScrollBarEnabled = false;
									
									scrollView.Touch += (sender, e) => {
										e.Handled = true;
									};
									
									((Android.App.Activity)this.mContext).RunOnUiThread(() => {
										
										
										scrollView.Visibility = ViewStates.Invisible;
										surfaceView = new SurfaceView(mContext);
										
										pBar = new ProgressBar(mContext);
										RelativeLayout.LayoutParams pBarParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent,
										                                                                         RelativeLayout.LayoutParams.WrapContent);
										
										pBarParams.AddRule(LayoutRules.CenterInParent);
										
										pBar.LayoutParameters = pBarParams;
										
										pBar.Visibility = ViewStates.Visible;
										rlFullScreen.LayoutParameters =new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
										
										RelativeLayout.LayoutParams lps = new RelativeLayout.LayoutParams((int) Java.Lang.Math.Round(width), (int) Java.Lang.Math.Round(height));
										lps.LeftMargin = (int) Java.Lang.Math.Round(x);
										lps.TopMargin = (int) Java.Lang.Math.Round(y);
										int heightTmp = 0;
										int widthTmp = 0;
										
										if (width * AR >= height) {
											heightTmp = (int) Java.Lang.Math.Round(width * AR);
											widthTmp = (int) Java.Lang.Math.Round(width);
										} else {
											widthTmp = (int) Java.Lang.Math.Round(height / AR);
											heightTmp = (int) Java.Lang.Math.Round(height);
										}
										
										float heightTmpRunnable = heightTmp;
										rlSurfaceContainer.LayoutParameters = new RelativeLayout.LayoutParams(Java.Lang.Math.Round(widthTmp), Java.Lang.Math.Round(heightTmp));
										surfaceView.LayoutParameters = new RelativeLayout.LayoutParams(Java.Lang.Math.Round(widthTmp), Java.Lang.Math.Round(heightTmp));
										
										rlSurfaceContainer.AddView(surfaceView);
										
										if (ScannerActivity.param_EnableFlash) {
											int widthHeight = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 64, mContext.Resources.DisplayMetrics);
											int marginDP = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 5, mContext.Resources.DisplayMetrics);
											int padding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 16, mContext.Resources.DisplayMetrics);
											
											flashButton = new ImageButton(mContext);
											RelativeLayout.LayoutParams flashParams = new RelativeLayout.LayoutParams(widthHeight, widthHeight);
											flashParams.AddRule(LayoutRules.AlignParentRight);
											flashParams.AddRule(LayoutRules.AlignParentTop);
											flashParams.TopMargin = (int) ((heightTmp - height) / 2) + marginDP;
											flashParams.RightMargin = (int) ((widthTmp - width) / 2) + marginDP;
											flashButton.SetImageResource(mContext.Resources.GetIdentifier("flashbuttonoff", "drawable", mContext.ApplicationInfo.PackageName));
											flashButton.SetScaleType(ImageView.ScaleType.FitXy);
											flashButton.SetPadding(padding, padding, padding, padding);
											flashButton.Background = null;
											if (flashButton != null) {
												flashButton.Click += delegate {
													toggleFlash();
												};
											}
											
											rlSurfaceContainer.AddView(flashButton, flashParams);
											rlSurfaceContainer.BringChildToFront(flashButton);
										}
										
										
										if (ScannerActivity.param_EnableZoom) {
											int widthHeight = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 64, mContext.Resources.DisplayMetrics);
											int marginDP = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, mContext.Resources.DisplayMetrics);
											int padding = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 16, mContext.Resources.DisplayMetrics);
											
											zoomButton = new ImageButton(mContext);
											RelativeLayout.LayoutParams zoomParams = new RelativeLayout.LayoutParams(widthHeight, widthHeight);
											zoomParams.AddRule(LayoutRules.AlignParentLeft);
											zoomParams.AddRule(LayoutRules.AlignParentTop);
											zoomParams.TopMargin = (int) ((heightTmp - height) / 2) + marginDP;
											zoomButton.SetPadding(padding, padding, padding, padding);
											zoomParams.LeftMargin = (int) ((widthTmp - width) / 2) + marginDP;
											zoomButton.SetImageResource(mContext.Resources.GetIdentifier("zoom", "drawable", mContext.ApplicationInfo.PackageName));
											
											zoomButton.SetScaleType(ImageView.ScaleType.FitXy);
											zoomButton.Background = null;
											if (zoomButton != null) {
												zoomButton.Click += delegate {
													
													ScannerActivity.toggleZoom();
													
												};
											}
											
											rlSurfaceContainer.AddView(zoomButton, zoomParams);
											rlSurfaceContainer.BringChildToFront(zoomButton);
										}
										
										
										
										scrollView.AddView(rlSurfaceContainer);
										
										scrollView.SetClipToPadding(true);
										
										rlFullScreen.AddView(scrollView,lps);
										rootView.AddView(rlFullScreen);
										
										if (xP == 0 && yP == 0 && widthP == 1 && heightP == 1) {
											rlFullScreen.Visibility = ViewStates.Invisible;
										}
										
										rlFullScreen.AddView(pBar);
										
										Timer timer = null; 
										
										timer = new Timer((obj) =>
										{
											
											((Android.App.Activity)this.mContext).RunOnUiThread(() => {
												setAutoRect();
												scrollView.ScrollTo(0, (int) (heightTmpRunnable / 2 - height / 2));
												
												if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_MW)
												{
													MWOverlay.addOverlay(mContext, surfaceView);
													//															MWOverlay.SetPaused(false);
												}
												else if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_IMAGE)
												{
													overlayImage = new ImageView(mContext);
													overlayImage.SetScaleType(ImageView.ScaleType.FitXy);
													overlayImage.SetImageResource(mContext.Resources.GetIdentifier("overlay", "drawable",
													                                                               mContext.ApplicationInfo.PackageName));
													RelativeLayout.LayoutParams tmpLps = new RelativeLayout.LayoutParams((int)Java.Lang.Math.Round(width), (int)Java.Lang.Math.Round(height));
													tmpLps.TopMargin = (int)Java.Lang.Math.Round(heightTmpRunnable / 2 - height / 2);
													rlSurfaceContainer.AddView(overlayImage, tmpLps);
												}
												else if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_VIEW)
												{
													MWOverlay.addOverlayView(mContext, surfaceView);
													
												}
												Timer timerRotation = null;
												
												timerRotation = new Timer((obj2) =>
												{
													
													((Android.App.Activity)this.mContext).RunOnUiThread(() =>
													{
														ViewTreeObserver vtObserver = rootView.ViewTreeObserver;
														vtObserver.GlobalLayout += (object sender, EventArgs e) =>
														{
															
															if (previousWidth != rootView.Width) {
																previousWidth = rootView.Width;
																onConfigChanged();
															}
															
														};
													});
													timerRotation.Dispose();
													
												}, null, 400, Timeout.Infinite);
												
											});
											timer.Dispose();
											
										}, null, 300, Timeout.Infinite);
										
										
										
										
										ISurfaceHolder holder = surfaceView.Holder;
										holder.AddCallback(this);
										holder.SetType(SurfaceType.PushBuffers);
									});
									
									scanResultResetEvent.Reset();
									ScannerActivity.successCallback = (IScanSuccessCallback) context;
								}
								
								
								
								return true;
							}
							catch (System.Exception ex)
							{
								System.Console.WriteLine(ex);
								
								return false;
							}
							
						}
						
						private void closeScannerView(){
							if (rlFullScreen != null) {
								((Android.App.Activity)this.mContext).RunOnUiThread(() => {
									
									if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_MW )
									{
										MWOverlay.removeOverlay();
									} else if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_IMAGE) {
										rlSurfaceContainer.RemoveView(overlayImage);
									}
									
									CameraManager.get().stopPreview();
									CameraManager.get().closeDriver();
									
									((ViewGroup)rlFullScreen.Parent).RemoveView(rlFullScreen);
									
									rlFullScreen = null;
									rlSurfaceContainer = null;
									surfaceView = null;
									scrollView = null;
									flashButton = null;
									
								});
								
							}
						}
						
						
						public void SurfaceChanged (ISurfaceHolder holder, global::Android.Graphics.Format format, int width, int height){
							
							if (!ScannerActivity.hasSurface)
							{
								ScannerActivity.hasSurface = true;
								initCamera(holder);
							}
							
						}
						
						
						
						
						public void SurfaceCreated(ISurfaceHolder holder) {
							
						}
						
						
						
						public void SurfaceDestroyed(ISurfaceHolder holder) {
							
							ScannerActivity.hasSurface = false;
							
						}
						
						public void toggleZoom(){
							ScannerActivity.toggleZoom ();
						}
						
						public void toggleFlash() {
							ScannerActivity.flashOn = !ScannerActivity.flashOn;
							updateFlash();
						}
						
						private void updateFlash() {
							
							if (flashButton != null) {
								if (!CameraManager.get().isTorchAvailable()) {
									flashButton.Visibility = ViewStates.Gone;
									return;
									
								}
								
								
								if (ScannerActivity.flashOn) {
									flashButton.SetImageResource(mContext.Resources.GetIdentifier("flashbuttonon", "drawable", mContext.ApplicationInfo.PackageName));
								} else {
									flashButton.SetImageResource(mContext.Resources.GetIdentifier("flashbuttonoff", "drawable", mContext.ApplicationInfo.PackageName));
								}
								
								CameraManager.get().setTorch(ScannerActivity.flashOn);
								
								flashButton.PostInvalidate();
							}
							
						}
						private void setAutoRect(){
							
							if (rlFullScreen != null) {
								
								float p1x;
								float p1y;
								
								float p2x;
								float p2y;
								
								p1x = (float) (surfaceView.Width - scrollView.Width) / 2 / surfaceView.Width;
								p1y = (float) (surfaceView.Height - scrollView.Height) / 2 / surfaceView.Height;
								
								p2x = (float) scrollView.Width / surfaceView.Width;
								p2y = (float) scrollView.Height / surfaceView.Height;
								
								if (surfaceView.Width < surfaceView.Height) {
									float tmp = p1x;
									p1x = p1y;
									p1y = tmp;
									tmp = p2x;
									p2x = p2y;
									p2y = tmp;
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
								
								if (useAutoRect) {
									
									p1x += 0.02f;
									p1y += 0.02f;
									p2x -= 0.04f;
									p2y -= 0.04f;
									
									for (int i = 0; i < masks.Length; i++) {
										BarcodeConfig.MWB_setScanningRect(masks[i], p1x * 100, p1y * 100, (p2x) * 100, (p2y) * 100);
									}
									
								} else {
									
									if (rects == null) {
										
										rects = new RectangleF[14];
										
										for (int i = 0; i < masks.Length; i++) {
											rects [i] = BarcodeConfig.MWBgetScanningRect (masks [i]);
										}
										
									} else {
										
										for (int i = 0; i < masks.Length; i++) {
											BarcodeConfig.MWBsetScanningRect(masks[i], rects[i]);
										}
										
									}
									
									
									
									for (int i = 0; i < masks.Length; i++) {
										BarcodeConfig
											.MWB_setScanningRect(masks[i],
											                     (p1x + ((BarcodeConfig.MWBgetScanningRect(masks[i]).X / 100)
											                             * (surfaceView.Width * p2x)) / surfaceView.Width) * 100,
											                     (p1y + ((BarcodeConfig.MWBgetScanningRect(masks[i]).Y / 100) * (surfaceView.Height * p2y))
											                      / surfaceView.Height) * 100,
											                     (((BarcodeConfig.MWBgetScanningRect(masks[i]).Width / 100) * (surfaceView.Width * p2x))
											                      / surfaceView.Width) * 100,
											                     (((BarcodeConfig.MWBgetScanningRect(masks[i]).Height / 100) * (surfaceView.Width * p2y))
											                      / surfaceView.Width) * 100);
									}
									
								}
								
							}
						}
						
						
						public void onConfigChanged(){
							if (rlFullScreen != null && CameraManager.get().camera != null) {
								
								IWindowManager wm = (IWindowManager) mContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
								Display display = wm.DefaultDisplay;
								
								Android.Graphics.Point size = new Android.Graphics.Point();
								display.GetSize(size);
								
								ViewGroup rootView = (ViewGroup)((Android.App.Activity)mContext).FindViewById(Android.Resource.Id.Content);
								
								
								int w = rootView.Width;
								int h = rootView.Height;
								
								float AR = (float) size.Y / (float) size.X;
								
								double x = xP / 100 * w;
								double y = yP / 100 * h;
								double width = widthP / 100 * w;
								double height = heightP / 100 * h;
								
								RelativeLayout.LayoutParams lps = (RelativeLayout.LayoutParams) scrollView.LayoutParameters;
								
								lps.Width = (int) Java.Lang.Math.Round(width);
								lps.Height = (int) Java.Lang.Math.Round(height);
								
								lps.LeftMargin = (int) Java.Lang.Math.Round(x);
								lps.TopMargin = (int) Java.Lang.Math.Round(y);
								int heightTmp = 0;
								int widthTmp = 0;
								
								if (width * AR >= height) {
									heightTmp = (int) Java.Lang.Math.Round(width * AR);
									widthTmp = (int) Java.Lang.Math.Round(width);
								} else {
									widthTmp = (int) Java.Lang.Math.Round(height / AR);
									heightTmp = (int) Java.Lang.Math.Round(height);
								}
								float heightTmpRunnable = heightTmp;
								float widthTmpRunnable = widthTmp;
								
								scrollView.LayoutParameters = (lps);
								
								ViewGroup.LayoutParams surfaceLPS = rlSurfaceContainer.LayoutParameters;
								surfaceLPS.Width = widthTmp;
								surfaceLPS.Height = heightTmp;
								rlSurfaceContainer.LayoutParameters = (surfaceLPS);
								
								ViewGroup.LayoutParams surfaceViewLPS = surfaceView.LayoutParameters;
								surfaceViewLPS.Width = widthTmp;
								surfaceViewLPS.Height = heightTmp;
								
								surfaceView.LayoutParameters = (surfaceViewLPS);
								
								if (flashButton != null) {
									RelativeLayout.LayoutParams flashParams = (RelativeLayout.LayoutParams)flashButton.LayoutParameters;
									int marginDP = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, mContext.Resources.DisplayMetrics);
									
									flashParams.TopMargin = (int) ((heightTmp - height) / 2) + marginDP;
									flashParams.RightMargin = (int) ((widthTmp - width) / 2) + marginDP;
									flashButton.LayoutParameters = (flashParams);
								}
								if (zoomButton != null) {
									RelativeLayout.LayoutParams zoomParams = (RelativeLayout.LayoutParams) zoomButton.LayoutParameters;
									int marginDP = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, 6, mContext.Resources.DisplayMetrics);
									
									zoomParams.TopMargin = (int) ((heightTmp - height) / 2) + marginDP;
									zoomParams.LeftMargin = (int) ((widthTmp - width) / 2) + marginDP;
									zoomButton.LayoutParameters = (zoomParams);
								}
								
								((Android.App.Activity)this.mContext).RunOnUiThread(() =>
								{
									
									if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_MW)
									{
										MWOverlay.removeOverlay();
									}
									else if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_IMAGE)
									{
										RelativeLayout.LayoutParams overlayLps = (RelativeLayout.LayoutParams)overlayImage.LayoutParameters;
										overlayLps.Width = (int)Java.Lang.Math.Round(width);
										overlayLps.Height = (int)Java.Lang.Math.Round(height);
										overlayLps.TopMargin = (int)Java.Lang.Math.Round(heightTmpRunnable / 2 - height / 2);
										
										overlayImage.LayoutParameters = (overlayLps);
										
									}
								});
								
								Timer timer = null; 
								
								timer = new Timer((obj) =>
								{
									((Android.App.Activity)this.mContext).RunOnUiThread(() => {
										
										setAutoRect();
										if (ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_MW) {
											MWOverlay.removeOverlay();
											MWOverlay.addOverlay(mContext, surfaceView);
										}
										scrollView.ScrollTo((int) Java.Lang.Math.Round(widthTmpRunnable / 2 - width / 2),
										                    (int) Java.Lang.Math.Round(heightTmpRunnable / 2 - height / 2));
									});
									timer.Dispose();
									
								}, null, 300, Timeout.Infinite);
								
								CameraManager.get().setCameraDisplayOrientation(0, CameraManager.get().camera,
								                                                (mContext.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait));
							}
						}

						int currentThread = 0;

						void DecodeCallback.decode(byte[] data, int width, int height) {


							if (ScannerActivity.param_maxThreads > ScannerActivity.MAX_THREADS) {
								ScannerActivity.param_maxThreads = ScannerActivity.MAX_THREADS;
							}
							
							if (ScannerActivity.activeThreads >= ScannerActivity.param_maxThreads || ScannerActivity.state == ScannerActivity.State.STOPPED) {
								return;
							}	


							ScannerActivity.activeThreads++;
							int threadTMP = currentThread++;

							Task.Factory.StartNew (() => {
								
								MWResult mwResult = null;
                                IntPtr pp_data;


								int resLen = BarcodeConfig.MWB_scanGrayscaleImage(data, width, height, out pp_data);
                                
								if (ScannerActivity.state == ScannerActivity.State.STOPPED) {
									ScannerActivity.activeThreads--;
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
									
									if (ScannerActivity.state == ScannerActivity.State.STOPPED) { 
										ScannerActivity.activeThreads--;
										return;
									}
									
									ScannerActivity.state = ScannerActivity.State.STOPPED;
									
									
									((Android.App.Activity)this.mContext).RunOnUiThread(() => 
									                                                    handleDecode (mwResult)
									                                                   );
								}

								ScannerActivity.activeThreads--;

								
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
							
							if (ScannerActivity.param_EnableLocation 
							    && result.locationPoints != null
							    && CameraManager.get().getCurrentResolution() != null 
							    && ScannerActivity.param_OverlayMode == ScannerActivity.OverlayMode.OM_MW) 
							{
								MWOverlay.showLocation(result.locationPoints.points, result.imageWidth, result.imageHeight);
							}
							
							if(ScannerActivity.parserMask != BarcodeConfig.MWP_PARSER_MASK_NONE && !(ScannerActivity.parserMask == BarcodeConfig.MWP_PARSER_MASK_GS1 && !result.isGS1)){
								byte[] parserResult = new byte[10000];
								double parserRes = -1;

								try
								{
									parserRes = BarcodeConfig.MWP_getJSON(ScannerActivity.parserMask, result.encryptedResult.GetBytes(), result.bytesLength, out parserResult);
								}
								catch (System.Exception e) {
									
								}

								
								if (parserRes >= 0) {
									
									s = System.Text.Encoding.UTF8.GetString (parserResult);
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
							
							
							ScannerActivity.successCallback.barcodeDetected(result);
							
							
							if (ScannerActivity.param_CloseScannerOnSuccess) {
								closeScanner();
							}
							
						}
						
						
						private void initCamera(ISurfaceHolder surfaceHolder)
						{
							try
							{
								// Select desired camera resoloution. Not all devices supports all resolutions, closest available will be chosen
								// If not selected, closest match to screen resolution will be chosen
								// High resolutions will slow down scanning proccess on slower devices
								
								if (ScannerActivity.param_EnableHiRes){
									CameraManager.setDesiredPreviewSize(1280, 720);
								} else {
									CameraManager.setDesiredPreviewSize(800, 480);
								}
								
								CameraManager.get().openDriver(surfaceHolder, (mContext.Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait));
								//									int maxZoom = CameraManager.get().getMaxZoom();
								//									if (maxZoom > 100) {
								//										if (ScannerActivity.param_EnableZoom) {
								//											ScannerActivity.updateZoom();
								//										}
								//									}
							}
							catch (IOException ioe)
							{
								return;
							}
							catch (System.Exception e)
							{
								// Barcode Scanner has seen crashes in the wild of this variety:
								// java.?lang.?RuntimeException: Fail to connect to camera service
								return;
							}
							
							
							//Fix for camera sensor rotation bug
							var cameraInfo = new Android.Hardware.Camera.CameraInfo();
							Android.Hardware.Camera.GetCameraInfo(0, cameraInfo);
							if (cameraInfo.Orientation == 270)
							{
								BarcodeConfig.MWB_setFlags(0, BarcodeConfig.MWB_CFG_GLOBAL_ROTATE180);
								
							}	
							
							
							ScannerActivity.flashOn = ScannerActivity.param_defaultFlashOn;
							
							CameraManager.get().startPreview();
							ScannerActivity.state = ScannerActivity.State.PREVIEW;
							
							BarcodeConfig.MWB_setResultType (BarcodeConfig.MWB_RESULT_TYPE_MW);
							CameraManager.get().requestPreviewFrame(this);
							//	         CameraManager.get().requestAutoFocus();
							scrollView.Visibility = ViewStates.Visible;
							pBar.Visibility = ViewStates.Gone;
						    updateFlash();		
							
						}
						
						
						override public Task<ScannerResult> Scan ()
						{

							return Task.Factory.StartNew<ScannerResult>(() => {
								try
								{
									
									ScannerResult result = null;
									
									scanResultResetEvent.Reset();
									var waitScanResetEvent = new System.Threading.ManualResetEvent(false);
									var scanIntent = new Intent(this.mContext, typeof(ScannerActivity));
									
									ScannerActivity.successCallback = null;
									ScannerActivity.OnScanCompleted += (ScannerResult sresult) => 
									{
										result = sresult;
										waitScanResetEvent.Set();
									};
									
									this.mContext.StartActivity(scanIntent);
									
									waitScanResetEvent.WaitOne();
									
									return result;
								}
								catch (System.Exception ex)
								{
									System.Console.WriteLine(ex);
									
									return null;
								}
							});
							
						}
						
					}
					public class MWLocation{
						
						public Android.Graphics.PointF p1;
						public Android.Graphics.PointF p2;
						public Android.Graphics.PointF p3;
						public Android.Graphics.PointF p4;
						
						public Android.Graphics.PointF[] points;
						
						public MWLocation(float[] _points) {
							
							points = new Android.Graphics.PointF[4];
							
							for (int i = 0; i < 4; i++){
								points[i] = new Android.Graphics.PointF();
								points[i].X = _points[i * 2];
								points[i].Y = _points[i * 2 + 1];
							}
							p1 = new Android.Graphics.PointF();
							p2 = new Android.Graphics.PointF();
							p3 = new Android.Graphics.PointF();
							p4 = new Android.Graphics.PointF();
							
							p1.X = _points[0];
							p1.Y = _points[1];
							p2.X = _points[2];
							p2.Y = _points[3];
							p3.X = _points[4];
							p3.Y = _points[5];
							p4.X = _points[6];
							p4.Y = _points[7];
							
						}
						
					}
					
					
					public class MWResult{
						public Java.Lang.String text;
						public byte[] bytes;
						public Java.Lang.String encryptedResult;
						
						public int bytesLength;
						public int type;
						public string typeText;
						public int subtype;
						public int imageWidth;
						public int imageHeight;
						public bool isGS1;
						public MWLocation locationPoints;
						
						public MWResult(){
							text = null;
							bytes = null;
							encryptedResult = null;
							bytesLength = 0;
							type = 0;
							typeText = "";
							subtype = 0;
							isGS1 = false;
							locationPoints = null;
							imageWidth = 0;
							imageHeight = 0;
						}
						
					}
					
					public class MWResults{
						
						public int version;
						public IList<MWResult> results;
						
						public int count;
						
						public MWResults(byte[] buffer){
							
							results = new List<MWResult>();
							count = 0;
							version = 0;
							
							if (buffer[0] != 'M' || buffer[1] != 'W' || buffer[2] != 'R'){
								return;
							}
							
							version = buffer[3];
							
							count = buffer[4];
							
							int currentPos = 5;
							
							for (int i = 0; i < count; i++){
								
								MWResult result = new MWResult();
								
								int fieldsCount = buffer[currentPos];
								currentPos++;
								for (int f = 0; f < fieldsCount; f++){
									int fieldType = buffer[currentPos];
									int fieldNameLength = buffer[currentPos + 1];
									int fieldContentLength = 256 * (buffer[currentPos + 3 + fieldNameLength] & 0xFF) + (buffer[currentPos + 2 + fieldNameLength]& 0xFF);
									
									int contentPos = currentPos + fieldNameLength + 4;
									float[] locations = new float[8];
									switch (fieldType) {
										
										case BarcodeConfig.MWB_RESULT_FT_TYPE:
											result.type = (int)Java.Nio.ByteBuffer.Wrap (buffer, contentPos, 4).Order (Java.Nio.ByteOrder.LittleEndian).Int;
										break;
										case BarcodeConfig.MWB_RESULT_FT_SUBTYPE:	
											result.subtype = Java.Nio.ByteBuffer.Wrap (buffer, contentPos, 4).Order (Java.Nio.ByteOrder.LittleEndian).Int;
										break;
										case BarcodeConfig.MWB_RESULT_FT_ISGS1:
											result.isGS1 = (Java.Nio.ByteBuffer.Wrap(buffer, contentPos, 4).Order(Java.Nio.ByteOrder.LittleEndian).Int == 1);
										break;
										case BarcodeConfig.MWB_RESULT_FT_IMAGE_WIDTH:
											result.imageWidth = Java.Nio.ByteBuffer.Wrap(buffer, contentPos, 4).Order(Java.Nio.ByteOrder.LittleEndian).Int;
										break;
										case BarcodeConfig.MWB_RESULT_FT_IMAGE_HEIGHT:
											result.imageHeight = Java.Nio.ByteBuffer.Wrap(buffer, contentPos, 4).Order(Java.Nio.ByteOrder.LittleEndian).Int;
										break;
										case BarcodeConfig.MWB_RESULT_FT_LOCATION:
											for (int l = 0; l < 8; l++){
												locations[l] = Java.Nio.ByteBuffer.Wrap(buffer, contentPos + l * 4, 4).Order(Java.Nio.ByteOrder.LittleEndian).Float;
											}
											result.locationPoints =  new MWLocation(locations);
											
										break;
										case BarcodeConfig.MWB_RESULT_FT_TEXT:
											result.text = new Java.Lang.String(buffer, contentPos, fieldContentLength);
										break;
										case BarcodeConfig.MWB_RESULT_FT_BYTES:
											result.bytes = new byte[fieldContentLength];
											result.bytesLength = fieldContentLength;
											for (int c = 0; c < fieldContentLength; c++){
												result.bytes[c] = buffer[contentPos + c];
											}
											
										break;
										case BarcodeConfig.MWB_RESULT_FT_PARSER_BYTES:
											result.encryptedResult = new Java.Lang.String(buffer, contentPos, fieldContentLength);
											
										break;
											
											
										default:
										break;
									}
									
									currentPos += (fieldNameLength + fieldContentLength + 4);
									
								}
								
								
								results.Add(result);
								
							}
							
						}
						
						public MWResult getResult(int index){
							return results[index];
						}
						
						
						
					}
				}
				
					