using UnityEngine;
using System.Collections;

public class GlaiveSprite : MonoBehaviour {
	void Update () {
		Glaive Glaive = GetComponentInParent<Glaive> ();
		if (Glaive.First == true) {
			GetComponent<CapsuleCollider>().enabled = true;
		}
	}
}
