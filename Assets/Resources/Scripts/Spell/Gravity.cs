using UnityEngine;
using System.Collections;

public class Gravity : Spell {
    Rigidbody GravityBody;
    float speed = 5f;
    float Damagetimer;
    float duration;
    float Forcetimer;
    int PlayerSenderID = -1;
    // Use this for initialization
    void Start() {
        GravityBody = GetComponent<Rigidbody>();
        GravityBody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        transform.rotation = Quaternion.identity;
        if (GetComponent<PhotonView>().isMine) {
            GetComponent<PhotonView>().RPC("SetSender", PhotonTargets.All, PhotonManager.Me.PlayerID);
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other) {
        //if (GetComponent<PhotonView>().isMine) {
            if (other.name != "Floor") {
                GameObject Object = other.gameObject;
                if (Object.GetComponent<Rigidbody>() != null) {
                    Rigidbody Body = Object.GetComponent<Rigidbody>();
                    Vector3 Force = (transform.position - Object.transform.position);
                    Force.y = 0;
                    if (Damagetimer > 0.5f && Object.tag == "Player" && PlayerSenderID != -1) {
                        if (Object.gameObject != PhotonManager.Me.Players[PlayerSenderID]) {
                            if (GetComponent<PhotonView>().isMine) {
                                Damagetimer = 0;

                                float[] Test = new float[4];
                                Test[0] = Object.GetComponent<PhotonInfo>().PlayerID;
                                Test[1] = 0.5f;
                                if ((transform.position - Object.transform.position).magnitude < 1) {
                                    Test[1] = 1f;
                                }
                                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Stay");
                            }
                        }
                    }
                    if (Object.tag == "Player" && PlayerSenderID != -1) {
                        if (Object.gameObject != PhotonManager.Me.Players[PlayerSenderID]) {
                            Damagetimer += Time.deltaTime;
                            Forcetimer += Time.deltaTime;
                            float ForceAmount = 3f;
                            if (Forcetimer > 0.3f) {
                                if ((transform.position - Object.transform.position).magnitude < 1) {
                                    ForceAmount = 4f;
                                }
                            Body.AddForce((Force).normalized * ForceAmount, ForceMode.VelocityChange);
                            }
                        }
                    } else {
                        if (Body.velocity != new Vector3(0, 0, 0)) {
                            if (Object.name == "Bouncer(Clone)") {
                                Body.AddForce((Force).normalized * 0.3f, ForceMode.VelocityChange);
                                return;
                            }
                            if (Object.name == "GravityBlack(Clone)") {
                                Body.AddForce((Force).normalized * 0.05f, ForceMode.VelocityChange);
                                return;
                            }
                            if (Object.name == "Glaive(Clone)") {
                                Body.AddForce((Force).normalized * 1.2f, ForceMode.VelocityChange);
                                return;
                            }
                            Body.AddForce((Force).normalized * 0.6f, ForceMode.VelocityChange);
                        }
                    }
                }
            //}
        }
    }
    void FixedUpdate() {
        duration += Time.deltaTime;
        if (duration > 5f) {
            Destroy(gameObject);
        }
    }
    [PunRPC]
    void SetSender(int PlayerID) {
        PlayerSenderID = PlayerID;
    }
}
