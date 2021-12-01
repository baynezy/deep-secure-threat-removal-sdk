using System;

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

			return fieldInfo != null && fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) is StringValueAttribute[] { Length: > 0 } attributes ? attributes[0].StringValue : null;
		}

		/// <summary>
		/// Converts a string the the matching enum
		/// </summary>
		public static T ToEnum<T>(this string str)
		{
			foreach (T item in Enum.GetValues(typeof(T)))
			{
				var attributes = (StringValueAttribute[])item.GetType().GetField(item.ToString() ?? string.Empty)?.GetCustomAttributes(typeof(StringValueAttribute), false);
				if ((attributes != null) && (attributes.Length > 0) && (attributes[0].StringValue.Equals(str)))
				{
					return item;
				}
			}
			return (T)Enum.Parse(typeof(T), str, true);
		}
	}
}