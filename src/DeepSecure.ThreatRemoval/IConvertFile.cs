using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval
{
	/// <summary>
	/// Converts files to a safe version where only items known to
	/// definitely be safe are returned. Leaving other items behind.
	/// </summary>
	public interface IConvertFile
	{
		/// <summary>
		/// Synchronously remove threats from a file
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <returns>The converted file with threats removed</returns>
		Task<SyncResponse> Sync(byte[] file, MimeType mimeType);

		/// <summary>
		/// Synchronously remove threats from a file
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <param name="risks">The risks that are or are not accessptable for the transformation</param>
		/// <returns>The converted file with threats removed</returns>
		Task<SyncResponse> Sync(byte[] file, MimeType mimeType, RiskOptions risks);
	}
}