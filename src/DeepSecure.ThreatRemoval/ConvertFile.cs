using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Comms;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval
{
	/// <summary>
	/// Converts files to a safe version where only items known to
	/// definitely be safe are returned. Leaving other items behind.
	/// </summary>
	public class ConvertFile : IConvertFile
	{
		private readonly IRequester _requester;

		/// <summary>
		/// Initializes a new instance of the <c>ConvertFile</c> class with a specified
		/// implementation of <c>IRequester</c> for handling comms to the API
		/// </summary>
		/// <param name="requester"></param>
		public ConvertFile(IRequester requester)
		{
			_requester = requester;
		}

		/// <summary>
		/// Synchronously remove threats from a file
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <returns>The converted file with threats removed</returns>
		public async Task<SyncResponse> Sync(byte[] file, MimeType mimeType)
		{
			var convertedFile = _requester.Sync(file, mimeType);

			return new SyncResponse(await convertedFile);
		}
	}
}