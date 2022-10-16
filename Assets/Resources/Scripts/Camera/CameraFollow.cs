using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float smoothing = 5f;

	Vector3 offset;

	void FixedUpdate()
	{
        if (target != null) {
            offset = offset == new Vector3(0, 0, 0) ? transform.position - target.position : offset;
            Vector3 targetCamPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
	}
}
