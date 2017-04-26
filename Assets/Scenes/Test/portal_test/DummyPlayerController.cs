using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour {

    private bool focused = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.localPosition += transform.TransformDirection(new Vector3(h, 0, v) * 10 * Time.deltaTime);
        
        if (focused && Cursor.lockState == CursorLockMode.Locked)
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            var angles = transform.localEulerAngles;
            angles.y += x * 60 * Time.deltaTime;
            angles.x -= y * 60 * Time.deltaTime;

            // prevent bottom-up camera
            if (angles.x < 200)
                angles.x = Mathf.Min(angles.x, 89); // 0-90
            else
                angles.x = Mathf.Max(angles.x, 271); // 270-360

            transform.localEulerAngles = angles;
        }
    }

    void OnApplicationFocus(bool flag)
    {
        focused = flag;
        if (flag)
            Cursor.lockState = CursorLockMode.Locked;
    }
}
