using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MappedGraphicMaterialModifier : MonoBehaviour, IMaterialModifier
{
	#region IMaterialModifier

	public Material GetModifiedMaterial(Material material)
	{
		CanvasMapping mapping = GetComponentInParent<CanvasMapping>();
		if (mapping != null)
		{
			mapping.SetCanvasToScreenParameters(material);
		}

		return material;
	}

	#endregion
}
