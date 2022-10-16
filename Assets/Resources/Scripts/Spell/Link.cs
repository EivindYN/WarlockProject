using UnityEngine;
using System.Collections;

public class Link : MonoBehaviour {
	public Transform Sender;
	public GameObject Child;
	public GameObject PSChild;
	private GameObject target;
	private LineRenderer lineRend;
	private LineRenderer lineRend2;
	private float arcLength = 1.0f;
	private float arcVariation = 1.0f;
	private float inaccuracy = 0.2f;
	private bool First;
	Rigidbody FireBody;
	float speed = 12f;
	float timer;
	float Duration = 1.2f;
	bool Hit;
	ParticleSystem PS;
	GameObject PlayerHit;
	Rigidbody PlayerHitBody;
	PlayerHealth PH;
	float damagetimer;
	void Start () {
		lineRend = gameObject.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (1);
		lineRend2 = Child.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (1);
		PS = GetComponent<ParticleSystem> ();
	}
	void OnTriggerEnter (Collider other){
		if (First == false) {
			FireBody = GetComponent<Rigidbody> ();
			FireBody.AddForce (transform.forward * speed, ForceMode.VelocityChange); 
			transform.rotation = Quaternion.identity;
			First = true;
		}
		if (timer > Duration || Hit == true) {
			if (other.transform == Sender){
				Destroy (gameObject);
			}
		}
		if (other.tag == "PlayerEnemy") {
			if (Hit == false){
				Hit = true;
				Destroy(PSChild);
				PS.enableEmission = false;
				PlayerHit = other.gameObject;
				PlayerHitBody = other.GetComponent<Rigidbody>();
				transform.localScale = new Vector3 (3f,3f,3f);
				PH = other.GetComponent<PlayerHealth>();
			}
		}
	}
	void Update() {
		if (Hit == true) {
			transform.position = PlayerHit.transform.position;
			PlayerHitBody.AddForce ((Sender.position - transform.position).normalized * 30, ForceMode.VelocityChange); 
		}
		Vector3 lastPoint = transform.position;
		int i = 1;
		lineRend.SetPosition (0, transform.position);//make the origin of the LR the same as the transform
		lineRend2.SetPosition (0, transform.position);//make the origin of the LR the same as the transform
		while (Vector3.Distance(Sender.position, lastPoint) > 0.5f) {//was the last arc not touching the target?
			lineRend.SetVertexCount (i + 1);//then we need a new vertex in our line renderecr
			lineRend2.SetVertexCount (i + 1);//then we need a new vertex in our line renderecr
			Vector3 fwd = Sender.position - lastPoint;//gives the direction to our target from the end of the last arc
			fwd.Normalize ();//makes the direction to scale
			fwd = Randomize (fwd, inaccuracy);//we don't want a straight line to the target though
			fwd *= Random.Range (arcLength * arcVariation, arcLength);//nature is never too uniform
			fwd += lastPoint;//point + distance * direction = new point. this is where our new arc ends
			lineRend.SetPosition (i, fwd);//this tells the line renderer where to draw to
			lineRend2.SetPosition (i, fwd);//this tells the line renderer where to draw to
			i++;
			lastPoint = fwd;//so we know where we are starting from for the next arc
		}
		lineRend.SetVertexCount (i + 1);
		lineRend.SetPosition (i, Sender.position);
		lineRend2.SetVertexCount (i + 1);
		lineRend2.SetPosition (i, Sender.position);
	}
	
	private Vector3 Randomize (Vector3 newVector, float devation) {
		newVector += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * devation;
		newVector.Normalize();
		return newVector;
	}
	void FixedUpdate(){
		damagetimer += 1;
		if (Hit == true && damagetimer > 5) {
			PH.TakeDamage(0.2f);
			damagetimer = 0;
		}
		timer += Time.deltaTime;
		if (timer > Duration) {
			FireBody.velocity = speed * (Sender.position - transform.position).normalized;
		}
	}
}
