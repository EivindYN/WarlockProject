using UnityEngine;
using System.Collections;

public class TeleportEnd : MonoBehaviour {
	GameObject Player;
	GameObject Gun;
	float timer;
	SkinnedMeshRenderer renderer;
	SkinnedMeshRenderer renderer2;
	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectWithTag ("PlayerMesh");
		Gun = GameObject.FindGameObjectWithTag ("GunMesh");
		renderer = Player.GetComponent<SkinnedMeshRenderer> ();
		renderer2 = Gun.GetComponent<SkinnedMeshRenderer> ();
		renderer.enabled = false;	
		renderer2.enabled = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.deltaTime;
		if (timer > 0.4f) {
			renderer.enabled = true;	
			renderer2.enabled = true;
		}
	}
}
