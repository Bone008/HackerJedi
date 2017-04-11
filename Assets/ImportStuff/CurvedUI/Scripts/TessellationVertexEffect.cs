using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TessellationVertexEffect : BaseVertexEffect
{
	#region BaseVertexEffect

	public override void ModifyVertices(List<UIVertex> verts)
	{
		if (verts.Count == 0)
		{
			// Nothing to do...
			return;
		}

		// Assume we are getting quads
		if ((verts.Count % 4) != 0)
		{
			Debug.LogError("Modifier expects vertices to be arranged as quads");
			return;
		}

		// Tessellate
		int startingVertexCount = verts.Count;
		for (int i = 0; i < startingVertexCount; i += 4)
		{
			TessellateQuad(verts, i);
		}

		// Remove old quads from the start of the list
		verts.RemoveRange(0, startingVertexCount);
	}

	#endregion

	void TessellateQuad(List<UIVertex> verts, int vertexIndex)
	{
		// Read the existing quad vertices
		UIVertex v0 = verts[vertexIndex];
		UIVertex v1 = verts[vertexIndex + 1];
		UIVertex v2 = verts[vertexIndex + 2];
		UIVertex v3 = verts[vertexIndex + 3];

		// Position deltas, A and B are the local quad up and right axes
		Vector3 dPdA = v2.position - v1.position;
		Vector3 dPdB = v1.position - v0.position;

		// Determine how many tiles there should be
		float rcpTessSize = 1.0f / Mathf.Max(1.0f, m_tessellationSize);
		int aQuads = Mathf.CeilToInt(dPdA.magnitude * rcpTessSize);
		int bQuads = Mathf.CeilToInt(dPdB.magnitude * rcpTessSize);

		// Build the sub quads
		float rcpAQuads = 1.0f / (float)aQuads;
		float rcpBQuads = 1.0f / (float)bQuads;
		float startBProp = 0.0f;
		for (int b = 0; b < bQuads; ++b)
		{
			float endBProp = (float)(b + 1) * rcpBQuads;
			float startAProp = 0.0f;
			for (int a = 0; a < aQuads; ++a)
			{
				float endAProp = (float)(a + 1) * rcpAQuads;

				// Append new quad to list
				verts.Add(Bilerp(v0, v1, v2, v3, startAProp, startBProp));
				verts.Add(Bilerp(v0, v1, v2, v3, startAProp, endBProp));
				verts.Add(Bilerp(v0, v1, v2, v3, endAProp, endBProp));
				verts.Add(Bilerp(v0, v1, v2, v3, endAProp, startBProp));

				startAProp = endAProp;
			}
			startBProp = endBProp;
		}
	}

	#region Interpolation

	// TODO: This could all be optimised by calculating the four weighting factors once
	// and re-using the result for all attributes

	UIVertex Bilerp(UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3, float a, float b)
	{
		UIVertex output;
		output.position = Bilerp(v0.position, v1.position, v2.position, v3.position, a, b);
		output.normal = Bilerp(v0.normal, v1.normal, v2.normal, v3.normal, a, b);

		// Bilerping w is almost certainly not the right thing to do here
		output.tangent = Bilerp(v0.tangent, v1.tangent, v2.tangent, v3.tangent, a, b);

		output.uv0 = Bilerp(v0.uv0, v1.uv0, v2.uv0, v3.uv0, a, b);
		output.uv1 = Bilerp(v0.uv1, v1.uv1, v2.uv1, v3.uv1, a, b);
		output.color = Bilerp(v0.color, v1.color, v2.color, v3.color, a, b);
		return output;
	}

	float Bilerp(float v0, float v1, float v2, float v3, float a, float b)
	{
		float top = Mathf.Lerp(v1, v2, a);
		float bottom = Mathf.Lerp(v0, v3, a);
		return Mathf.Lerp(bottom, top, b);
	}

	Vector2 Bilerp(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float a, float b)
	{
		Vector2 top = Vector2.Lerp(v1, v2, a);
		Vector2 bottom = Vector2.Lerp(v0, v3, a);
		return Vector2.Lerp(bottom, top, b);
	}

	Vector3 Bilerp(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float a, float b)
	{
		Vector3 top = Vector3.Lerp(v1, v2, a);
		Vector3 bottom = Vector3.Lerp(v0, v3, a);
		return Vector3.Lerp(bottom, top, b);
	}

	Vector4 Bilerp(Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3, float a, float b)
	{
		Vector4 top = Vector4.Lerp(v1, v2, a);
		Vector4 bottom = Vector4.Lerp(v0, v3, a);
		return Vector4.Lerp(bottom, top, b);
	}

	Color Bilerp(Color v0, Color v1, Color v2, Color v3, float a, float b)
	{
		Color top = Color.Lerp(v1, v2, a);
		Color bottom = Color.Lerp(v0, v3, a);
		return Color.Lerp(bottom, top, b);
	}

	#endregion
	
	[SerializeField]
	float m_tessellationSize = 10.0f;
}
