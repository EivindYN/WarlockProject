using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SpellUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	Vector3 StartLocation;
	public static Transform StartParent;
	public static GameObject Item;
	public static Transform OriginalParent;

	#region IBeginDragHandler implementation
	public void OnBeginDrag(PointerEventData eventData){
		//Debug.Log ("On Begin Drag");
		Item = gameObject;
		StartLocation = transform.position;
		StartParent = transform.parent;
		transform.parent = OriginalParent.transform;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}
	#endregion

	#region IDragHandler implementation
	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("On Drag");
		transform.position = eventData.position;
	}
	#endregion

	#region IEndDragHandler implementation
	public void OnEndDrag(PointerEventData eventData){
		//Debug.Log ("On End Drag");
		if (transform.parent == StartParent || transform.parent == OriginalParent.transform) {
			transform.position = StartLocation;
			transform.parent = StartParent;            
        } else {
			Slot Slot = GetComponentInParent<Slot>();
			if (Slot.item != gameObject){                
			Slot.item.transform.parent = StartParent;
			}
		}
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
        Item = null;
	}
	#endregion
}