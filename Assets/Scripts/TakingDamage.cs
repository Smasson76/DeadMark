using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TakingDamage : MonoBehaviourPunCallbacks {

    [SerializeField]
    Image healthBar;

    public float health;
    public float startHealth = 100;
        
    Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void TakeDamage(float damage) {
        health -= damage;

        healthBar.fillAmount = health / startHealth;

        if (health <= 0f) {
            Die();
        }
    }
    
    void Die() {
        if (photonView.IsMine) {
            anim.SetBool("isDead", true);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn() {
        
        GameObject respawnText = GameObject.Find("RespawnText");

        float respawnTime = 8.0f;
        while (respawnTime > 0.0f) {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            transform.GetComponent<MovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning at: " + respawnTime.ToString(".00");
        }

        anim.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        transform.GetComponent<MovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth() {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }
}
