using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SinWaveMapping : CanvasMapping
{
	#region CanvasMapping

	public override bool MapScreenToCanvas(Vector2 screenCoord, out Vector2 o_canvasCoord)
	{
		Vector2 canvasSize = m_canvas.pixelRect.size;

		float theta = ((screenCoord.x / canvasSize.x) * m_scale) + m_phase;

		o_canvasCoord.x = screenCoord.x;
		o_canvasCoord.y = screenCoord.y - (Mathf.Sin(theta) * m_amplitude);
		return true;
	}

	public override void SetCanvasToScreenParameters(Material material)
	{
		material.SetFloat("SinWave_Scale", m_scale);
		material.SetFloat("SinWave_Phase", m_phase);
		material.SetFloat("SinWave_Amplitude", m_amplitude);
	}

	#endregion

	[SerializeField]
	[Range(0.0f, 200.0f)]
	float m_scale = 100.0f;

	[SerializeField]
	[Range(-Mathf.PI, Mathf.PI)]
	float m_phase = 0.0f;
	
	[SerializeField]
	[Range(0.0f, 20.0f)]
	float m_amplitude = 5.0f;
}
