using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopTest : MonoBehaviour {
	GameObject UI;
	bool Found;
	float Number;
	float PageAmount = 11; /*10*/
	float PageDistance = 655;
    List<SpellIcon> SpellIcons = new List<SpellIcon>();
	// Use this for initialization
	void Start(){
		SpellUI SpellUI = GetComponent<SpellUI>();
		SpellUI.OriginalParent = transform;
        foreach (GameObject Icon in Resources.LoadAll<GameObject>("Prefabs/SpellsIcons/")) {
            Add(Icon.name.Replace("UI",""));
        }
    }
    void Add(string name) {
        SpellIcon Icon = gameObject.AddComponent<SpellIcon>();
        Icon.Spellname = name;
        SpellIcons.Add(Icon);
    }
	void OnGUI(){
		Number = 0;
        foreach (SpellIcon Icon in SpellIcons) {
            if (Icon.Purchased == false) {
                if (GUI.Button(new Rect(25f + (Number > PageAmount ? PageDistance : 0), 25f + 37.5f /*40*/ * Number - (Number > PageAmount ? 440 : 0), 150f, 30f), "Learn " + Icon.Spellname)) {
                    UI = Instantiate(Resources.Load<GameObject>("Prefabs/SpellsIcons/" + Icon.Spellname + "UI")) as GameObject;
                    Icon.Purchased= true;
                    Spawn();
                }
                Number += 1;
            }
        }
	}
	void Spawn(){
		Found = false;
		foreach (Transform child in GameObject.Find("SlotsUpper").transform)
		{
			Slot Slot = child.GetComponent<Slot>();
			if (Slot.item == null){
				UI.transform.parent = child.transform;
				Found = true;
				break;
			}
		}
		if (Found == false){
			foreach (Transform child in GameObject.Find("SlotsLower").transform)
			{
				Slot Slot = child.GetComponent<Slot>();
				if (Slot.item == null){
					UI.transform.parent = child.transform;
					break;
				}
			}
		}
	}
}
class SpellIcon : MonoBehaviour{
    public string Spellname {
        get; set;
    }
    public bool Purchased {
        get; set;
    }
}
/*
 * 			UI.transform.SetParent(transform, false);
			UI.transform.SetSiblingIndex(2);
			RectTransform Pos = UI.GetComponent<RectTransform> ();
			Pos.anchorMin = new Vector2(0.05f, 0.515f);
			Pos.anchorMax = new Vector2(0.26f, 0.975f);
			
			RectTransform SlotPos = GameObject.Find("SlotQ").GetComponent<RectTransform> ();
			Pos.anchorMin = SlotPos.anchorMin;
			Pos.anchorMax = SlotPos.anchorMax;
*/
//UI.transform.parent = GameObject.Find("SpellQ").transform;
//RectTransform Pos = UI.GetComponent<RectTransform> ();
//Pos.SetAsFirstSibling();
//Pos.rect.Set(0,0,0,0);
//Pos.sizeDelta = new Vector2(0,0); 
//Pos.anchorMin = new Vector2(0.05f, 0.515f);
//Pos.anchorMax = new Vector2(0.26f, 0.975f);
