using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Spell : MonoBehaviour {
    public GameObject Explosion;
    public GameObject BloodSplat;
    [PunRPC]
    /// <summary>
    /// The spell has hit a Player. 
    /// </summary>
    /// <param name = "Info"> 
    /// PlayerID, Amountdamage, Length force vector, Degree force vector 
    /// </param>
    public void Explode(float[] Info) {
        ExplodeExtra(Info, "");
    }
    [PunRPC]
    public void ExplodeExtra(float[] Info, string ExtraInfo) {
        int PlayerID = (int)Info[0];
        float AmountDamage = Info[1];
        float Magnitude = Info[2];
        float Angle = Info[3];
        GameObject Player = PhotonManager.Me.Players[PlayerID];
        PlayerHealth PH = Player.GetComponent<PlayerHealth>();
        PH.FireForward = new Vector3(Mathf.Cos(Angle * Mathf.PI / 180f), 0, Mathf.Sin(Angle * Mathf.PI / 180f)) * Magnitude;
        if (ExtraInfo == "Lava") {
            PH.TakeDamage(AmountDamage, false, false);
        } else {
            PH.TakeDamage(AmountDamage);
            ShowDamage(Player, AmountDamage);
        }
        if (Explosion) {
            Instantiate(Explosion, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.Euler(270, 0, 0));
        }
        if (ExtraInfo == "Glaive") {
            AudioSource Audio = GameObject.Find("GameManager").GetComponents<AudioSource>()[0];
            Destroy(Instantiate(BloodSplat, PhotonManager.Me.Players[PlayerID].transform.position, Quaternion.identity), 0.20f);
            Audio.Play();
        } else if (ExtraInfo != "Stay" && ExtraInfo != "Lava") {
            Destroy(gameObject);
        }
    }
    public static void ShowDamage(GameObject Object, float num) {
        GameObject Number = Instantiate(Resources.Load<GameObject>("Prefabs/DamageUI")) as GameObject;
        RectTransform RectTrans = Number.GetComponent<RectTransform>();
        Number.transform.SetParent(GameObject.Find("HUDCanvas").transform);
        RectTrans.rect.Set(0, 0, 0, 0);
        RectTrans.anchoredPosition = Vector2.zero;
        RectTrans.anchorMin = Camera.main.WorldToViewportPoint(Object.transform.position);
        RectTrans.anchorMax = Camera.main.WorldToViewportPoint(Object.transform.position);
        int Extra = (int)Mathf.Sqrt(num * 2);
        Number.GetComponent<Text>().text = num + "";
        Number.GetComponent<Text>().fontSize = 20 + Extra;
        GameManager.DamageUIs.Add(Number);
        if (GameManager.DamageUIs.Count > 50) {
            //maybe not
            GameObject ToBeDestroyed = GameManager.DamageUIs[0];
            GameManager.DamageUIs.Remove(GameManager.DamageUIs[0]);
            Destroy(ToBeDestroyed);
        }
    }

    [PunRPC]
    public void Remove() {
        Destroy(gameObject);
    }
    [PunRPC]
    public void PlayAudio(int AudioNum) {
        AudioSource Audio = GameObject.Find("GameManager").GetComponents<AudioSource>()[AudioNum];
        Audio.Play();
    }
}
