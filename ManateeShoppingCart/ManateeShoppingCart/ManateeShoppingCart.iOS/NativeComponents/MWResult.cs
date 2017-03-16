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
	public class MWResults: NSObject
	{


		public int version;
		public System.Collections.ArrayList results;
		public int count;



		public MWResults(byte[] buffer){

			if (buffer[0] != 'M' || buffer[1] != 'W' || buffer[2] != 'R'){

				return;
			}

			results = new System.Collections.ArrayList ();
			this.count = 0;

			version = buffer[3];

			int countIn = buffer[4];

			int currentPos = 5;

			for (int i = 0; i < countIn; i++){

				MWResult result = new MWResult ();

				int fieldsCount = buffer[currentPos];
				currentPos++;
				for (int f = 0; f < fieldsCount; f++){
					int fieldType = buffer[currentPos];
					int fieldNameLength = buffer[currentPos + 1];
					int fieldContentLength = 256 * buffer[currentPos + 3 + fieldNameLength] + buffer[currentPos + 2 + fieldNameLength];

//					const int floatSize = sizeof(float);

					int contentPos = currentPos + fieldNameLength + 4;
					float[] locations= new float[8];
					switch (fieldType) {
					case BarcodeConfig.MWB_RESULT_FT_TYPE:
						result.type =  bufferToInt(buffer,contentPos);
						result.typeName = getTypeName(result.type);
	
						break;
					case BarcodeConfig.MWB_RESULT_FT_SUBTYPE:
						result.subtype = bufferToInt(buffer,contentPos);
						break;
					case BarcodeConfig.MWB_RESULT_FT_ISGS1:
						result.isGS1 = (bufferToInt(buffer,contentPos) == 1);
						break;
					case BarcodeConfig.MWB_RESULT_FT_IMAGE_WIDTH:
						result.imageWidth = bufferToInt(buffer,contentPos);
						break;
					case BarcodeConfig.MWB_RESULT_FT_IMAGE_HEIGHT:
						result.imageHeight =bufferToInt(buffer,contentPos);
						break;

					case BarcodeConfig.MWB_RESULT_FT_LOCATION:
						for (int l = 0; l < 8; l++){
							byte[] locationBytes = new byte[sizeof(float)];
							Buffer.BlockCopy (buffer, contentPos + l * 4, locationBytes, 0, sizeof(float));
							locations[l] = System.BitConverter.ToSingle(locationBytes, 0);
						}
						result.locationPoints =new MWLocation(locations[0],locations[1],locations[2],locations[3],locations[4],locations[5],locations[6],locations[7]);
						break;
					case BarcodeConfig.MWB_RESULT_FT_TEXT:
						
						result.text = bufferToString (buffer, contentPos, fieldContentLength);
							break;
					case BarcodeConfig.MWB_RESULT_FT_BYTES:
						result.bytes = new byte[fieldContentLength];
						result.bytesLength = fieldContentLength;
						Buffer.BlockCopy (buffer, contentPos, result.bytes, 0, fieldContentLength);
						break;
					case BarcodeConfig.MWB_RESULT_FT_PARSER_BYTES:
						result.encryptedResult = new byte[fieldContentLength + 1];
						result.encryptedResult [fieldContentLength] = 0;
						Buffer.BlockCopy (buffer, contentPos, result.encryptedResult, 0, fieldContentLength);
						break;

					default:
						break;
					}

					currentPos += (fieldNameLength + fieldContentLength + 4);

				}

				results.Add (result);

			}
			this.count = countIn;



		}

		public string getTypeName(int typeID)
		{
			string typeName = "Unknown";
			switch (typeID) {
			case BarcodeConfig.FOUND_25_INTERLEAVED: typeName = "Code 25 Interleaved";break;
			case BarcodeConfig.FOUND_25_STANDARD: typeName = "Code 25 Standard";break;
			case BarcodeConfig.FOUND_128: typeName = "Code 128";break;
			case BarcodeConfig.FOUND_128_GS1: typeName = "Code 128 GS1";break;
			case BarcodeConfig.FOUND_39: typeName = "Code 39";break;
			case BarcodeConfig.FOUND_93: typeName = "Code 93";break;
			case BarcodeConfig.FOUND_AZTEC: typeName = "AZTEC";break;
			case BarcodeConfig.FOUND_DM: typeName = "Datamatrix";break;
			case BarcodeConfig.FOUND_QR: typeName = "QR";break;
			case BarcodeConfig.FOUND_EAN_13: typeName = "EAN 13";break;
			case BarcodeConfig.FOUND_EAN_8: typeName = "EAN 8";break;
			case BarcodeConfig.FOUND_NONE: typeName = "None";break;
			case BarcodeConfig.FOUND_RSS_14: typeName = "Databar 14";break;
			case BarcodeConfig.FOUND_RSS_14_STACK: typeName = "Databar 14 Stacked";break;
			case BarcodeConfig.FOUND_RSS_EXP: typeName = "Databar Expanded";break;
			case BarcodeConfig.FOUND_RSS_LIM: typeName = "Databar Limited";break;
			case BarcodeConfig.FOUND_UPC_A: typeName = "UPC A";break;
			case BarcodeConfig.FOUND_UPC_E: typeName = "UPC E";break;
			case BarcodeConfig.FOUND_PDF: typeName = "PDF417";break;
			case BarcodeConfig.FOUND_CODABAR: typeName = "Codabar";break;
			case BarcodeConfig.FOUND_DOTCODE: typeName = "Dotcode";break;
			case BarcodeConfig.FOUND_11: typeName = "Code 11";break;
			case BarcodeConfig.FOUND_MSI: typeName = "MSI Plessey";break;
			case BarcodeConfig.FOUND_25_IATA: typeName = "25 IATA"; break;
			case BarcodeConfig.FOUND_25_MATRIX: typeName = "25 Matrix"; break;
			case BarcodeConfig.FOUND_25_COOP: typeName = "25 Coop"; break;
			case BarcodeConfig.FOUND_25_INVERTED: typeName = "25 Inverted"; break;
			case BarcodeConfig.FOUND_QR_MICRO: typeName = "QR Micro"; break;
			case BarcodeConfig.FOUND_MAXICODE: typeName = "Maxicode"; break;
			case BarcodeConfig.FOUND_POSTNET: typeName = "Postnet"; break;
			case BarcodeConfig.FOUND_PLANET: typeName = "Planet"; break;
			case BarcodeConfig.FOUND_IMB: typeName = "IMB"; break;
			case BarcodeConfig.FOUND_ROYALMAIL: typeName = "Royal Mail"; break;
					
					


			}

			return typeName;
		}

		public MWResult resultAtIndex(int index){
			return (MWResult)results[index];
		}

		private string bufferToString(byte[] buffer, int start, int length){


			string value = "";
			for (int i = 0; i < length; i++) {
				value += ((char)buffer [start + i]);
			}


			return value;
		}


		private int bufferToInt(byte[] buffer, int start){

			int value = 0;
			for(int i=0;i<4;i++){
				if(i>0){
					value*=256;
				}
				value += buffer[start+3-i];
			}
			return value;
		}

	}

	public class MWResult: NSMutableDictionary
	{
		public MWResult(){}

		public string text;
		public byte[] bytes;
		public byte[] encryptedResult;
		public int bytesLength;
		public int type;
		public string typeName;
		public int subtype;
		public int imageWidth;
		public int imageHeight;
		public bool isGS1;
		public MWLocation locationPoints;

	}

	public class MWLocation:NSObject
	{
		public CGPoint[] points;
		public CGPoint p1;
		public CGPoint p2;
		public CGPoint p3;
		public CGPoint p4;


		public MWLocation(float x1,float y1,float x2,float y2,float x3,float y3,float x4,float y4){
			
			p1 = new CGPoint(x1, y1);
			p2 = new CGPoint(x2, y2);
			p3 = new CGPoint(x3, y3);
			p4 = new CGPoint(x4, y4);

			points = new CGPoint[4];
			points [0] = new CGPoint (x1, y1);
			points [1] = new CGPoint (x2, y2);
			points [2] = new CGPoint(x3, y3);
			points [3] = new CGPoint(x4, y4);

			//p4 = /new CGPoint(x4, y4);

		}

	}


}







