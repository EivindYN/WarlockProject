using UnityEngine;
using System.Collections;

public class Meteor : MonoBehaviour {
	// Update is called once per frame
	public GameObject Explosion;
    public GameObject Shadow;
	float speed = 0.2f;
    float Time;
    float Timer;
    GameObject ShadowSpawn;
    void Start() {
        Time = transform.position.y / (speed * 1.5f);
        Vector3 EndPosition = new Vector3(transform.position.x - speed * Time, transform.position.y - speed * 1.5f * Time, transform.position.z + speed * Time);
        ShadowSpawn = Instantiate(Shadow, EndPosition, Quaternion.Euler(90, 45, 0)) as GameObject;
    }
	void FixedUpdate(){
        Timer++;
		transform.position = new Vector3(transform.position.x - speed, transform.position.y - speed*1.5f, transform.position.z + speed);
       
        if (Timer > Time / 1.5f) {
            float Scale = (Timer - Time / 1.5f);
            ShadowSpawn.transform.localScale = new Vector3(Scale/400f, Scale/400f, 1);
        }
		if (transform.position.y < 0) {
            if (GetComponent<PhotonView>().isMine) {
                PhotonNetwork.Instantiate("MeteorExplosion", new Vector3(transform.position.x, 0, transform.position.z - 0.5f), Quaternion.Euler(270, 0, 0),0);
            }
            Destroy(ShadowSpawn.gameObject);
			Destroy(gameObject);
		}
	}
}
