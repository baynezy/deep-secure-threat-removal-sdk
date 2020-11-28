using System.Collections.Generic;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Model object to deserialise Deep Secure Threat Removal API
	/// JSON responses into an object
	/// </summary>
	public class ApiResponse
	{
		/// <summary>
		/// The converted file returned from the API
		/// </summary>
		public byte[] File { get; set; }
		/// <summary>
		/// The risks taken for this API request
		/// </summary>
		public IList<Risk> RisksTaken { get; set; }
	}
}