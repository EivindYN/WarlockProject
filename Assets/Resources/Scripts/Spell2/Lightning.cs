using UnityEngine;
using System.Collections;

public class Lightning : Spell {
	public Transform Sender;
	public GameObject Child;
	private GameObject target;
	private LineRenderer lineRend;
	private LineRenderer lineRend2;
	private float arcLength = 1.0f;
	private float arcVariation = 1.0f;
	private float inaccuracy = 0.2f;
	private bool First;
	Rigidbody FireBody;
	float timer;
	float Duration = 0.4f;
	PlayerHealth PH;
	Vector3 SenderPos;
	Vector3 MyPos;
	void Start () {
		lineRend = gameObject.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (1);
		lineRend2 = Child.GetComponent<LineRenderer> ();
		lineRend.SetVertexCount (1);
        if (GetComponent<PhotonView>().isMine) {
            GetComponent<PhotonView>().RPC("SetSenderPos", PhotonTargets.All, PhotonManager.Me.Player.transform.position);
        }
    }
	void OnTriggerEnter (Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player") {
                Vector3 FireForward = ((transform.position - Sender.position).normalized * 2 + (other.transform.position - transform.position).normalized).normalized * 3f;
                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 10;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Stay");
            }
        }
	}
	void Update() {
        if (SenderPos == null) {
            return;
        }
		MyPos = new Vector3 (transform.position.x, 0f, transform.position.z);
		Vector3 lastPoint = MyPos;
		int i = 1;
		lineRend.SetPosition (0, MyPos);//make the origin of the LR the same as the transform
		lineRend2.SetPosition (0, MyPos);//make the origin of the LR the same as the transform
		while (Vector3.Distance(SenderPos, lastPoint) > 0.5f) {//was the last arc not touching the target?
			lineRend.SetVertexCount (i + 1);//then we need a new vertex in our line renderecr
			lineRend2.SetVertexCount (i + 1);//then we need a new vertex in our line renderecr
			Vector3 fwd = SenderPos - lastPoint;//gives the direction to our target from the end of the last arc
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
		lineRend.SetPosition (i, SenderPos);
		lineRend2.SetVertexCount (i + 1);
		lineRend2.SetPosition (i, SenderPos);
	}
	
	private Vector3 Randomize (Vector3 newVector, float devation) {
		newVector += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * devation;
		newVector.Normalize();
		return newVector;
	}
	void FixedUpdate(){
		timer += Time.deltaTime;
		if (timer > Duration) {
			Destroy(gameObject);
		}
	}
    [PunRPC]
    void SetSenderPos(Vector3 Pos) {
        SenderPos = Pos;
    }
}

