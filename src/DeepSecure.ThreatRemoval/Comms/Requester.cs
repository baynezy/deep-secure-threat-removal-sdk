using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Extensions;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval.Comms
{
	/// <summary>
	/// Handles all requests to the Deep Secure Threat removal API.
	/// </summary>
	public class Requester : IRequester
	{
		private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions {
				PropertyNameCaseInsensitive = true
			};
		private static IList<int> _acceptableNon200StatusCodes = new List<int>{400,429,500};
		private readonly HttpClient _client;
		private readonly IConfig _config;

		public Requester(HttpClient client, IConfig config)
		{
			_client = client;
			_config = config;
		}

		public Requester(IConfig config) : this(new HttpClient(), config)
		{

		}

		/// <summary>
		/// Call the instant threat removal API.
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <returns>The converted file returned from the Deep Secure Threat Removal API</returns>
		/// <remarks>If the API returns a non-200 response then can throw either an <c>ApiRequestException</c> or <c>HttpRequestException</c></remarks>
		public async Task<byte[]> Sync(byte[] file, MimeType mimeType)
		{
			var content = new ByteArrayContent(file);
			content.Headers.ContentType = new MediaTypeHeaderValue(mimeType.GetStringValue());
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(_config.SyncUrl),
				Headers = {
					{ HttpRequestHeader.Accept.ToString(), String.Format("{0}, application/json", mimeType.GetStringValue()) },
					{ "x-api-key", _config.ApiKey }
				},
				Content = content
			};

			var response = await _client.SendAsync(request);

			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException ex) {
				var statusCode = (int)response.StatusCode;

				if (exceptionHasUnexpectedStatusCode(statusCode))
				{
					throw;
				}

				var body = await response.Content.ReadAsStringAsync();
				var apiErrorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(body, _serializerOptions);

				throw new ApiRequestException(String.Format("API Request Failed with a {0} response.", statusCode), apiErrorResponse, ex);
			}

			return await response.Content.ReadAsByteArrayAsync();
		}

		private bool exceptionHasUnexpectedStatusCode(int statusCode)
		{
			return _acceptableNon200StatusCodes.IndexOf(statusCode) == -1;
		}
	}
}