using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panels : MonoBehaviour {

    public GameObject panel;

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag.Equals("player") == true) {
            panel.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other) {

        if (other.gameObject.tag.Equals("player") == true) {
            panel.SetActive(false);
        }

    }

}
