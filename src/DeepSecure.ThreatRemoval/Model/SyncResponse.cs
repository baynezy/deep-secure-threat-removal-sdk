using System.Collections.Generic;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Response from the instant Deep Secure Threat
	/// Removal API
	/// </summary>
	public class SyncResponse
	{
		/// <summary>
		/// Response for synchronous file threat removal requests
		/// </summary>
		/// <param name="file">The file after threat removal</param>
		/// <param name="risksTaken">Any risks taken when removing threats</param>
		public SyncResponse(byte[] file, IList<Risk> risksTaken)
		{
			File = file;
			RisksTaken = risksTaken;
		}

		/// <summary>
		/// The file after threat removal
		/// </summary>
		public byte[] File { get; }
		/// <summary>
		/// Any risks taken when removing threats
		/// </summary>
		public IList<Risk> RisksTaken { get; }
	}
}