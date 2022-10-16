using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhotonManager : PunBehaviour {

	private string roomName = "";
	private RoomInfo[] roomsList;

	RoomOptions roomOptions;

    public static bool JoinRoom;

    public GameObject Player;

    public int PlayerID;

    public static PhotonManager Me;

    public GameObject[] Players = new GameObject[0];
    public void Start()
	{
        Me = this;
        JoinRoom = false;
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.sendRate = 50;
        PhotonNetwork.sendRateOnSerialize = 50;
    }
	public void OnGUI()
	{
        if (!PhotonNetwork.connected) {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        } else if (PhotonNetwork.room == null) {
            // Create Room
            roomName = GUI.TextField(new Rect(210, 80, 200, 20), roomName, 15);
            roomOptions = new RoomOptions { isVisible = true, isOpen = true, maxPlayers = 4 };
            if (GUI.Button(new Rect(210, 100, 100, 30), "Start Server"))
                PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
            //if (GUI.Button(new Rect(100, 140, 100, 30), "Start Server with password"))
            //    PhotonNetwork.CreateRoom(roomName + "aaaaaaaaaaaaaaaaaaaa", roomOptions, TypedLobby.Default);

            // Join Room
            if (roomsList != null) {
                int rom = roomsList.Length;

                for (int i = 0; i < rom; i++) {
                    if (roomsList[i].name.Contains("abcdefghijklmnopqrstu")) {
                        if (GUI.Button(new Rect(210, 100 + (70 + 30 * i), 200, 30), "JoinPassword " + roomsList[i].name.Replace("abcdefghijklmnopqrstu", ""))) {
                            PhotonNetwork.JoinRoom(roomsList[i].name);
                        }
                    } else {
                        if (GUI.Button(new Rect(210, 100 + (70 + 30 * i), 200, 30), "Join " + roomsList[i].name)) {
                            PhotonNetwork.JoinRoom(roomsList[i].name);
                        }
                    }
                }
            }
        } else {
            if (!Player)
            if (GUI.Button(new Rect(210, 100, 100, 30), "Spawn Player")) {
                PlayerID = GameObject.FindGameObjectsWithTag("Player").Length;
                Player = PhotonNetwork.Instantiate("Player", new Vector2(0, 0), Quaternion.identity, 0);
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().target = Player.transform;
                Player.GetComponent<PhotonView>().RPC("SetID", PhotonTargets.All, PlayerID);
            }
            if (GUI.Button(new Rect(210, 140, 100, 30), "Reset")) {
                GetComponent<PhotonView>().RPC("Reset", PhotonTargets.All);
            }
        }
	}
    [PunRPC]
    void Reset() {
        Destroy(FindMyPlayer());
        Player = PhotonNetwork.Instantiate("Player", new Vector2(0, 0), Quaternion.identity, 0);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().target = Player.transform;
        Player.GetComponent<PhotonView>().RPC("SetID", PhotonTargets.All, PlayerID);
    }
    GameObject FindMyPlayer()
    {
        GameObject MyPlayer = null;
        foreach (GameObject G in GameObject.FindGameObjectsWithTag("Player")) {
            if (G.GetComponent<PhotonInfo>().PlayerID == PlayerID)
            {
                MyPlayer = G;
            }
        }
        return MyPlayer;
    }
    void Update() {
        
    }
	void OnReceivedRoomListUpdate()
	{
		roomsList = PhotonNetwork.GetRoomList();	
	}
	public override void OnJoinedRoom()
	{
		Debug.Log ("Connected to Room");
	}
}
