using UnityEngine;
using System.Collections;

public class CircleMovement : MonoBehaviour {
    public GameObject Staff;
    float Distance;
    float Timer;
    public float CircleStartAmount;
    // Use this for initialization
    void Start() {
        Timer = CircleStartAmount * 2 * Mathf.PI;
        Distance = (Staff.transform.position - transform.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void FixedUpdate() {
        Timer += Time.deltaTime;
        transform.localPosition = new Vector3 (Mathf.Sin(Timer), Mathf.Cos(Timer), 0) * Distance;
    }
}
