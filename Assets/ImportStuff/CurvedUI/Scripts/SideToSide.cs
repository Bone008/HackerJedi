using UnityEngine;
using System.Collections;

public class SideToSide : MonoBehaviour
{
	void Start()
	{
		m_initialPosition = transform.localPosition;
	}

	void Update()
	{
		m_theta = Mathf.Repeat(m_theta + (m_rate * Time.deltaTime), 360.0f);

		Vector3 position = m_initialPosition;
		position.x += Mathf.Sin(m_theta * Mathf.Deg2Rad) * m_radius;
		transform.localPosition = position;
	}

	Vector3 m_initialPosition;
	float m_theta;

	[SerializeField]
	float m_radius = 50.0f;

	[SerializeField]
	float m_rate = 45.0f;
}
