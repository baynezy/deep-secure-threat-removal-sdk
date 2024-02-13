using System;

namespace DeepSecure.ThreatRemoval.Test.Model;

[TestFixture]
public class ConfigTest
{
	[Test]
	public void Config_ImplementsIConfig()
	{
			var config = CreateConfig();

			Assert.That(config, Is.InstanceOf<IConfig>());
		}

	[Test]
	public void Config_WhenConstructing_ThenSyncUrlIsMandatoryAndNotNullable()
	{
			const string syncUrl = null;
			const string apiKey = "qwerty123";

			var ex = Assert.Throws<ArgumentNullException>(() => CreateConfig(syncUrl: syncUrl, apiKey:apiKey));
			ex.Should().NotBeNull();
			ex!.Message.Should().Be("Value cannot be null. (Parameter 'SyncUrl')");
		}

	[Test]
	public void Config_WhenConstructing_ThenApiKeyIsMandatoryAndNotNullable()
	{
			const string syncUrl = "https://example.com";
			const string apiKey = null;

			var ex = Assert.Throws<ArgumentNullException>(() => CreateConfig(syncUrl: syncUrl, apiKey:apiKey));
			ex.Should().NotBeNull();
			ex!.Message.Should().Be("Value cannot be null. (Parameter 'ApiKey')");
		}

	[Test]
	public void SyncUrl_WhenSettingProperty_ThenShouldReturnSameValue()
	{
			const string syncUrl = "https://example.com";
			var config = CreateConfig(syncUrl: syncUrl);

			Assert.That(config.SyncUrl, Is.EqualTo(syncUrl));
		}

	[Test]
	public void ApiKey_WhenSettingProperty_ThenShouldReturnSameValue()
	{
			const string apiKey = "qwerty123";
			var config = CreateConfig(apiKey: apiKey);

			Assert.That(config.ApiKey, Is.EqualTo(apiKey));
		}

	private Config CreateConfig(string syncUrl = "http://localhost", string apiKey = "api-key-123")
	{
			return new Config(syncUrl, apiKey);
		}
}