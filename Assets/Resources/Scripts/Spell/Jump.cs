using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Jump : Spell {
	float Duration;
	float DurationTimer;
	GameObject Player;
	Vector3 PlayerToThis;
	Vector3 PlayerLocation;
	ParticleSystem Particles;
	bool Switch;
	private ParticleSystem ps;

	public GameObject JumpObject;
	float Counter;
    bool First;
    List<int> Playershit = new List<int>();
	// Use this for initialization
	void Start () {
        Particles = GetComponent<ParticleSystem>();
        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        Player = GameObject.Find("GameManager").GetComponent<PhotonManager>().Player;
        PlayerToThis = transform.position - Player.transform.position;
        Duration = PlayerToThis.magnitude / 25;
		Player.GetComponent<PlayerMovement>().Stop = true;
		PlayerLocation = Player.transform.position;
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!GetComponent<PhotonView>().isMine) {
            return;
        }
        if (Switch == false) {
			DurationTimer += Time.deltaTime;
			if ((Player.transform.position - PlayerLocation).magnitude > Counter){
				PhotonNetwork.Instantiate ("Jump",Player.transform.position,Player.transform.rotation,0);
				Counter += 2;
			}	
			//PlayerToThis = transform.position - Player.transform.position;
			//Player.transform.position = Player.transform.position + PlayerToThis * DurationTimer;
			Player.transform.position = PlayerLocation + PlayerToThis * Mathf.Pow (DurationTimer, 2) / Mathf.Pow (Duration, 2);
			if (DurationTimer > Duration) {
                GetComponent<PhotonView>().RPC("StartParticles", PhotonTargets.All);
                PhotonManager.Me.Player.GetComponent<PlayerMovement>().targetPosition = transform.position;
                Player.GetComponent<PlayerMovement>().Stop = false;
				Switch = true;
				GetComponent<CapsuleCollider>().enabled = true;
			}
		} else {
			if(ps)
			{
                if (!First) {
                    First = true;
                    GetComponent<PhotonView>().RPC("PlayAudio", PhotonTargets.All,1);
                }
				if(!ps.IsAlive())
				{
                    GetComponent<PhotonView>().RPC("Remove", PhotonTargets.All);
                }
			}
		}
	}
	void OnTriggerStay(Collider other){
        if (GetComponent<PhotonView>().isMine) {
            if (other.tag == "Player" && other.gameObject != PhotonManager.Me.Player) {
                if (Playershit.Contains(other.GetComponent<PhotonInfo>().PlayerID)) {
                    return;
                }
                Playershit.Add(other.GetComponent<PhotonInfo>().PlayerID);
                Vector3 ObjectPositions = other.transform.position - transform.position;
                ObjectPositions.y = 0f;
                Vector3 FireForward = ObjectPositions.normalized * 2.5f;

                float[] Test = new float[4];
                Test[0] = other.GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 8;
                Test[2] = FireForward.magnitude;
                Test[3] = GameManager.GetAngle(new Vector2(FireForward.x, FireForward.z));
                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Stay");
            }
        }
	}
    [PunRPC]
    void StartParticles() {
        Particles.Play();
    }
}
