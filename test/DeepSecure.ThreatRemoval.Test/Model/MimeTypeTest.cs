using DeepSecure.ThreatRemoval.Extensions;
using DeepSecure.ThreatRemoval.Model;
using NUnit.Framework;

namespace DeepSecure.ThreatRemoval.Test.Model
{
	[TestFixture]
	public class MimeTypeTest {
		[Test]
		public void ToEnum_WhenConvertingFromString_ThenReturnEnum()
		{
			var mimeType = "exe";
			Assert.That(mimeType.ToEnum<Risk>(), Is.EqualTo(Risk.Exe));
		}
	}
}