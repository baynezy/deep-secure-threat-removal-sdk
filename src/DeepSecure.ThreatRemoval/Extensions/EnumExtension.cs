using System;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval.Extensions
{
	internal static class EnumExtension
	{
		internal static string GetStringValue(this Enum value)
		{
			var type = value.GetType();
			var fieldInfo = type.GetField(value.ToString());
			var attributes = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

			return attributes.Length > 0 ? attributes[0].StringValue : null;
		}
	}
}