using UnityEngine;
using UnityEngine.EventSystems;
public class PlayerShooting : MonoBehaviour
{
	public float timeBetweenBullets = 0.15f;
	
	
	float timer;
	Ray shootRay;
	RaycastHit shootHit;
	AudioSource gunAudio;
	
	//Extra
	Ray camRay;
	Rigidbody playerRigidbody;
	int floorMask;
	bool Fire;
	float timer2;
	float timer3;
	float Delay;
	//End Extra
	
	
	public GameObject Fireball;
	public GameObject Meteorball;
	public GameObject Bouncer;
	public GameObject SelfExplosion;
	public GameObject Glaive;
	public GameObject Gravity;
	public GameObject Jump;
	public GameObject TeleportStart;
	public GameObject TeleportEnd;
	public GameObject Swap;
	public GameObject Homing;
	public GameObject TwinYin;
	public GameObject TwinYang;
	public GameObject Link;
	public GameObject Lightning;
	public GameObject Flames;
	public GameObject Drag;
	Quaternion newRotation;
	bool Shot;
	Vector3 Differanse;
	int SpellNumber;
	public static string[] Key;
	bool Shooting;
	public GameObject SpellAreaAnimation;
	public GameObject TargetAnimation;
	GameObject Animation;
	Vector3 Scale;
	public bool Quickcast;
	Vector3 playerToMouse;
	public GameObject Player;
	bool Wait;
	GameObject GlaiveObject;
	bool Target;
	Vector3 Location;
    public static float[] Cooldowns = new float[8];
    public static float[] CooldownsMax = new float[8];
    void Start ()
	{
		Key = new string [9];
		Key [1] = "q";
		Key [2] = "w";
		Key [3] = "e";
		Key [4] = "r";
		Key [5] = "a";
		Key [6] = "s";
		Key [7] = "d";
		Key [8] = "f";
		//Extra
		floorMask = LayerMask.GetMask ("Floor");
		playerRigidbody = Player.GetComponent<Rigidbody> ();
		//End Extra
		gunAudio = GetComponent<AudioSource> ();
	}
	
