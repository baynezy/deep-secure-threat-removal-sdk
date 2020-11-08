namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Response from the instant Deep Secure Threat
	/// Removal API
	/// </summary>
	public class SyncResponse
	{
		public SyncResponse(byte[] file)
		{
			File = file;
		}

		/// <summary>
		/// The file returned from the API
		/// </summary>
		public byte[] File { get; }
	}
}