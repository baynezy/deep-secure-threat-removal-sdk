namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// PoCo wrapping API error responses from the Deep Secure Threat Removal API
	/// </summary>
	public class ApiErrorResponse
	{
		/// <summary>
		/// Human readable description of the error
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The property that caused the error
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Error code - see error glossary https://threat-removal.deep-secure.com/api/errors
		/// </summary>
		public int Code { get; set; }

		/// <summary>
		/// Name of the error
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Category of the error
		/// </summary>
		public string Type { get; set; }
	}
}