	void Update(){
        if (!GetComponent<PhotonView>().isMine)
            return;
        if (!EventSystem.current.IsPointerOverGameObject () && Wait == false && timer >= timeBetweenBullets && Time.timeScale != 0 && Fire == false && Player.GetComponent<PlayerHealth>().currentHealth > 0) {
			//Maybe remove over
			for (int n = 1; n <= 8; n++) {
				if (Input.GetKeyDown (Key [n])) {
                    if (Cooldowns[n - 1] != 0) {
                        return;
                    }
                    if (GameManager.Slot [n] != null) {
						SpellNumber = 0;
						Destroy (Animation);
					}
					if (GameManager.Slot [n] == null) {
						return;
					}
					SpellNumber = n;
					if (GameManager.Slot [n] == "SelfExplosionUI(Clone)"){
						Spell (n);
					} else {
                        if (GameManager.Slot[n] == "TeleportUI(Clone)" || GameManager.Slot[n] == "SwapUI(Clone)" || GameManager.Slot[n] == "LightningUI(Clone)") {
                            Delay = 0.4f;
                        } else {
                            Delay = 0.2f;
                        }
                        if (Quickcast == false) {
							camRay = Camera.main.ScreenPointToRay (Input.mousePosition);	
							RaycastHit floorHit; 
							if (Physics.Raycast (camRay, out floorHit, 100f, floorMask)) {
								Vector3 ToMouse = floorHit.point;
								ToMouse.y = 0.1f;
								if (GameManager.Slot[n] == "TeleportUI(Clone)" || GameManager.Slot[n] == "SwapUI(Clone)" ){
									Animation = Instantiate (TargetAnimation, ToMouse, Quaternion.Euler (0, 0, 0)) as GameObject;
									Target = true;
								} else {
									Animation = Instantiate (SpellAreaAnimation, ToMouse, Quaternion.Euler (90, 0, 0)) as GameObject;
									Target = false;
								}
								Scale = Animation.transform.localScale;
								ReScale (GameManager.Slot [SpellNumber]);
							}
						} else {
							Shooting = true;
						}
					}
				}				
			}
		}
		if (Input.GetKeyDown ("escape") && Fire == false) {
			SpellNumber = 0;
			Destroy (Animation);
		}
		if (SpellNumber != 0 && Fire == false && Shooting == false && Wait == false) {
			if (GameManager.Slot [SpellNumber] != null) {
				if (Target == true){
					var v3 = Input.mousePosition;
					v3.z = 10.0f;
					v3 = Camera.main.ScreenToWorldPoint(v3);
					Animation.transform.position = v3;
				} else {
					camRay = Camera.main.ScreenPointToRay (Input.mousePosition);	
					RaycastHit floorHit; 
					if (Physics.Raycast (camRay, out floorHit, 100f, floorMask)) {
						Vector3 ToMouse = floorHit.point;
						
						ToMouse.y = 0.1f;
						Animation.transform.position = ToMouse;
					}
				}
			}
			if (Input.GetButtonDown ("Fire1")) {
				Shooting = true;
				Destroy (Animation);
			}
			
		}
	}
	void FixedUpdate ()
	{
        if (!GetComponent<PhotonView>().isMine)
            return;
        timer += Time.deltaTime;
        for (int n = 0; n < 8; n++) {
            if (Cooldowns[n] != 0) {
                Cooldowns[n] -= Time.deltaTime;
                if (Cooldowns[n] < 0) {
                    Cooldowns[n] = 0;
                }
            }
        }
		if (Shooting == true) {
			Player.GetComponent<PlayerMovement>().Stop = true;
			Turning ();
			Fire = true;
			timer2 = 0;
			Shot = false;
			Shooting = false;
		}
		if (Fire == true) {
			timer2 += Time.deltaTime;
			if (timer2 > Delay && Shot == false) {
				Spell (SpellNumber);
				SpellNumber = 0;
				Shot = true;
			}
			if (timer2 > Delay + 0.2f) {
				Fire = false;
				Player.GetComponent<PlayerMovement>().Stop = false;
				timer2 = 0;
			}
		}	
		if (Wait == true) { //Self explosion
			timer3 += Time.deltaTime;
			if (timer3 > 1f) {
				Spell (SpellNumber);
			}
		}
	}
	
