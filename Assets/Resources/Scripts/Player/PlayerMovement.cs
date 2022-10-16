using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PlayerMovement : MonoBehaviour {

    public Vector3 targetPosition;

    //Turning
    int floorMask;
    Rigidbody playerRigidbody;
    //EndTurning
    Vector3 movement;
    Ray camRay;
    Animator anim;
    public GameObject Move;
    public bool Stop;
    bool Moving;
    public bool OnLava;
    public bool Dead;

    public bool Pushed;
    public float PushTimer;
    public float PushSpeed;
    Vector3 PushEffecter;

    void Start() {
        anim = GetComponent<Animator>();
        //Turning
        floorMask = LayerMask.GetMask("Floor");
        playerRigidbody = GetComponent<Rigidbody>();
        //EndTurning
        targetPosition = transform.position;
    }
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    public Vector3 CurrentPosition = Vector3.zero;
    public Vector3 syncStartPosition = Vector3.zero;
    public Vector3 syncExpectedPosition = Vector3.zero;
    private Quaternion syncStartRotation;
    private Quaternion syncEndRotation;
    private bool syncAnim = false;
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(GetComponent<Rigidbody>().position);
            stream.SendNext(velocity);
            //stream.SendNext(PushEffecter);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Animator>().GetBool("IsWalking"));

        } else {

            Vector3 syncPosition = (Vector3)stream.ReceiveNext();
            Vector3 syncVelocity = (Vector3)stream.ReceiveNext();
            //Vector3 RetrievedPushEffecter = (Vector3)stream.ReceiveNext();
            Quaternion syncRotation = (Quaternion)stream.ReceiveNext();
            syncAnim = (bool)stream.ReceiveNext();


            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncExpectedPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = syncPosition;

            CurrentPosition = GetComponent<Rigidbody>().position;
            

            syncEndRotation = syncRotation;
            syncStartRotation = transform.rotation;

            //PushEffecter = RetrievedPushEffecter;

        }
    }
    Vector3 previous;
    Vector3 velocity;
    void Update() {

        if (!GetComponent<PhotonView>().isMine) {
                syncTime += Time.deltaTime;
                if (syncDelay > 0) {
                    GetComponent<Rigidbody>().position = Vector3.Lerp(CurrentPosition, syncExpectedPosition, syncTime / syncDelay);
                } else {
                    GetComponent<Rigidbody>().position = syncStartPosition;
                }
                //GetComponent<Rigidbody>().position = syncStartPosition;
                transform.rotation = syncEndRotation;
                anim.SetBool("IsWalking", syncAnim);
                /* OLD
                if (syncDelay > 0) {
                    syncTime += Time.deltaTime;
                    GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
                    //GetComponent<Rigidbody>().position = syncStartPosition;
                    transform.rotation = syncEndRotation;
                    anim.SetBool("IsWalking", syncAnim);
                }*/
            return;
        }
        gameObject.tag = "Player";
        if (Input.GetKeyDown("space")) {
            if (Time.timeScale == 0) {
                Time.timeScale = 1;
            } else {
                Time.timeScale = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            Vector3 FireballPos = transform.position;
            FireballPos.z -= 5;
            GameObject Fire = PhotonNetwork.Instantiate("Fireball", FireballPos, Quaternion.Euler(0,0,0), 0) as GameObject;
        }
        if (!EventSystem.current.IsPointerOverGameObject() && GetComponent<PlayerHealth>().currentHealth > 0) {
            if (Input.GetMouseButtonDown(1)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Pow(8, 2))) {
                    targetPosition = hit.point;
                    targetPosition.y = 0;
                    Moving = true;
                    camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Instantiate(Move, (targetPosition), Quaternion.Euler(270, 0, 0));
                }
            }
        }

    }
    void FixedUpdate() {/*
        if (Mathf.Abs(transform.position.y) > 0.05f) {
            Vector3 FixedPos = transform.position;
            FixedPos.y = 0;
            transform.position = FixedPos;
        }*/

        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        velocity = ((transform.position - previous)) / Time.deltaTime;
        previous = transform.position;
        if ((targetPosition.x - transform.position.x > 0.05f || targetPosition.x - transform.position.x < -0.05f || targetPosition.z - transform.position.z > 0.05f || targetPosition.z - transform.position.z < -0.05f) && Moving == true) {

            movement.Set(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z);

            movement = movement.normalized * 5 * Time.deltaTime;
            if (PushSpeed != 0 && Pushed == true || Stop) {
                if (Stop) {
                    targetPosition = transform.position;
                    anim.SetBool("IsWalking", false);
                    return;
                }
            }
            Turning();
            anim.SetBool("IsWalking", true);
            if (PushSpeed != 0 && Pushed == true) {
                PushEffecter = movement.normalized;
            } else {
                Vector3 Route = transform.position + movement;
                Route.y = 0;
                playerRigidbody.MovePosition(Route);
            }


        } else {
            anim.SetBool("IsWalking", false);
            Moving = false;
        }
        if (Pushed == true) {
            PushTimer += Time.deltaTime;

            //PV.RPC("Push", PhotonTargets.All, FireForward * speed * (10f - Mathf.Pow(timer * 20000, 0.25f)) / 4);
            if (PushTimer > 0.5f) {
                Pushed = false;
                PushSpeed = 0;
                //Body.velocity = Vector3.zero;
            } else {
                playerRigidbody.AddForce((GetComponent<PlayerHealth>().FireForward + PushEffecter * 1.5f) * PushSpeed * (10f - Mathf.Pow(PushTimer * 20000, 0.25f)) / 4, ForceMode.VelocityChange);
            }
        }
    }

    void Turning() {
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, 100f, floorMask)) {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.tag == "Stage") {
            OnLava = true;
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Stage") {
            OnLava = false;
        }
    }
    void OnTriggerStay(Collider other) {
        if (other.tag == "Stage" && OnLava == true) {
            OnLava = false;
        }
    }
}