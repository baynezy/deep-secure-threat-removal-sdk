using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DeepSecure.ThreatRemoval.Comms;

/// <summary>
/// Handles all requests to the Deep Secure Threat removal API.
/// </summary>
public class Requester : IRequester
{
	private static readonly JsonSerializerOptions SerializerOptions = InitialiseSerialiserOptions();

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

	private const string RisksTakenHeader = "X-Risks-Taken";
	private static readonly IList<int> AcceptableNon200StatusCodes = new List<int>{400,429,500};
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
					{ HttpRequestHeader.Accept.ToString(), $"{mimeType.GetStringValue()}, application/json" },
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

				if (ExceptionHasUnexpectedStatusCode(statusCode))
				{
					throw;
				}

				var body = await response.Content.ReadAsStringAsync();
				var restApiResponse = JsonSerializer.Deserialize<RestApiResponse>(body, SerializerOptions);
				var apiErrorResponse = restApiResponse.Error;

				throw new ApiRequestException($"API Request Failed with a {statusCode} response.", apiErrorResponse, ex);
			}

			var apiResponse = new ApiResponse {
				File = await response.Content.ReadAsByteArrayAsync()
			};

			if (ResponseHasRisksTakenHeader(response))
			{
				AddRisksTakenToApiResponse(apiResponse, response);
			}

			return apiResponse;
		}

	private static void AddRisksTakenToApiResponse(ApiResponse apiResponse, HttpResponseMessage response)
	{
			if (!response.Headers.TryGetValues(RisksTakenHeader, out var values)) { return;}
			var risksTaken = new List<Risk>();
			values.First()
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.ToList()
				.ForEach(r => risksTaken.Add(r.ToEnum<Risk>()));
			apiResponse.RisksTaken = risksTaken;
		}

	private static bool ResponseHasRisksTakenHeader(HttpResponseMessage response)
	{
			return response.Headers.Contains(RisksTakenHeader);
		}

	private static void AddXOptionsHeader(HttpRequestMessage request, RiskOptions risks)
	{
			if (risks == null)
			{
				return;
			}

			var header = new XOptionsHeader{
				Risks = risks
			};

			request.Content?.Headers.Add("X-Options",
				JsonSerializer.Serialize(header, SerializerOptions));
		}

	private static bool ExceptionHasUnexpectedStatusCode(int statusCode)
	{
			return AcceptableNon200StatusCodes.IndexOf(statusCode) == -1;
		}

	private sealed class RestApiResponse
	{
		public ApiErrorResponse Error { get; init; }
	}

	private sealed class XOptionsHeader
	{
		public RiskOptions Risks { get; internal set; }
	}
}