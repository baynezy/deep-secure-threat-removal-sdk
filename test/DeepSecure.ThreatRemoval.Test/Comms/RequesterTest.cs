using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
			var returnedFile = File.ReadAllBytes(path);
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.OK, "application/pdf", new MemoryStream(returnedFile));
			var requester = CreateRequester(mockHttp);

			var cleanedFile = await requester.Sync(new byte[10], MimeType.ApplicationPdf);

			Assert.That(cleanedFile, Is.EqualTo(returnedFile));
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
			Assert.That(ex.InnerException, Is.TypeOf<HttpRequestException>());
		}

		[Test]
		public void Sync_WhenApiRespondsWith500_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.InternalServerError, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.That(ex.Message, Is.EqualTo("API Request Failed with a 500 response."));
		}

		[Test]
		public void Sync_WhenApiRespondsWith400_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.BadRequest, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
			Assert.That(ex.Message, Is.EqualTo("API Request Failed with a 400 response."));
		}

		[Test]
		public void Sync_WhenApiRespondsWith429_TheReturnThrowApiRequestExceptionError()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("*").Respond(HttpStatusCode.TooManyRequests, "application/json", new TestApiErrorResponse().ToString());
			var requester = CreateRequester(mockHttp);

			var ex = Assert.ThrowsAsync<ApiRequestException>(() => requester.Sync(new byte[10], MimeType.ApplicationPdf));
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
			private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions {
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				IgnoreNullValues = true
			};
			public string Message { get; internal set; }
			public string Path { get; internal set; }
			public int Code { get; internal set; }
			public string Name { get; internal set; }
			public string Type { get; internal set; }

			public override string ToString()
			{
				return JsonSerializer.Serialize(this, _serializerOptions);
			}
		}
	}
}