using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviourPunCallbacks {
    
    public float health;
    public float startHealth = 100;
        
    Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
        health = startHealth;
    }

    [PunRPC]
    public void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0f) {
            Die();
        }
    }

    void Die() {
        if (photonView.IsMine) {
            anim.SetTrigger("death");
            StartCoroutine(Death());
        }
    }

    IEnumerator Death() {
        yield return new WaitForSeconds(8f);
        Destroy(this.gameObject);
    }
}
