using UnityEngine;
using System.Collections;

public class GravityTiltRotation : MonoBehaviour {
	float timer;
	float TurnDuration = 2f;
	float TurnAmount = 1f;	

	void FixedUpdate () {
		timer += Time.deltaTime;
		if (timer < TurnDuration * 0.25f){
			transform.Rotate (0, 0, -TurnAmount);
		}
		if (timer > TurnDuration * 0.25f && timer < TurnDuration * 0.75f){
			transform.Rotate (0, 0, TurnAmount);
		}
		if (timer > TurnDuration * 0.75f) {
			transform.Rotate (0, 0, -TurnAmount);
		}
		if (timer > TurnDuration){
			timer = 0;
		}
	}
}
