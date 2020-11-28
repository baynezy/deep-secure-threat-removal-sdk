using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval.Comms
{
	/// <summary>
	/// Handles all requests to the Deep Secure Threat removal API.
	/// </summary>
	public interface IRequester
	{
		/// <summary>
		/// Call the instant threat removal API.
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <returns>The converted file returned from the Deep Secure Threat Removal API</returns>
		Task<ApiResponse> Sync(byte[] file, MimeType mimeType);

		/// <summary>
		/// Call the instant threat removal API.
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <param name="risks">The risks that are or are not accessptable for the transformation</param>
		/// <returns>The converted file returned from the Deep Secure Threat Removal API</returns>
		Task<ApiResponse> Sync(byte[] file, MimeType mimeType, RiskOptions risks);
	}
}