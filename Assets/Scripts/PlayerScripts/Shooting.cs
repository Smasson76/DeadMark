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
    public float totalKills = 0f;

    public float bulletsInChamper = 25f;
    public float bulletsForReload = 150f;

    public ParticleSystem muzzleFlash;
    public AudioSource playerAudioSource;


    void Start() {
        playerSetup = GetComponent<PlayerSetup>();
    }

    void Update() {
        if (fireTimer < fireRate) {
            fireTimer += Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && fireTimer > fireRate && bulletsInChamper > 0) {

            //Reset fireTimer
            fireTimer = 0.0f;

            bulletsInChamper--;
            
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
                            photonView.RPC("CreateHitMarkerSound", RpcTarget.All, hit.point);
                        }
                        if (hit.collider.gameObject.GetComponent<TakingDamage>().health <= 0f) {
                            Debug.Log("Should get kill");
                            totalKills++;
                        }
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
                        totalDamage += 10f;
                        playerSetup.damageText.text = "Damage: " + totalDamage;
                        playerSetup.killsText.text = "Kills: " + totalKills;
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

        if (Input.GetKeyDown(KeyCode.R) && bulletsForReload > 0 && bulletsInChamper < 25) {
            BulletsAction();
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
        playerAudioSource.PlayOneShot(SoundManager.instance.soundClips[0]);
    }

    [PunRPC]
    public void CreateMuzzleFlash(Vector3 pos) {
        muzzleFlash.Play();
    }
    
    [PunRPC]
    public void CreateHitMarkerSound(Vector3 pos) {
        //SoundManager.instance.soundEffectsSource.PlayOneShot(SoundManager.instance.soundClips[1]);
    }

    void BulletsAction() {
        if (bulletsForReload < 25) {
            bulletsInChamper += bulletsForReload;
            bulletsForReload -= bulletsInChamper;
        }
        else {
            float newBullets = 25f - bulletsInChamper;
            bulletsInChamper += newBullets;
            bulletsForReload -= newBullets;
        }
    }
}
