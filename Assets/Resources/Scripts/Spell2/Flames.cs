using UnityEngine;
using System.Collections;

public class Flames : MonoBehaviour {
	public GameObject Flame1;
	public GameObject Flame2;
	float TotalTimer;
	float Timer;
	float RNG;
	float Angle = 10;
	// Update is called once per frame
	void FixedUpdate () {

		TotalTimer += Time.deltaTime;
		Timer += Time.deltaTime;
		if (Timer > 0.1f) {
            if (GetComponent<PhotonView>().isMine) {
                RNG = Random.Range(-Angle, Angle);
                Quaternion Rotation1 = Quaternion.AngleAxis(RNG, Vector3.up) * transform.rotation;
                PhotonNetwork.Instantiate("Flames1", transform.position, Rotation1, 0);
                RNG = Random.Range(-Angle, Angle);
                Quaternion Rotation2 = Quaternion.AngleAxis(RNG, Vector3.up) * transform.rotation;
                PhotonNetwork.Instantiate("Flames1", transform.position, Rotation2, 0);
                RNG = Random.Range(-Angle, Angle);
                Quaternion Rotation3 = Quaternion.AngleAxis(RNG, Vector3.up) * transform.rotation;
                PhotonNetwork.Instantiate("Flames2", transform.position, Rotation3, 0);
                RNG = Random.Range(-Angle, Angle);
                Quaternion Rotation4 = Quaternion.AngleAxis(RNG, Vector3.up) * transform.rotation;
                PhotonNetwork.Instantiate("Flames2", transform.position, Rotation4, 0);
            }
			Timer = 0;
		}
		if (TotalTimer > 0.5f) {
			Destroy(gameObject);
		}
	}
}
