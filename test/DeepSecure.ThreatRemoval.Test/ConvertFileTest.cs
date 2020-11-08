using System.IO;
using System.Threading.Tasks;
using DeepSecure.ThreatRemoval.Comms;
using DeepSecure.ThreatRemoval.Model;
using Moq;
using NUnit.Framework;

namespace DeepSecure.ThreatRemoval.Test
{
	[TestFixture]
	public class ConvertFileTest
	{
		[Test]
		public void ConvertFile_ImplementsIConvertFile()
		{
			var converter = CreateConverter();

			Assert.That(converter, Is.InstanceOf<IConvertFile>());
		}

		[Test]
		public async Task Sync_WhenConvertingAFile_ThenShouldCallRequesterSync()
		{
			var path = @"../../../Fixtures/clean-file.pdf";
			var file = File.ReadAllBytes(path);
			var mimeType = MimeType.ApplicationPdf;
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>())).ReturnsAsync(new byte[0]);
			var converter = CreateConverter(mockRequester.Object);

			await converter.Sync(file, mimeType);

			mockRequester.Verify(f => f.Sync(file, mimeType), Times.Once);
		}

		[Test]
		public async Task Sync_WhenConvertingAFile_ThenFileReturnedMustMatchFileFromApi()
		{
			var dummyFile = new byte[0];
			var path = @"../../../Fixtures/clean-file.pdf";
			var returnedFile = File.ReadAllBytes(path);
			var mimeType = MimeType.ApplicationPdf;
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>())).ReturnsAsync(returnedFile);
			var converter = CreateConverter(mockRequester.Object);

			var response = await converter.Sync(dummyFile, mimeType);

			Assert.That(response.File, Is.EqualTo(returnedFile));
		}

		private IConvertFile CreateConverter(IRequester requester = null)
		{
			return new ConvertFile(requester ?? new Mock<IRequester>().Object);
		}
	}
}