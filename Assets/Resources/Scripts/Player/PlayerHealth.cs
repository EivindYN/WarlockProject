using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : Spell
{
    public float startingHealth = 100;
    public float currentHealth;
    Slider healthSlider;
    Image damageImage;	
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    Animator anim;
    AudioSource playerAudio;
    bool isDead;
    bool damaged;


	public Vector3 FireForward;
	float health;


    public GameObject PlayerHealthText;
    PlayerMovement Movement;

    public bool TestObject;

    public float HPRegen = 5;
    float RegenTimer;
    PhotonView PV;
    Rigidbody Body;
    public GameObject HealthBarPrefab;
    GameObject HealthBar;
    void Start ()
    {
        healthSlider = GameObject.FindGameObjectWithTag("HealthSlider").GetComponent<Slider>();
        PlayerHealthText = GameObject.FindGameObjectWithTag("HealthText");
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        currentHealth = startingHealth;
        Body = GetComponent<Rigidbody>();
        Movement = GetComponent<PlayerMovement>();        
        PV = GetComponent<PhotonView>();
        if (!GetComponent<PhotonView>().isMine) {
            HealthBar = Instantiate (HealthBarPrefab, new Vector3(-10000, -10000, -10000), Quaternion.identity) as GameObject;
            HealthBar.GetComponent<UIFollow>().Target = gameObject.transform;
            HealthBar.transform.SetParent(GameObject.Find("HUDCanvas").transform, false);
        }
    }


    void FixedUpdate ()
    {

        if (!TestObject) {
            if (Movement.OnLava) {
                //TakeDamage(0.15f, false, false);
                float[] Test = new float[4];
                Test[0] = GetComponent<PhotonInfo>().PlayerID;
                Test[1] = 0.15f;
                GetComponent<PhotonView>().RPC("ExplodeExtra", PhotonTargets.All, Test, "Lava");
            }
        }
        if (!GetComponent<PhotonView>().isMine)
            return;
        RegenTimer += Time.deltaTime;
        if (RegenTimer == HPRegen) {
            currentHealth++;
            if (currentHealth < startingHealth) {
                currentHealth = startingHealth;
            }
            RegenTimer = 0;
        }
        if(damaged)
        {
            //damageImage.color = flashColour;
        }
        else
        {
            //damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }
    [PunRPC]
    void Push(Vector3 Amount) {
        //Body.AddForce(Amount, ForceMode.VelocityChange);
    }
    void Update() {
        float Num = currentHealth;
        if (Num < 0) {
            Num = 0;
        }
        if (PlayerHealthText != null) {
            if (GetComponent<PhotonView>().isMine) {
                PlayerHealthText.GetComponent<Text>().text = Mathf.Round(Num) + "/" + Mathf.Round(startingHealth);
            }
        }
        if (!GetComponent<PhotonView>().isMine) {
            if (currentHealth < 0) {
                HealthBar.transform.GetChild(1).localScale = Vector3.zero;
            } else {
                HealthBar.GetComponent<Slider>().value = currentHealth;
            }
        
        }
        if (GetComponent<PhotonView>().isMine) {
            healthSlider.value = currentHealth;
        }
    }
    public void TakeDamage (float amount){
        if (amount < 1) {
            TakeDamage(amount, true);
        } else {
            TakeDamage(amount, false);
        }
	}
    public void TakeDamage (float amount, bool sound)
    {
		TakeDamage (amount, sound, true);
	}
    public void TakeDamage (float amount, bool sound, bool push)
	{
        damaged = true;

        currentHealth -= amount;

        if (GetComponent<PhotonView>().isMine) {
            healthSlider.value = currentHealth;
        }
		

        if(currentHealth <= 0 && !isDead)
        {
            if (GetComponent<PhotonView>().isMine) {
                GameObject.Find("Fill").transform.localScale = new Vector3(0, 0, 0);
            }
            Death ();
            Movement.Stop = true;
        }
        if (push) {
			if (sound == true){
				//playerAudio.Play ();
			}

            if (amount >= 1) {
                Movement.PushSpeed = 5f;
            } else {
                Movement.PushSpeed = 2.25f;
            }
            /*
            if (push == true) {
                speed = 2.25f;
            } else {
                speed = 5;
            }*/
            Movement.Pushed = true;
            Movement.PushTimer = 0;
            Movement.targetPosition = transform.position;
        }
    }


    void Death ()
    {
        isDead = true;

        anim.SetTrigger ("Die");
        GetComponent<CapsuleCollider>().enabled = false;

        //playerAudio.clip = deathClip;
        //playerAudio.Play ();
    }
}
