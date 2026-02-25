using System.Collections.Generic;
using Godot;

namespace Astral.Tools;

public static class GodotUtility
{
	public static T GetSetting<[MustBeVariant] T>(string pSetting, T pDefaultValue = default)
	{
		return ProjectSettings.GetSetting(pSetting, Variant.From(pDefaultValue)).As<T>();
	}

	public static T FindNode<T>(this Node pWorldContext) where T : Node
	{
		if (pWorldContext?.GetTree()?.Root is not {} lRoot)
			return null;

		return lRoot.FindChild<T>();
	}

	public static T FindChild<T>(this Node pNode) where T : Node
	{
		if (pNode == null || pNode.GetChildCount() <= 0)
			return null;

		Queue<Node> lNodesToCheck = new Queue<Node>(pNode.GetChildren());

		while (lNodesToCheck.Count > 0)
		{
			Node lNode = lNodesToCheck.Dequeue();

			if (lNode is T lTypedChild)
			{
				return lTypedChild;
			}

			for (int i = 0; i < lNode.GetChildCount(); i++)
			{
				lNodesToCheck.Enqueue(lNode.GetChild(i));
			}
		}

		return null;
	}
}