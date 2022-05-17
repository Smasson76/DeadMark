using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienCheckForPlayer : MonoBehaviour {

    GameObject alienController;

    void Start() {
        alienController = this.transform.parent.gameObject; 
    }
    
    void OnTriggerStay(Collider c) {
        if (c.gameObject.CompareTag("Player")) {
            alienController.GetComponent<AlienController>().player = c.gameObject;
        }
    }
    
    void OnTriggerExit(Collider c) {
        if (c.gameObject.CompareTag("Player")) {
            alienController.GetComponent<AlienController>().player = null;
        }
    }
}
