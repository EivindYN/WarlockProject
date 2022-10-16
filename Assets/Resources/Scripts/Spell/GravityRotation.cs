using UnityEngine;
using System.Collections;

public class GravityRotation : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		transform.Rotate (0, 0, -4);
	}
}
