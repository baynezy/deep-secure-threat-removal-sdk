using System.Collections.Generic;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Risk options for the request see https://threat-removal.deep-secure.com/api/instant/documentation
	/// </summary>
	public interface IRiskOptions
	{
		/// <summary>
		/// Set Allowed risks for request
		/// </summary>
		/// <value>Collection of risks</value>
		IList<Risk> Allow { get; set; }

		/// <summary>
		/// Set Denied risks for request
		/// </summary>
		/// <value>Collection of risks</value>
		IList<Risk> Deny { get; set; }
	}
}