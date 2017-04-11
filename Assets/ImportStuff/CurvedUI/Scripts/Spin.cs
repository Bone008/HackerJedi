using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour
{
	void Update()
	{
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles += m_spinRate * Time.deltaTime;
		transform.localEulerAngles = localEulerAngles;
	}

	[SerializeField]
	Vector3 m_spinRate = new Vector3(0.0f, 0.0f, 45.0f);
}
