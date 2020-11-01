using DeepSecure.ThreatRemoval.Extensions;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Enum representing the supported MimeTypes of the Deep Secure Threat
	/// Removal API
	/// </summary>
	public enum MimeType
	{
		[StringValue("application/pdf")]
		ApplicationPdf,

		[StringValue("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
		ApplicationVndOpenXmlFormatsOfficeDocumentPresentationMlPresentation,

		[StringValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
		ApplicationVndOpenXmlFormatsOfficeDocumentSpreadsheetMlSheet,

		[StringValue("image/gif")]
		ImageGif,

		[StringValue("image/bmp")]
		ImageBmp,

		[StringValue("image/jpeg")]
		ImageJpeg,

		[StringValue("image/jpg")]
		ImageJpg,

		[StringValue("image/jp2")]
		ImageJp2,

		[StringValue("image/png")]
		ImagePng,

		[StringValue("image/x-ms-bmp")]
		ImageXMsBmp,

		[StringValue("image/tiff")]
		ImageTiff,

		[StringValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
		ApplicationVndOpenXmlFormatsOfficeDocumentWordProcessinMmlDocument,

		[StringValue("application/msword")]
		ApplicationMsWord,

		[StringValue("application/vnd.ms-powerpoint")]
		ApplicationVndMsPowerpoint,

		[StringValue("application/vnd.ms-excel")]
		ApplicationVndMsExcel,

		[StringValue("application/vnd.oasis.opendocument.text")]
		ApplicationVndOasisOpenDocumentText,

		[StringValue("application/vnd.oasis.opendocument.presentation")]
		ApplicationVndOasisOpenDocumentPresentation,

		[StringValue("application/vnd.oasis.opendocument.spreadsheet")]
		ApplicationVndOasisOpenDocumentSpreadsheet
	}
}