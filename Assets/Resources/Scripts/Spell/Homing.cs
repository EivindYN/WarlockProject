using UnityEngine;
using System.Collections;
using System.Linq;

public class Homing : Spell {
	Rigidbody FireBody;
	public GameObject[] Players;
	int[] Length;
	int EnemyAmount;
	int num;
	float Timer;
	int minValue;
	int minIndex;
	Vector3 Force;
	Vector3 Forward;
    float Speed = 5f;
    // Use this for initializa;tion
    // Update is called once per frame
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 CurrentPosition = Vector3.zero;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncExpectedPosition = Vector3.zero;
    private Quaternion syncStartRotation;
    private Quaternion syncEndRotation;
    private bool syncAnim = false;
    Vector3 velocity;
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(GetComponent<Rigidbody>().position);
            stream.SendNext(velocity);
            stream.SendNext(transform.rotation);
        } else {

            Vector3 syncPosition = (Vector3)stream.ReceiveNext();
            Vector3 syncVelocity = (Vector3)stream.ReceiveNext();
            Quaternion syncRotation = (Quaternion)stream.ReceiveNext();


            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncExpectedPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = syncPosition;

            CurrentPosition = GetComponent<Rigidbody>().position;


            syncEndRotation = syncRotation;
            syncStartRotation = transform.rotation;


        }
    }


    void Start() {
        Destroy(gameObject, 6);
        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        FireBody = GetComponent<Rigidbody>();
        FireBody.velocity = Vector3.zero;
        FireBody.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
        Forward = transform.forward;
        transform.rotation = Quaternion.identity;
        EnemyAmount = GameObject.FindGameObjectsWithTag("Player").Length - 1;
        Length = new int[EnemyAmount];
        Players = new GameObject[EnemyAmount];
        Players = GameObject.FindGameObjectsWithTag("Player");
        System.Collections.Generic.List<GameObject> list = new System.Collections.Generic.List<GameObject>(Players);
        list.Remove(GameObject.Find("GameManager").GetComponent<PhotonManager>().Player);
        Players = list.ToArray();
    }

	void OnTriggerEnter (Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player") {
                Vector3 ObjectPositions = other.transform.position - transform.position;
                ObjectPositions.y = 0f;
                Vector3 FireForward = Vector3.zero;
                if (FireBody) {
                    FireForward = (ObjectPositions.normalized + (FireBody.velocity).normalized * 2).normalized * 3f;
                } else {
                    FireForward = (ObjectPositions.normalized + (transform.forward * Speed).normalized * 2).normalized * 3f;
                }

                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 10;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("Explode", PhotonTargets.All, Test);
            }
        }
	}

	void FixedUpdate () {
        if (!GetComponent<PhotonView>().isMine) {
            syncTime += Time.deltaTime;
            /*if (syncDelay > 0) {
                GetComponent<Rigidbody>().position = Vector3.Lerp(CurrentPosition, syncExpectedPosition, syncTime / syncDelay);
            } else {*/
            GetComponent<Rigidbody>().position = syncStartPosition;
            transform.rotation = syncEndRotation;
            return;
        }
        Timer += Time.deltaTime;
        num = 0;
		foreach (GameObject Enemy in Players) {
			Length[num] = (int)(Enemy.transform.position - transform.position).magnitude;	
			num += 1;
		}
        //Homing
        if (Length.Length > 0) {
            minValue = Length.Min();
            minIndex = Length.ToList().IndexOf(minValue);
            Force = (Players[minIndex].transform.position - transform.position);
            Force.y = 0;
        } else {
            Force = Vector3.zero;
        }
        Debug.Log(Force);
		if (Timer < 0.5f) {
			FireBody.AddForce (Force.normalized * 0.4f + Forward * 10f * (0.5f-Timer) * (0.5f-Timer), ForceMode.VelocityChange);
		} else {	
			//FireBody.AddForce (Force.normalized * 0.4f ?? , ForceMode.VelocityChange);
			FireBody.AddForce (Force.normalized * 0.6f, ForceMode.VelocityChange);
		}
		if (Timer < 2f) {
			//FireBody.velocity = (16f - Timer * 8f)  * FireBody.velocity.normalized; //1f
			//FireBody.velocity = (12f - Timer * 2f)  * FireBody.velocity.normalized; //2f
			FireBody.velocity = (10.5f - Timer * 2f)  * FireBody.velocity.normalized; //2f
		} else {
			//FireBody.velocity = 8f  * FireBody.velocity.normalized;
			FireBody.velocity = 6.5f  * FireBody.velocity.normalized;
		}

		//End Homing


	}
}
