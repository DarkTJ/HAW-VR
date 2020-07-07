using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBubble : MonoBehaviour {

    AudioSource auso;

    float vol = 0.0f;

    void Start() {

        auso = GetComponent<AudioSource>();

        auso.Play();

    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag.Equals("player") == true) {
            while (vol < 1) {
                vol += 0.1f * Time.deltaTime;
                auso.volume = vol;
            }
        }

    }

    void OnTriggerExit(Collider other) {

        if (other.gameObject.tag.Equals("player") == true) {
            while (vol > 0) {
                vol -= 0.1f * Time.deltaTime;
                auso.volume = vol;
            }
        }

    }

}
