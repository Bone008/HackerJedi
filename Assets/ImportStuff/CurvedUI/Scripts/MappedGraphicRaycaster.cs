using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasMapping))]
public class MappedGraphicRaycaster : GraphicRaycaster
{
	protected override void Awake()
	{
		base.Awake();
		m_mapping = GetComponent<CanvasMapping>();
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		// Remap position
		Vector2 remappedPosition;
		if (!m_mapping.MapScreenToCanvas(eventData.position, out remappedPosition))
		{
			// Invalid, do nothing
			return;
		}

		// Update event data
		eventData.position = remappedPosition;

		// Use base class raycast method
		base.Raycast(eventData, resultAppendList);
	}

	CanvasMapping m_mapping;
}
