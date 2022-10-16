using UnityEngine;
using System.Collections;

public class UIFollow : MonoBehaviour {
    public Transform Target;
    void Update() {
        Vector3 Position = Target.position;
        Position.y += 1.8f;
        Position.x += 0.1f;
        Vector3 wantedPos = Camera.main.WorldToScreenPoint(Position);
        transform.position = wantedPos;
    }
}
