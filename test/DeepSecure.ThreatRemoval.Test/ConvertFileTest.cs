using System;
using System.Collections.Generic;
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
			const string path = @"../../../Fixtures/clean-file.pdf";
			var file = await File.ReadAllBytesAsync(path).ConfigureAwait(false);;
			const MimeType mimeType = MimeType.ApplicationPdf;
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>())).ReturnsAsync(new ApiResponse {File=Array.Empty<byte>()});
			var converter = CreateConverter(mockRequester.Object);

			await converter.Sync(file, mimeType);

			mockRequester.Verify(f => f.Sync(file, mimeType), Times.Once);
		}

		[Test]
		public async Task Sync_WhenConvertingAFile_ThenFileReturnedMustMatchFileFromApi()
		{
			var dummyFile = Array.Empty<byte>();
			const string path = @"../../../Fixtures/clean-file.pdf";
			var returnedFile = await File.ReadAllBytesAsync(path).ConfigureAwait(false);;
			const MimeType mimeType = MimeType.ApplicationPdf;
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>())).ReturnsAsync(new ApiResponse{File = returnedFile});
			var converter = CreateConverter(mockRequester.Object);

			var response = await converter.Sync(dummyFile, mimeType);

			Assert.That(response.File, Is.EqualTo(returnedFile));
		}

		[Test]
		public async Task Sync_WhenConvertingAFileWithRisks_ThenRisksMustBePassedToRequester()
		{
			var dummyFile = new byte[10];
			const MimeType mimeType = MimeType.ApplicationPdf;
			var risks = new RiskOptions {
				Allow = new List<Risk> {
					Risk.Exe,
					Risk.ExeMacro
				}
			};
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>(), risks)).ReturnsAsync(new ApiResponse{File = new byte[11]});
			var converter = CreateConverter(mockRequester.Object);

			await converter.Sync(dummyFile, mimeType, risks);
			mockRequester.Verify(f => f.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>(), risks), Times.Once);
		}

		[Test]
		public async Task Sync_WhenReceivingRisksMetadataInTheResponse_ThenMetadataMustBeInTheSyncResponse()
		{
			var dummyFile = new byte[10];
			const MimeType mimeType = MimeType.ApplicationPdf;
			var risksTaken = new List<Risk>();
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>())).ReturnsAsync(new ApiResponse{File = new byte[11], RisksTaken = risksTaken});
			var converter = CreateConverter(mockRequester.Object);

			var response = await converter.Sync(dummyFile, mimeType);

			Assert.That(response.RisksTaken, Is.EqualTo(risksTaken));
		}

		[Test]
		public async Task Sync_ConvertingAFileWithRisksAndReceivingRisksMetadataInTheResponse_ThenMetadataAndFileMustBeInTheSyncResponse()
		{
			var dummyFile = new byte[10];
			var returnedFile = new byte[11];
			const MimeType mimeType = MimeType.ApplicationPdf;
			var risksTaken = new List<Risk>();
			var risks = new RiskOptions {
				Allow = new List<Risk> {
					Risk.Exe,
					Risk.ExeMacro
				}
			};
			var mockRequester = new Mock<IRequester>();
			mockRequester.Setup(m => m.Sync(It.IsAny<byte[]>(), It.IsAny<MimeType>(), It.IsAny<RiskOptions>())).ReturnsAsync(new ApiResponse{File = returnedFile, RisksTaken = risksTaken});
			var converter = CreateConverter(mockRequester.Object);

			var response = await converter.Sync(dummyFile, mimeType, risks);

			Assert.That(response.RisksTaken, Is.EqualTo(risksTaken));
			Assert.That(response.File, Is.EqualTo(returnedFile));
		}

		private static IConvertFile CreateConverter(IRequester requester = null)
		{
			return new ConvertFile(requester ?? new Mock<IRequester>().Object);
		}
	}
}