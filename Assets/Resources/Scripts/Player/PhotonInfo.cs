using UnityEngine;
using System;
using System.Collections;

public class PhotonInfo : MonoBehaviour {
    public int PlayerID;
    [PunRPC]
    void SetID(int ID) {
        PlayerID = ID;
        if (PhotonManager.Me.Players.Length < ID+1) {
            Array.Resize(ref PhotonManager.Me.Players, ID + 1);
        }
        PhotonManager.Me.Players[ID] = gameObject;
    }
}
