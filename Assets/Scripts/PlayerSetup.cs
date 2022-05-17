using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks {

    [SerializeField]
    GameObject fpsCam;

    [SerializeField]
    TextMeshProUGUI playerNameText;

    public GameObject[] fpsHandsGameObject;
    public GameObject[] soldierGameObject;

    //Texts
    public Text killsText;
    public Text damageText;

    public GameObject pauseMenu;
    
    public float kills = 0f;

    Animator anim;
    
    void Start() {
        anim = GetComponent<Animator>();
        pauseMenu.SetActive(false);
        if (photonView.IsMine) {

            //Activate FPS hands, Deactivate Soldier
            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(true);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(false);
            }

            anim.SetBool("isSoldier", false);
            transform.GetComponent<MovementController>().enabled = true;
            fpsCam.GetComponent<Camera>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            damageText.text = "Damage: 0";
            killsText.text = "Kills: 0";
        }
        else {

            //Activate Soldier, Deactivate FPS Hands
            foreach (GameObject gameObject in fpsHandsGameObject) {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in soldierGameObject) {
                gameObject.SetActive(true);
            }
            
            anim.SetBool("isSoldier", true);
            transform.GetComponent<MovementController>().enabled = false;
            fpsCam.GetComponent<Camera>().enabled = false;
        }

        SetPlayerUI();
    }

    void Update() {
        if (photonView.IsMine) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                pauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetKeyDown(KeyCode.Escape)) {
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void Resume() {
        pauseMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame() {
        if (photonView.IsMine) {
            GameManager.instance.LeaveRoom();
        }
    }

    void SetPlayerUI() {
        if (playerNameText != null) {
            playerNameText.text = photonView.Owner.NickName;
        }
    }

    [PunRPC]
    public void GetKill(float kill) {
        if (this.gameObject.GetComponent<TakingDamage>().health <= 0f) {
            //kills += kill;
            //killsText.text = "Kills: " + kills;
        }
    }
}
