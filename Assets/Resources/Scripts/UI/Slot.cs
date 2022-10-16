using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler{
	string Number;
	int n;
	public GameObject item {
		get {
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
        int FirstSpell = Convert.ToInt32(transform.name.Replace("Slot", "")) - 1;
        int SecondSpell = Convert.ToInt32(SpellUI.StartParent.name.Replace("Slot", "")) - 1;
        Debug.Log(FirstSpell);
        Debug.Log(SecondSpell);
        GameObject.Find("GameManager").GetComponent<GameManager>().SwapCooldowns(FirstSpell, SecondSpell);
        SpellUI.Item.transform.SetParent(transform);

    }
	#endregion
	void Start(){
		Number = gameObject.name;
		Number = Number.Replace ("Slot", "");
		n = int.Parse(Number);
	}
	void Update(){
		if (item != null) {
			GameManager.Slot[n] = item.name;
		} else {
			GameManager.Slot[n] = null;
		}
	}
}
