using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks {

    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject alienPrefab;

    void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
        }
        else {
            instance = this;
        }
    }

    void Start() {
        if (PhotonNetwork.IsConnected) {
            if (playerPrefab != null) {
                int randomPoints = Random.Range(0, 2);
                Vector3[] points = new Vector3[] {
                    new Vector3(11.45f, 2.32f, -0.52f),
                    new Vector3(-11.45f, 2.32f, -0.52f)
                    //new Vector3(-4f, 1.45f, 15f),
                    //new Vector3(-4f, 1.45f, -16f),
                    //new Vector3(-25f, 1.45f, -16f),
                    //new Vector3(-25f, 1.45f, 10f)
                };
                //PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoints, 0, randomPoints), Quaternion.identity);
                PhotonNetwork.Instantiate(playerPrefab.name, points[randomPoints], Quaternion.identity);
            }
        }
        //StartCoroutine(StartAlienSpawn());
    }

    IEnumerator StartAlienSpawn() {
        while (enabled) { 
            yield return new WaitForSeconds(10);
            for (int i = 0; i < 15; i++) {
                int nextEnemyPoint = Random.Range(0, 2);
                Vector3[] points = new Vector3[] {
                    new Vector3(431f, 17.307f, 517f),
                    new Vector3(431f, 17.307f, 517f)
                    //new Vector3(-32f, 0f, 120f),
                    //new Vector3(117f, 0f, 101.9f),
                    //new Vector3(-95.6f, 0f, 70.6f),
                   //new Vector3(188f, 0f, 84.9f),
                    //new Vector3(236.7f, 0f, 211f)
                };
                PhotonNetwork.Instantiate(alienPrefab.name, points[nextEnemyPoint], Quaternion.identity);
            }
        }
    }

    public override void OnJoinedRoom() {
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnLeftRoom() {
        SceneManager.LoadScene("GameLauncherScene");
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }
}
