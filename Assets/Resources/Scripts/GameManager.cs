using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {
    public static string[] Slot;
    public int[] SizeRow;
    public bool SmallerOverTime = true;
    public static List<GameObject> DamageUIs;

    public GameObject Stage;
    GameObject[] CooldownSlots = new GameObject[8];
    GameObject[] SpellSlots = new GameObject[8];
    Vector3[] StartPositions = new Vector3[8];
    bool[] CDMovement = new bool[8];
    // Use this for initialization
    void Start() {
        DamageUIs = new List<GameObject>();
        Slot = new string[9];
        GameObject CoolDowns = GameObject.Find("Cooldowns");
        CooldownSlots[0] = CoolDowns.transform.GetChild(0).GetChild(0).gameObject;
        CooldownSlots[1] = CoolDowns.transform.GetChild(0).GetChild(1).gameObject;
        CooldownSlots[2] = CoolDowns.transform.GetChild(0).GetChild(2).gameObject;
        CooldownSlots[3] = CoolDowns.transform.GetChild(0).GetChild(3).gameObject;
        CooldownSlots[4] = CoolDowns.transform.GetChild(1).GetChild(0).gameObject;
        CooldownSlots[5] = CoolDowns.transform.GetChild(1).GetChild(1).gameObject;
        CooldownSlots[6] = CoolDowns.transform.GetChild(1).GetChild(2).gameObject;
        CooldownSlots[7] = CoolDowns.transform.GetChild(1).GetChild(3).gameObject;

        GameObject Spells = GameObject.Find("SpellUI and ShopTest");
        SpellSlots[0] = Spells.transform.GetChild(0).GetChild(0).gameObject;
        SpellSlots[1] = Spells.transform.GetChild(0).GetChild(1).gameObject;
        SpellSlots[2] = Spells.transform.GetChild(0).GetChild(2).gameObject;
        SpellSlots[3] = Spells.transform.GetChild(0).GetChild(3).gameObject;
        SpellSlots[4] = Spells.transform.GetChild(1).GetChild(0).gameObject;
        SpellSlots[5] = Spells.transform.GetChild(1).GetChild(1).gameObject;
        SpellSlots[6] = Spells.transform.GetChild(1).GetChild(2).gameObject;
        SpellSlots[7] = Spells.transform.GetChild(1).GetChild(3).gameObject;

    }

    // Update is called once per frame
    void Update() {
        for (int n = 0; n < 8; n++) {
            if (PlayerShooting.CooldownsMax[n] == 0) {
                CooldownSlots[n].GetComponent<Image>().fillAmount = 0;
            } else {
                CooldownSlots[n].GetComponent<Image>().fillAmount = PlayerShooting.Cooldowns[n] / PlayerShooting.CooldownsMax[n];
            }

            //Clunky, should be a better solution available Grunn: Positionen er feil ved Start() pågrunn av Layout Group Flyttingen
            if (SpellUI.Item) {
                if (n == (Convert.ToInt32(SpellUI.StartParent.name.Replace("Slot", "")) - 1)) {
                    if (!CDMovement[n]) {
                        CDMovement[n] = true;
                        StartPositions[n] = CooldownSlots[n].transform.position;
                    }
                    CooldownSlots[n].transform.position = SpellUI.Item.transform.position;
                }
            } else {
                if (CDMovement[n]) {
                    CDMovement[n] = false;
                    CooldownSlots[n].transform.position = StartPositions[n];
                }
            }


            //PlayerShooting.Cooldowns
        }
    }
    public void SwapCooldowns(int Cooldown1, int Cooldown2) {
        float CD1 = PlayerShooting.Cooldowns[Cooldown1];
        float CD1Max = PlayerShooting.CooldownsMax[Cooldown1];
        float CD2 = PlayerShooting.Cooldowns[Cooldown2];
        float CD2Max = PlayerShooting.CooldownsMax[Cooldown2];
        PlayerShooting.Cooldowns[Cooldown1] = CD2;
        PlayerShooting.CooldownsMax[Cooldown1] = CD2Max;
        PlayerShooting.Cooldowns[Cooldown2] = CD1;
        PlayerShooting.CooldownsMax[Cooldown2] = CD1Max;
    }
    void FixedUpdate() {

        foreach (GameObject UI in DamageUIs) {
            UI.transform.position = new Vector2(UI.transform.position.x, UI.transform.position.y + 0.5f);
            Color TextColor = UI.GetComponent<Text>().color;
            UI.GetComponent<Text>().color = new Color(TextColor.r, TextColor.g, TextColor.b, TextColor.a - 0.025f);
            if (TextColor.a <= 0) {
                GameObject ToBeDestroyed = UI;
                DamageUIs.Remove(UI);
                Destroy(ToBeDestroyed);
                break;
            }
        }

        if (SmallerOverTime)
            Stage.transform.localScale = Stage.transform.localScale / 1.0002f;
    }
    public static float GetAngle(Vector2 Direction) {
        float Angle;
        if (Vector3.Angle(Vector3.left, Direction) > 90) {
            Angle = Vector3.Angle(Vector3.up, Direction);
        } else {
            Angle = 360 - Vector3.Angle(Vector3.up, Direction);
        }
        //Its like a clock (↑) is 0 and (→) is 90
        Angle = 90 - Angle;
        //Now instead its  (→) is 0 and (↑) is 90
        return Angle;
    }

}
