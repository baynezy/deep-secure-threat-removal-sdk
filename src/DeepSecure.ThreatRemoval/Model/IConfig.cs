namespace DeepSecure.ThreatRemoval.Model;

/// <summary>
/// Configuration parameters for <c>IRequester</c>
/// </summary>
public interface IConfig
{
	/// <summary>
	/// The URL for the synchronous API of the Deep Secure Threat
	/// Removal API
	/// </summary>
	string SyncUrl { get; }

	/// <summary>
	/// The API Key for the synchronous API of the Deep Secure Threat
	/// Removal API
	/// </summary>
	string ApiKey { get; }
}