using UnityEngine;
using System.Collections;

public class Glaive : Spell {
    Rigidbody FireBody;
    float speed = 10f;
    float timer;
    Vector3 Forward;
    Vector3 ForwardNew;
    public bool First;
    Vector3 Force;
    bool IsTrigger;
    public GameObject GlaiveObject;
    Vector3 LastVelocity;
    float PauseTimer;
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
        FireBody = GetComponent<Rigidbody>();
        Forward = transform.forward;
        Forward.y = 0;
        FireBody.AddForce(Forward.normalized * speed * 3.3f, ForceMode.VelocityChange);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Destroy(gameObject, 8f);
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other) {
        if (GetComponent<PhotonView>().isMine) {
            if (other.gameObject == PhotonManager.Me.Player) {
                First = true;
                transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            }
        }
    }
    void OnTriggerEnter(Collider other) {
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player" && other.gameObject != PhotonManager.Me.Player) {
                Vector3 FireForward = (other.transform.position - transform.position).normalized * 2.5f; //Last Forward.
                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 10f;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Glaive");
            }
            if (other.gameObject == PhotonManager.Me.Player && First) {
                GetComponent<PhotonView>().RPC("Remove", PhotonTargets.All);
            }
        }
    }
    void FixedUpdate() {
        if (!GetComponent<PhotonView>().isMine) {
            syncTime += Time.deltaTime;
            /*if (syncDelay > 0) {
                GetComponent<Rigidbody>().position = Vector3.Lerp(CurrentPosition, syncExpectedPosition, syncTime / syncDelay);
            } else {*/
            GetComponent<Rigidbody>().position = syncStartPosition;
            transform.rotation = syncEndRotation;
            return;
        }
        timer += Time.deltaTime;
        Force = (PhotonManager.Me.Player.transform.position - transform.position);
        Force.y = 0;
        if (FireBody.velocity.magnitude < 25) {
            FireBody.AddForce(((Force + Force.normalized * 3 - FireBody.velocity.normalized * (1 + timer / 4)) * 0.05f * (1 + timer / 4))*(0.3f + 30/(2f + FireBody.velocity.magnitude)), ForceMode.VelocityChange);
        } else {
            FireBody.AddForce(((Force + Force.normalized * 3 - FireBody.velocity.normalized * (1 + timer / 4)) * 0.05f * (1 + timer / 4)), ForceMode.VelocityChange);
        }
        //Debug.Log().magnitude);
        Debug.Log(FireBody.velocity);
        transform.Rotate(0, 10, 0);
    }
}