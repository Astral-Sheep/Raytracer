using Godot;

namespace Astral.Tools;

public static class GodotUtility
{
	public static T GetSetting<[MustBeVariant] T>(string pSetting, T pDefaultValue = default)
	{
		return ProjectSettings.GetSetting(pSetting, Variant.From(pDefaultValue)).As<T>();
	}
}