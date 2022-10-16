using UnityEngine;
using System.Collections;

public class Twin : Spell {
	Rigidbody FireBody;
	Vector3 Forward;
	float Duration = 100f;
	float Timer;
	float Angle = 80f;
	public float Number;
	bool Turned;
	bool First;
	bool GoFirst;
	float DestroyTimer;
    // Use this for initialization

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
            if (FireBody) {            
                velocity = FireBody.velocity;
            }
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

            velocity = syncVelocity;
        }
    }

    void Start() {
        FireBody = GetComponent<Rigidbody>();
        Destroy(gameObject, 8);
        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        FireBody.velocity = transform.forward * 6.5f;
        Forward = transform.forward;
        transform.rotation = Quaternion.identity;
        if (gameObject.name.Contains("Yang")) {
            Angle = -Angle;
        }
        FireBody.velocity = Quaternion.AngleAxis(-Angle, Vector3.up) * FireBody.velocity;
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
                    FireForward = (ObjectPositions.normalized + (transform.forward * 6.5f).normalized * 2).normalized * 3f;
                }

                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 8f;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("Explode", PhotonTargets.All, Test);
            }
        }
	}
	// Update is called once per frames
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
        //(x-1)pow2 + (y)pow2 = 1pow2
        //y=sqrt(1pow2-(x-1)pow2)	
        if (Timer < Duration / 2) {
			FireBody.velocity = Quaternion.AngleAxis (Angle*2/(Duration/2), Vector3.up) * FireBody.velocity;
		} else {
			FireBody.velocity = Quaternion.AngleAxis (-Angle*2/(Duration/2), Vector3.up) * FireBody.velocity;
		}
		Timer += 1;
		if (Timer == Duration) {
			Timer = 0;
		}

	}
	void LateUpdate () {
		
		ParticleSystem.Particle[] p = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount+1];
		int l = GetComponent<ParticleSystem>().GetParticles(p);
		int i = 0;
		while (i < l) {
            if (!GetComponent<PhotonView>().isMine) {
                p[i].velocity = -(velocity.normalized) * 10;
            } else {
                p[i].velocity = -(FireBody.velocity.normalized) * 10;
            }
			i++;
		}
		
		GetComponent<ParticleSystem>().SetParticles(p, l);  
		
	}
}
