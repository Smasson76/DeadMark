using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AlienController : MonoBehaviourPunCallbacks {
    
    [Header("VARIABLES")]
    public float speed = 2;
    public float chaseDist = 5;
    public float turnSpeed = 1;
    public float attackDist = 2;

    //bool isDead = false;
    
    public enum State {
        Idle,
        Chase,
        Attack,
        Dead
    }

    public State state = State.Idle;

    Animator anim;
    EnemyHealth health;
    public GameObject player;
    Rigidbody body;

    void Awake() {
        //photonView.ViewID = 999;
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
        //anim.ApplyBuiltinRootMotion();
    }

    void Update() {
        if (player == null) {
            anim.SetBool("isRunning", false);
            body.velocity = Vector3.zero;
            return;
        }

        if (health.health <= 0) body.velocity = Vector3.zero;

        switch (state) {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Chase:
                ChaseUpdate();
                break;
            case State.Attack:
                StartCoroutine(Attack());
                break;
            case State.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }
    }

    void IdleUpdate() {
        anim.SetBool("isRunning", false);
        body.velocity = Vector3.zero;
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < chaseDist) {
            state = State.Chase;
        }
    }

    void ChaseUpdate() {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Vector3 cross = Vector3.Cross(transform.forward, dir);
        transform.Rotate(Vector3.up * cross.y * turnSpeed * Time.deltaTime);

        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist > chaseDist) {
            state = State.Idle;
        }
        else if (dist < attackDist) {
            state = State.Attack;
            body.velocity = Vector3.zero;
        }
        else {
            anim.SetBool("isRunning", true);
        }
    }

    void DeadUpdate() {
        body.velocity = Vector3.zero;
    }

    IEnumerator Attack() {
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(3f);
        if (player == null) {
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", false);
            body.velocity = Vector3.zero;
        }
        else {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist > attackDist) {
                anim.SetBool("isAttacking", false);
                state = State.Chase;
            }
        }
    }
}
