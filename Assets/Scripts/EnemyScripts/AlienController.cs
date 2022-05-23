using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class AlienController : MonoBehaviourPunCallbacks {
    
    [Header("VARIABLES")]
    public float speed = 3f;
    public float turnSpeed = 500f;
    public float attackDist = 1.5f;

    public enum State {
        Idle,
        Chase,
        Attack,
        Dead
    }

    public State state = State.Idle;

    Animator anim;
    Transform player;
    Rigidbody body;
    EnemyHealth health;

    void Awake() {
        body = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();
    }

    void Update() {
        switch (state) {
			case State.Idle:		
				IdleUpdate();
				break;
			case State.Chase:
				ChaseUpdate();
				break;
			default:
				break;
		}
    }

    void IdleUpdate() {
        body.velocity = Vector3.zero;

        if (PlayerFound() == true) {
            state = State.Chase;
        }
    }

    void ChaseUpdate() {
        Vector3 dir  = (player.position - transform.position).normalized;
		Vector3 cross = Vector3.Cross(transform.forward, dir);
		transform.Rotate(Vector3.up * cross.y * turnSpeed * Time.deltaTime);

        float distance = Vector3.Distance(player.position, this.transform.position);
        if (PlayerFound() == true) {
            if (distance < attackDist) {
                state = State.Attack;
                body.velocity = Vector3.zero;
                anim.SetBool("isAttacking", true);
                anim.SetBool("isRunning", false);
                Debug.Log("should be attacking");
            }
            else {
                body.velocity = dir * speed;
                anim.SetBool("isRunning", true);
                anim.SetBool("isAttacking", false);
                Debug.Log("should be running");
            }
        }
        else {
            state = State.Idle;
        }
    }

    bool PlayerFound() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null) return false;
        else return true;
    }
}
