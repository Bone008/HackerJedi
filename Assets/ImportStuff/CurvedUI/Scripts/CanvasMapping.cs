using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public abstract class CanvasMapping : MonoBehaviour
{
	// Take a point on the screen and map it to the canvas
	public abstract bool MapScreenToCanvas(Vector2 screenCoord, out Vector2 o_canvasCoord);

	// Set parameters on a material to perform the canvas to screen mapping
	public abstract void SetCanvasToScreenParameters(Material material);

	protected virtual void Awake()
	{
		m_canvas = GetComponent<Canvas>();
	}

	protected virtual void Reset()
	{
		SetMaterialsDirty();
	}

	protected virtual void OnValidate()
	{
		SetMaterialsDirty();
	}

	protected void SetMaterialsDirty()
	{
		Graphic[] graphics = GetComponentsInChildren<Graphic>();
		foreach (Graphic graphic in graphics)
		{
			graphic.SetMaterialDirty();
		}
	}

	protected Canvas m_canvas;
}
