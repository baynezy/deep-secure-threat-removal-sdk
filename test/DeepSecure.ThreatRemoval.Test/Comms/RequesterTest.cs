using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Comms;
using DeepSecure.ThreatRemoval.Model;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace DeepSecure.ThreatRemoval.Test.Comms
{
	[TestFixture]
	public class RequesterTest
	{
		[Test]
		public void Requester_ImplementsIRequester() {
			var requester = CreateRequester();

			Assert.That(requester, Is.InstanceOf<IRequester>());
		}

		[Test]
		public async Task Sync_WhenApiRespondsWith200_TheReturnFileShouldMatchAsync() {
			var path = @"../../../Fixtures/clean-file.pdf";
			var returnedFile = await File.ReadAllBytesAsync(path);
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.OK, "application/pdf", new MemoryStream(returnedFile));
			var requester = CreateRequester(mockHttp);

			var response = await requester.Sync(new byte[10], MimeType.ApplicationPdf);

			Assert.That(response.File, Is.EqualTo(returnedFile));
		}

		[Test]
		public void Sync_WhenApiRespondsWith500_ThenParseApiResponse()
		{
			var errorResponse = new TestApiErrorResponse{
				Message = "Something went wrong please try again later",
				Path = "",
				Code = 1234,
				Name = "Internal Server Error",
				Type = "Serverside Error"
			};
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.InternalServerError, "application/json", errorResponse.ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.NotNull(ex);
			Assert.That(ex.ApiErrorResponse.Code, Is.EqualTo(errorResponse.Code));
			Assert.That(ex.ApiErrorResponse.Message, Is.EqualTo(errorResponse.Message));
			Assert.That(ex.ApiErrorResponse.Name, Is.EqualTo(errorResponse.Name));
			Assert.That(ex.ApiErrorResponse.Path, Is.EqualTo(errorResponse.Path));
			Assert.That(ex.ApiErrorResponse.Type, Is.EqualTo(errorResponse.Type));
		}

		[Test]
		public void Sync_WhenApiRespondsWith500_ThenReturnHttpRequestExceptionAsInnerException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.InternalServerError, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.NotNull(ex);
			Assert.That(ex.InnerException, Is.TypeOf<HttpRequestException>());
		}

		[Test]
		public void Sync_WhenApiRespondsWith500_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.InternalServerError, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.NotNull(ex);
			Assert.That(ex.Message, Is.EqualTo("API Request Failed with a 500 response."));
		}

		[Test]
		public void Sync_WhenApiRespondsWith400_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.BadRequest, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.NotNull(ex);
			Assert.That(ex.Message, Is.EqualTo("API Request Failed with a 400 response."));
		}

		[Test]
		public void Sync_WhenApiRespondsWith429_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.TooManyRequests, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.NotNull(ex);
			Assert.That(ex.Message, Is.EqualTo("API Request Failed with a 429 response."));
		}

		[Test]
		public void Sync_WhenApiRespondsWithUnexpectedCode_ThenRethrowException()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.Conflict, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			Assert.ThrowsAsync<HttpRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
		}

		[Test]
		public void Sync_WhenPassingRiskAllow_ThenShouldPassAsXOptionsHeader()
		{
			const string expectedHeader = "{\"risks\":{\"allow\":[\"exe\"],\"deny\":null}}";
			var config = new Config ("http://localhost/my/sync/url", "qwerty");
			var mockHttp = new MockHttpMessageHandler();
			var risks = new RiskOptions {
				Allow = new List<Risk> {
					Risk.Exe
				}
			};
			var mockedRequest = mockHttp.Expect(config.SyncUrl)
					.WithHeaders("X-Options", expectedHeader);
			var requester = CreateRequester(mockHttp, config);

			requester.Sync(new byte[10], MimeType.ApplicationPdf, risks);

			Assert.That(mockHttp.GetMatchCount(mockedRequest), Is.EqualTo(1),
				$"Request did not match signature. Expected: {expectedHeader}");
		}

		[Test]
		public void Sync_WhenPassingRiskDeny_ThenShouldPassAsXOptionsHeader()
		{
			const string expectedHeader = "{\"risks\":{\"allow\":null,\"deny\":[\"exe\"]}}";
			var config = new Config ("http://localhost/my/sync/url", "qwerty");
			var mockHttp = new MockHttpMessageHandler();
			var risks = new RiskOptions {
				Deny = new List<Risk> {
					Risk.Exe
				}
			};
			var mockedRequest = mockHttp.Expect(config.SyncUrl)
					.WithHeaders("X-Options", expectedHeader);
			var requester = CreateRequester(mockHttp, config);

			requester.Sync(new byte[10], MimeType.ApplicationPdf, risks);

			Assert.That(mockHttp.GetMatchCount(mockedRequest), Is.EqualTo(1),
				$"Request did not match signature. Expected: {expectedHeader}");
		}

		[Test]
		public void Sync_WhenPassingRiskBothAllowAndDeny_ThenShouldPassAsXOptionsHeader()
		{
			const string expectedHeader = "{\"risks\":{\"allow\":[\"exe\"],\"deny\":[\"exe/macro\"]}}";
			var config = new Config ("http://localhost/my/sync/url", "qwerty");
			var mockHttp = new MockHttpMessageHandler();
			var risks = new RiskOptions {
				Allow = new List<Risk> {
					Risk.Exe
				},
				Deny = new List<Risk> {
					Risk.ExeMacro
				}
			};
			var mockedRequest = mockHttp.Expect(config.SyncUrl)
					.WithHeaders("X-Options", expectedHeader);
			var requester = CreateRequester(mockHttp, config);

			requester.Sync(new byte[10], MimeType.ApplicationPdf, risks);

			Assert.That(mockHttp.GetMatchCount(mockedRequest), Is.EqualTo(1),
				$"Request did not match signature. Expected: {expectedHeader}");
		}

		[Test]
		public async Task Sync_WhenReturningRiskTakenHeader_ThenShouldBeInApiResponseAsync()
		{
			var path = @"../../../Fixtures/clean-file.pdf";
			var returnedFile = await File.ReadAllBytesAsync(path);
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.OK, new[] { new KeyValuePair<string, string>("X-Risks-Taken", "exe/macro/ms")}, "application/json", new MemoryStream(returnedFile));
			var requester = CreateRequester(mockHttp);

			var response = await requester.Sync(new byte[10], MimeType.ApplicationPdf);

			Assert.That(response.RisksTaken, Is.Not.Null);
			Assert.That(response.RisksTaken.Count, Is.EqualTo(1), "RisksTaken contains incorrect number of risks");
			Assert.That(response.RisksTaken.Contains(Risk.ExeMacroMs), Is.True, "Must contain the 'ExeMacroMs' risk");
		}

		[Test]
		public async Task Sync_WhenReturningRisksTakenHeader_ThenShouldBeInApiResponseAsync()
		{
			var path = @"../../../Fixtures/clean-file.pdf";
			var returnedFile = await File.ReadAllBytesAsync(path);
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.OK, new[] { new KeyValuePair<string, string>("X-Risks-Taken", "exe/macro/ms,exe")}, "application/json", new MemoryStream(returnedFile));
			var requester = CreateRequester(mockHttp);

			var response = await requester.Sync(new byte[10], MimeType.ApplicationPdf);

			Assert.That(response.RisksTaken, Is.Not.Null);
			Assert.That(response.RisksTaken.Count, Is.EqualTo(2), "RisksTaken contains incorrect number of risks");
			Assert.That(response.RisksTaken.Contains(Risk.ExeMacroMs), Is.True, "Must contain the 'ExeMacroMs' risk");
			Assert.That(response.RisksTaken.Contains(Risk.Exe), Is.True, "Must contain the 'Exe' risk");
		}

		private IRequester CreateRequester(HttpMessageHandler messageHandler = null, IConfig @config = null)
		{
			var client = (messageHandler == null) ? new HttpClient() : new HttpClient(messageHandler);
			return new Requester(client, config ?? MockConfig());
		}

		private IConfig MockConfig()
		{
			var mockConfig = new Mock<IConfig>();
			mockConfig.SetupGet(m => m.SyncUrl).Returns("http://localhost");
			mockConfig.SetupGet(m => m.ApiKey).Returns("qwerty123");

			return mockConfig.Object;
		}

		private class TestApiErrorResponse
		{
			private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions {
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};
			public string Message { get; internal init; }
			public string Path { get; internal init; }
			public int Code { get; internal init; }
			public string Name { get; internal init; }
			public string Type { get; internal init; }

			public override string ToString()
			{
				return $"{{\"error\":{JsonSerializer.Serialize(this, SerializerOptions)}}}";
			}
		}
	}
}