	// Extra
	void Turning()
	{
		camRay = Camera.main.ScreenPointToRay (Input.mousePosition);	
		RaycastHit floorHit; 
		
		if (Physics.Raycast (camRay, out floorHit, 100f, floorMask)) {
			playerToMouse = floorHit.point - Player.transform.position;
			playerToMouse.y = 0f;
			Location = floorHit.point;
			Location.y = 0;
			newRotation = Quaternion.LookRotation (playerToMouse);
			playerRigidbody.MoveRotation (newRotation);
		}
	}
	//End Extra
	void ReScale(string Spellname){
		if (Spellname == "MeteorUI(Clone)") {
			Scale = Scale * 1.8f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "FireballUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "BouncerUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "JumpUI(Clone)") {
			Scale = Scale * 1.2f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "HomingUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "TwinUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "LinkUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "LightningUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
		if (Spellname == "DragUI(Clone)") {
			Scale = Scale / 2.7f;
			Animation.transform.localScale = Scale;
		}
	}
	void Spell(int SpellNum)
	{
        string Spellname = GameManager.Slot[SpellNum];
        if (Spellname == "MeteorUI(Clone)") {
			timer = 0f;
			gunAudio.Play ();
			Vector3 MeteorSpawnpoint = new Vector3 (Location.x + 25, Location.y + 37.5f, Location.z - 25);
			PhotonNetwork.Instantiate ("Meteor", MeteorSpawnpoint, Quaternion.Euler (30, 260, 0),0);             
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 20;
        }
		if (Spellname == "FireballUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			
			GameObject Fire = PhotonNetwork.Instantiate ("Fireball", transform.position, newRotation, 0) as GameObject;
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 3.5f;
        }
		if (Spellname == "HomingUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			
			GameObject Fire = PhotonNetwork.Instantiate ("Homing", transform.position, newRotation, 0) as GameObject;
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 14;
        }
		if (Spellname == "BouncerUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			
			GameObject BouncerObject = Instantiate (Bouncer, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Quaternion.identity) as GameObject;
            BouncerObject.GetComponent<Bouncer>().AimVector = playerToMouse;
        }
		if (Spellname == "SelfExplosionUI(Clone)") {
			timer = 0f;
			
			
			if (timer3 == 0){                
                Wait = true;
				Player.GetComponent<PlayerMovement>().Stop = true;
			} else {
                CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 6; //Unsure
                timer3 = 0;
				SpellNumber = 0;
				Wait = false;
				PhotonNetwork.Instantiate ("SelfExplosion", new Vector3 (Player.transform.position.x, 0.3f, Player.transform.position.z), Quaternion.identity,0);
				Player.GetComponent<PlayerMovement>().Stop = false;
			}
		}
		if (Spellname == "GlaiveUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
            GameObject Fire = PhotonNetwork.Instantiate("Glaive", transform.position, newRotation, 0) as GameObject;
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 16;
        }
		if (Spellname == "GravityUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			
			PhotonNetwork.Instantiate ("GravityBlack", transform.position, newRotation,0);
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 26;
        }
		if (Spellname == "SwapUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();

            GameObject Fire = PhotonNetwork.Instantiate("Swap", transform.position, newRotation, 0) as GameObject;
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 14;
        }
		if (Spellname == "TeleportUI(Clone)") {
			timer = 0f;
			Differanse = Location - Player.transform.position;
			if (Differanse.magnitude > 10){
				Differanse = Differanse.normalized * 10;
				Location = Player.transform.position + Differanse;
			}
            PhotonNetwork.Instantiate("TeleportStart", Player.transform.position, transform.rotation,0);
			Player.transform.position = Location;
			Location.y += 0.8f;
			Location.z -= 0.5f;
			PhotonNetwork.Instantiate ("TeleportEnd", Location, transform.rotation,0);
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 16;
        }
		if (Spellname == "JumpUI(Clone)") {
			timer = 0f;
			Differanse = Location - Player.transform.position;
			if (Differanse.magnitude > 10){
				Differanse = Differanse.normalized * 10;
				Location = Player.transform.position + Differanse;
			}
			Location.y += 0.1f;
			PhotonNetwork.Instantiate ("JumpEnd", Location, Quaternion.identity,0 );
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 17;
        }
		if (Spellname == "TwinUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();

            PhotonNetwork.Instantiate("Yang", new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), newRotation, 0);
            PhotonNetwork.Instantiate("Yin", new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), newRotation, 0);
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 15;
        }
		if (Spellname == "LinkUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			Link.GetComponent<Link>().Sender = Player.transform;
			GameObject LinkObject = Instantiate (Link, transform.position, Quaternion.identity) as GameObject;
			Rigidbody LinkBody = LinkObject.GetComponent<Rigidbody> ();
			LinkBody.MoveRotation (newRotation);
		}
		if (Spellname == "LightningUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			Lightning.GetComponent<Lightning>().Sender = Player.transform;
			Differanse = Location - Player.transform.position;
			if (Differanse.magnitude > 8){
				Differanse = Differanse.normalized * 8;
				Location = Player.transform.position + Differanse;
			}
			PhotonNetwork.Instantiate ("Lightning", new Vector3 (Location.x, 0.2f, Location.z), Quaternion.identity,0);
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 16.5f;
        }
		if (Spellname == "FlamesUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();

            GameObject Fire = PhotonNetwork.Instantiate("Flames", transform.position, newRotation, 0) as GameObject;
            CooldownsMax[SpellNum - 1] = Cooldowns[SpellNum - 1] = 15;
        }
		if (Spellname == "DragUI(Clone)") {
			timer = 0f;
			
			gunAudio.Play ();
			Drag.GetComponent<Drag>().Sender = Player.transform;
			GameObject DragObject = Instantiate (Drag, transform.position, Quaternion.identity) as GameObject;
			Rigidbody DragBody = DragObject.GetComponent<Rigidbody> ();
			DragBody.MoveRotation (newRotation);
		}
		/*
        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
        */
	}
}
