namespace DeepSecure.ThreatRemoval.Test.Model;

[TestFixture]
public class MimeTypeTest {
	[Test]
	public void ToEnum_WhenConvertingFromString_ThenReturnEnum()
	{
			const string mimeType = "exe";
			Assert.That(mimeType.ToEnum<Risk>(), Is.EqualTo(Risk.Exe));
		}
}