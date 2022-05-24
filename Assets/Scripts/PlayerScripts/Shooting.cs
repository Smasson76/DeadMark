using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks {
    
    [SerializeField]
    Camera fpsCam;

    PlayerSetup playerSetup;

    public GameObject envHitEffectPrefab;
    public GameObject enemyHitEffectPrefab;

    //Variables
    public float fireRate = 0.1f;
    float fireTimer;
    float totalDamage = 0f;

    //Gun sounds and effects
    public AudioSource gunSounds;
    public AudioClip gunShot;
    public ParticleSystem muzzleFlash;


    void Start() {
        playerSetup = GetComponent<PlayerSetup>();
    }

    void Update() {
        if (fireTimer < fireRate) {
            fireTimer += Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && fireTimer > fireRate) {

            //Reset fireTimer
            fireTimer = 0.0f;
            
            RaycastHit hit;
            Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 500, 1 << LayerMask.NameToLayer("Target"))) {
                if (this.gameObject.GetComponent<TakingDamage>().health > 0f) {
                    
                    if (photonView.IsMine) {
                        photonView.RPC("CreateShotSound", RpcTarget.All, hit.point);
                        photonView.RPC("CreateMuzzleFlash", RpcTarget.All, hit.point);
                    }
                    
                    if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine) {
                        if (photonView.IsMine) {
                            photonView.RPC("EnemyHitEffect", RpcTarget.All, hit.point);
                        }
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
                        totalDamage += 10f;
                        playerSetup.damageText.text = "Damage: " + totalDamage;

                        //hit.collider.gameObject.GetComponent<PhotonView>().RPC("GetKill", RpcTarget.AllBuffered, 1f);
                    }

                    if (hit.collider.gameObject.CompareTag("Enemy")) {
                        if (photonView.IsMine) {
                            photonView.RPC("EnemyHitEffect", RpcTarget.All, hit.point);
                        }
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
                        totalDamage += 10f;
                        playerSetup.damageText.text = "Damage: " + totalDamage;
                    }

                    if (hit.collider.gameObject.CompareTag("Environment")) {
                        if (photonView.IsMine) {
                            photonView.RPC("EnvironementHitEffect", RpcTarget.All, hit.point);
                        }
                    }
                }
            }
        }    
    }

    [PunRPC]
    public void EnvironementHitEffect(Vector3 pos) {
        GameObject hitEffectGameobject = Instantiate(envHitEffectPrefab, pos, Quaternion.identity);
        Destroy(hitEffectGameobject, 0.3f);
    }

    [PunRPC]
    public void EnemyHitEffect(Vector3 pos) {
        GameObject hitEffectGameobject = Instantiate(enemyHitEffectPrefab, pos, Quaternion.identity);
        Destroy(hitEffectGameobject, 0.3f);
    }

    [PunRPC]
    public void CreateShotSound(Vector3 pos) {
        gunSounds.PlayOneShot(gunShot);
    }

    [PunRPC]
    public void CreateMuzzleFlash(Vector3 pos) {
        muzzleFlash.Play();
    }
}
