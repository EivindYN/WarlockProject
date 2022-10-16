using UnityEngine;
using System.Collections;

public class SelfExplosion : Spell {
	void OnTriggerEnter (Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player" && other.gameObject != PhotonManager.Me.Player) {
                Debug.Log("Happens");
                Vector3 ObjectPositions = other.transform.position - transform.position;
                ObjectPositions.y = 0f;
                Vector3 FireForward = ObjectPositions.normalized * 3f;

                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 10;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Stay");
            }
        }
	}
}