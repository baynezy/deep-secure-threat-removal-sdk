using System;
using DeepSecure.ThreatRemoval.Model;

namespace DeepSecure.ThreatRemoval.Extensions
{
	public static class EnumExtension
	{
		/// <summary>
		/// Returns the <c>StringValue</c> attribute for this Enum
		/// </summary>
		public static string GetStringValue(this Enum value)
		{
			var type = value.GetType();
			var fieldInfo = type.GetField(value.ToString());
			var attributes = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];

			return attributes.Length > 0 ? attributes[0].StringValue : null;
		}

		/// <summary>
		/// Converts a string the the matching enum
		/// </summary>
		public static T ToEnum<T>(this string str)
		{
			foreach (T item in Enum.GetValues(typeof(T)))
			{
				StringValueAttribute[] attributes = (StringValueAttribute[])item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(StringValueAttribute), false);
				if ((attributes != null) && (attributes.Length > 0) && (attributes[0].StringValue.Equals(str)))
				{
					return item;
				}
			}
			return (T)Enum.Parse(typeof(T), str, true);
		}
	}
}