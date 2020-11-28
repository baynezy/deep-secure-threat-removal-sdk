using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		private static JsonSerializerOptions _serializerOptions = InitialiseSerialiserOptions();

		private static JsonSerializerOptions InitialiseSerialiserOptions()
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			options.Converters.Add(new JsonStringEnumConverterWithAttributeSupport());

			return options;
		}

		private static readonly string _risksTakenHeader = "X-Risks-Taken";
		private static readonly IList<int> _acceptableNon200StatusCodes = new List<int>{400,429,500};
		private readonly HttpClient _client;
		private readonly IConfig _config;

		/// <summary>
		/// Create a new Requester instance for querying the Deep Secure API
		/// </summary>
		/// <param name="client"><c>HttpClient</c> if you want to provide your own in a wider application</param>
		/// <param name="config"><c>IConfig</c> instance containing the parameters for the <c>Requester</c></param>
		public Requester(HttpClient client, IConfig config)
		{
			_client = client;
			_config = config;
		}

		/// <summary>
		///Create a new Requester instance for querying the Deep Secure API
		/// </summary>
		/// <param name="config"><c>IConfig</c> instance containing the parameters for the <c>Requester</c></param>
		/// <returns></returns>
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
		public async Task<ApiResponse> Sync(byte[] file, MimeType mimeType)
		{
			return await Sync(file, mimeType, null).ConfigureAwait(false);
		}

		/// <summary>
		/// Call the instant threat removal API.
		/// </summary>
		/// <param name="file">File to be converted to safe version</param>
		/// <param name="mimeType">The <c>MimeType</c> of the <c>file</c> parameter.</param>
		/// <param name="risks">The <c>RiskOptions</c> for the request. See https://threat-removal.deep-secure.com/api/instant/documentation</param>
		/// <returns>The converted file returned from the Deep Secure Threat Removal API</returns>
		/// <remarks>If the API returns a non-200 response then can throw either an <c>ApiRequestException</c> or <c>HttpRequestException</c></remarks>
		public async Task<ApiResponse> Sync(byte[] file, MimeType mimeType, RiskOptions risks)
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

			AddXOptionsHeader(request, risks);

			var response = await _client.SendAsync(request);

			try
			{
				response.EnsureSuccessStatusCode();
			}
			catch (HttpRequestException ex)
			{
				var statusCode = (int)response.StatusCode;

				if (exceptionHasUnexpectedStatusCode(statusCode))
				{
					throw;
				}

				var body = await response.Content.ReadAsStringAsync();
				var restApiResponse = JsonSerializer.Deserialize<RestApiResponse>(body, _serializerOptions);
				var apiErrorResponse = restApiResponse.Error;

				throw new ApiRequestException(String.Format("API Request Failed with a {0} response.", statusCode), apiErrorResponse, ex);
			}

			var apiResponse = new ApiResponse {
				File = await response.Content.ReadAsByteArrayAsync()
			};

			if (responseHasRisksTakenHeader(response))
			{
				addRisksTakenToApiResponse(apiResponse, response);
			}

			return apiResponse;
		}

		private void addRisksTakenToApiResponse(ApiResponse apiResponse, HttpResponseMessage response)
		{
			IEnumerable<string> values;
			if (response.Headers.TryGetValues(_risksTakenHeader, out values))
			{
				var risksTaken = new List<Risk>();
				values.First()
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.ToList()
					.ForEach(r => risksTaken.Add(r.ToEnum<Risk>()));
				apiResponse.RisksTaken = risksTaken;
			}
		}

		private bool responseHasRisksTakenHeader(HttpResponseMessage response)
		{
			return response.Headers.Contains(_risksTakenHeader);
		}

		private void AddXOptionsHeader(HttpRequestMessage request, RiskOptions risks)
		{
			if (risks == null)
			{
				return;
			}

			XOptionsHeader header = new XOptionsHeader{
				Risks = risks
			};

			request.Content.Headers.Add("X-Options", JsonSerializer.Serialize<XOptionsHeader>(header, _serializerOptions));
		}

		private bool exceptionHasUnexpectedStatusCode(int statusCode)
		{
			return _acceptableNon200StatusCodes.IndexOf(statusCode) == -1;
		}

		private class RestApiResponse
		{
			public ApiErrorResponse Error { get; set; }
		}

		private class XOptionsHeader
		{
			public RiskOptions Risks { get; internal set; }
		}
	}
}