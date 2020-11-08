using System;

namespace DeepSecure.ThreatRemoval.Extensions
{
	/// <summary>
	/// Attribute to allow enums to have a string value rather than
	/// just an label name
	/// </summary>
	public class StringValueAttribute : Attribute
	{
		public string StringValue { get; protected set; }

		public StringValueAttribute(string value) {
            this.StringValue = value;
        }
	}
}