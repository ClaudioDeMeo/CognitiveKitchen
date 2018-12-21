using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Stop();
	}

    public void Play() {
        foreach (Transform child in transform) {
            child.GetComponent<ParticleSystem>().Play();
        }
    }

    public void Stop() {
        foreach (Transform child in transform) {
            child.GetComponent<ParticleSystem>().Stop();
        }
    }

}
