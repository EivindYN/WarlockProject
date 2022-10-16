using UnityEngine;
using System.Collections;

public class LavaEffect : MonoBehaviour {
    public float AmountMove;
    public float PauseDuration;
    float PauseTimer;
    bool Moved;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void FixedUpdate() {
        PauseTimer++;
        if (PauseTimer > PauseDuration) {
            if (Moved)
                transform.position += new Vector3(AmountMove, 0, AmountMove);
            else
                transform.position -= new Vector3(AmountMove, 0, AmountMove);
            Moved = !Moved;
            PauseTimer = 0;
        }
    }
}
