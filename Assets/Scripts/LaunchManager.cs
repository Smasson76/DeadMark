using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks {    

    [Header("PUBLIC MENU OBJECTS")]
    public GameObject nameCreationPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;
    public GameObject OpeningPanel;

    // Awake() - sets mouse to lock to screen and calls PhotonNetwork to sync scene automatically
    void Awake() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //Start() - Sets every panel inactive except OpeningPanel
    void Start() {
        OpeningPanel.SetActive(true);
        nameCreationPanel.SetActive(false);
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);
    }

    void Update() {
        if (Input.anyKey) {
            OpeningPanel.SetActive(false);
            ConnectToPhotonServer();
        }
    }

    public void OnEnterLobbyButton() {
        nameCreationPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }

    public void OnlinePlayButton() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToPhotonServer() {
        if (!PhotonNetwork.IsConnected) {
            PhotonNetwork.ConnectUsingSettings();
            ConnectionStatusPanel.SetActive(true);
            nameCreationPanel.SetActive(false);
        }
    }

    public void JoinRandomRoom() {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster() {
        Debug.Log(PhotonNetwork.NickName + " Connected to photon servers");
        nameCreationPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }

    public override void OnConnected() {
        Debug.Log("Connected to Internet.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CreateAndJoinRoom();
    }

    public override void OnJoinedRoom() {
        Debug.Log(PhotonNetwork.NickName + " joined to" + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    void CreateAndJoinRoom() {
        string randomRoomName = "Room " + Random.Range(0,1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }
}
