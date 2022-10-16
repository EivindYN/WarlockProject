using UnityEngine;
using System.Collections;

public class Swap : MonoBehaviour {
	Rigidbody FireBody;
	float speed = 30f;
	float timer;
    bool Swapped;

    // Use this for initializa;tion
    void Start() {
        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        FireBody = GetComponent<Rigidbody>();
        FireBody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
        transform.rotation = Quaternion.identity;
        
    }
	// Update is called once per frame
	void OnTriggerEnter (Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player" && !Swapped) {
                Swapped = true;
                int EnemyID = other.GetComponent<PhotonInfo>().PlayerID;
                int MyID = PhotonManager.Me.PlayerID;

                Vector3 EnemyPos = PhotonManager.Me.Players[EnemyID].transform.position;
                Vector3 MyPos = PhotonManager.Me.Players[MyID].transform.position;
                GetComponent<PhotonView>().RPC("SetPositionPlayers", PhotonTargets.All, MyID, EnemyPos, EnemyID, MyPos);
            }
        }
	}
	void FixedUpdate(){
        timer += Time.deltaTime;
		if (timer > 0.5f) {
            if (GetComponent<PhotonView>().isMine) {
                Vector3 SetPos = transform.position;
                SetPos.y = 0;
                PhotonManager.Me.Player.transform.position = SetPos;
                PhotonManager.Me.Player.GetComponent<PlayerMovement>().targetPosition = SetPos;
            }
			Destroy(gameObject);
		}
	}
    [PunRPC]
    void SetPositionPlayers(int ID1, Vector3 ID1Pos, int ID2, Vector3 ID2Pos) {
        PhotonManager.Me.Players[ID1].transform.position = ID1Pos;
        PlayerMovement PM1 = PhotonManager.Me.Players[ID1].GetComponent<PlayerMovement>();
        PM1.syncStartPosition = PM1.syncExpectedPosition = PM1.CurrentPosition = ID1Pos;
        PhotonManager.Me.Players[ID2].transform.position = ID2Pos;
        PlayerMovement PM2 = PhotonManager.Me.Players[ID2].GetComponent<PlayerMovement>();
        PM2.syncStartPosition = PM2.syncExpectedPosition = PM2.CurrentPosition = ID2Pos;
        Destroy(gameObject);
    }
}
