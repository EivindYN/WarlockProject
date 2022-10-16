using UnityEngine;
using System.Collections;

public class Following : MonoBehaviour {
    Rigidbody Body;
    public GameObject StaffEnd;
    public bool InheritVelocity = true;
    public float AmountMove = 3;
	// Use this for initialization
	void Start () {
        Body = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void LateUpdate() {
        if (InheritVelocity) {
            Body.velocity = ((StaffEnd.transform.position - transform.position) * 15);
        }
        Body.transform.position = StaffEnd.transform.position;
    ParticleSystem.Particle[] p = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount + 1];
    int l = GetComponent<ParticleSystem>().GetParticles(p);
    int i = 0;
    while (i < l) {
            float SmallestMagnitude = Mathf.Min(Body.velocity.magnitude, 1);
        if (Body.velocity.magnitude == SmallestMagnitude) {
                p[i].velocity = -(Body.velocity) * AmountMove;
            } else {
                p[i].velocity = -(Body.velocity.normalized) * AmountMove;
            }
        i++;
    }

    GetComponent<ParticleSystem>().SetParticles(p, l);

    }
}
