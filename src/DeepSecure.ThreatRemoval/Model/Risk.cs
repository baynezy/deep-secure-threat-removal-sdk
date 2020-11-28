using System.Runtime.Serialization;
using DeepSecure.ThreatRemoval.Extensions;

namespace DeepSecure.ThreatRemoval.Model
{
	/// <summary>
	/// Risks that can be used with <c>RiskOptions</c> to configure
	/// risk management for requests
	/// </summary>
	public enum Risk
	{
		[EnumMember(Value = "exe")]
		[StringValue("exe")]
		Exe,
		[EnumMember(Value = "exe/macro")]
		[StringValue("exe/macro")]
		ExeMacro,
		[EnumMember(Value = "exe/macro/ms")]
		[StringValue("exe/macro/ms")]
		ExeMacroMs,
		[EnumMember(Value = "exe/macro/ms/word")]
		[StringValue("exe/macro/ms/word")]
		ExeMacroMsWord,
		[EnumMember(Value = "exe/macro/ms/powerpoint")]
		[StringValue("exe/macro/ms/powerpoint")]
		ExeMacroMsPowerpoint,
		[EnumMember(Value = "exe/macro/ms/excel")]
		[StringValue("exe/macro/ms/excel")]
		ExeMacroMsExcel,
		[EnumMember(Value = "poly")]
		[StringValue("poly")]
		Poly,
		[EnumMember(Value = "poly/text")]
		[StringValue("poly/text")]
		PolyText,
		[EnumMember(Value = "poly/text/xml")]
		[StringValue("poly/text/xml")]
		PolyTextXml,
		[EnumMember(Value = "poly/text/json")]
		[StringValue("poly/text/json")]
		PolyTextJson,
		[EnumMember(Value = "structured")]
		[StringValue("structured")]
		Structured,
		[EnumMember(Value = "structured/no-schema")]
		[StringValue("structured/no-schema")]
		StructuredNoSchema,
		[EnumMember(Value = "structured/no-schema/xml")]
		[StringValue("structured/no-schema/xml")]
		StructuredNoSchemaXml,
		[EnumMember(Value = "structured/no-schema/json")]
		[StringValue("structured/no-schema/json")]
		StructuredNoSchemaJson

	}
}