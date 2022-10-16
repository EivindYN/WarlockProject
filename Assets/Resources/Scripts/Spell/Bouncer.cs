using UnityEngine;
using System.Collections;
public class Bouncer : Spell {
	Rigidbody FireBody;
	float speed = 12.5f;
	float timer;
	bool stop;
	Vector3 Forward;
	Vector3 ForwardNew;
	AudioSource Bounce;
    Quaternion StartTransform;
    public Vector3 AimVector;
	// Use this for initializa;tion
	void Start () {
		FireBody = GetComponent<Rigidbody> ();
        StartTransform = transform.rotation;
		Forward = transform.forward;
		Forward.y = 0;
		FireBody.AddForce(AimVector.normalized * speed, ForceMode.VelocityChange); 
		Bounce = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void OnTriggerExit (Collider other){
		if (other.tag == "PlayerEnemy"){
			if (Forward.magnitude == 0){
				Forward = transform.forward;
				Forward.y = 0;
			}
			PlayerHealth PlayerHealth = other.GetComponent<PlayerHealth>();
			transform.LookAt(transform.position + FireBody.velocity);
			//FireBody.velocity = Vector3.zero;
			ForwardNew = transform.forward;
			ForwardNew.y = 0;
			PlayerHealth.FireForward = (Forward - ForwardNew).normalized * 3f; //Last Forward.
			PlayerHealth.TakeDamage(10,false);
			//FireBody.AddForce(Forward.normalized * speed, ForceMode.VelocityChange); 
			Bounce.Play ();
            transform.rotation = StartTransform;
		}
	}
	void FixedUpdate(){
		timer += Time.deltaTime;
		if (timer > 3f) {
			Destroy(gameObject);
		}
	}
}