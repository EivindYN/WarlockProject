
using UnityEngine;
using System.Collections;

public class Flameball : Spell {
	Rigidbody FireBody;
	float speed = 12.5f;
	float timer;
	Vector3 Forward; 
	bool First;
    // Use this for initializa;tion

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

    void Start () {
        FireBody = GetComponent<Rigidbody>();
        FireBody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        Forward = transform.forward;
        transform.rotation = Quaternion.identity;
        First = true;
        Destroy(gameObject, 1.5f);
    }
	
	// Update is called once per frame
	void OnTriggerEnter (Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player") {
                Vector3 ObjectPositions = other.transform.position - transform.position;
                ObjectPositions.y = 0f;
                Vector3 FireForward = Vector3.zero;
                if (FireBody) {
                    FireForward = (ObjectPositions.normalized + (FireBody.velocity).normalized * 2).normalized * 3f;
                } else {
                    FireForward = (ObjectPositions.normalized + (transform.forward * speed).normalized * 2).normalized * 3f;
                }

                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 0.2f;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("Explode", PhotonTargets.All, Test);
            }
        }
	}
	void LateUpdate () {
        if (!GetComponent<PhotonView>().isMine) {
            syncTime += Time.deltaTime;
            /*if (syncDelay > 0) {
                GetComponent<Rigidbody>().position = Vector3.Lerp(CurrentPosition, syncExpectedPosition, syncTime / syncDelay);
            } else {*/
            GetComponent<Rigidbody>().position = syncStartPosition;
            transform.rotation = syncEndRotation;
        }
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount+1];
		int l = GetComponent<ParticleSystem>().GetParticles(p);
		int i = 0;
		while (i < l) {
            if (!GetComponent<PhotonView>().isMine) {
                p[i].velocity = -velocity.normalized * 10;
            } else {
                p[i].velocity = -FireBody.velocity.normalized * 10;
            }
			i++;
		}
		
		GetComponent<ParticleSystem>().SetParticles(p, l);  
		
	}
}
/*/
using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour {
	float speed = 4.0f;
	
	// Update is called once per frame
	void Update(){
		transform.position += transform.forward * speed * Time.deltaTime;
	}
}
*/