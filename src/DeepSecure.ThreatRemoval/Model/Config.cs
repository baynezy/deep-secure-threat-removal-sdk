using System;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Configuration parameters for <c>IRequester</c>
	/// </summary>
	public class Config : IConfig
	{
		public Config(string syncUrl, string apiKey)
		{
			ErrorIfNull(syncUrl, "SyncUrl");
			ErrorIfNull(apiKey, "ApiKey");
			SyncUrl = syncUrl;
			ApiKey = apiKey;
		}

		private void ErrorIfNull(object parameter, string propertyName)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(propertyName);
			}
		}

		/// <summary>
		/// The URL for the synchronous API of the Deep Secure Threat
		/// Removal API
		/// </summary>
		public string SyncUrl { get; }

		/// <summary>
		/// The API Key for the synchronous API of the Deep Secure Threat
		/// Removal API
		/// </summary>
		public string ApiKey { get;}
	}
}