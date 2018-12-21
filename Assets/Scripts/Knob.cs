using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour {

    [SerializeField] public GameObject Flame;
    [SerializeField] public GameObject kitchen;
    [SerializeField] public int cooker;

    void OnValidate() {
        if (cooker >= 1 && cooker <= 4) {
            return;
        }
        cooker = 1;
    }

    public bool isOn { get; private set; }

    // Use this for initialization
    void Start () {
        isOn = false;
        (gameObject.GetComponent("Halo") as Behaviour).enabled = false;
    }

    void OnMouseDown() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            (gameObject.GetComponent("Halo") as Behaviour).enabled = true;
            if (!Flame) {
                Debug.LogError("Flame not found");
                return;
            }
            if (isOn) {
                Stop();
                if (kitchen) {
                    kitchen.GetComponent<Kitchen>().RegisterAction(cooker, isOn);
                } else {
                    Debug.LogError("kitchen not found");
                }
            } else {
                Play();
                if (kitchen) {
                    kitchen.GetComponent<Kitchen>().RegisterAction(cooker, isOn);
                } else {
                    Debug.LogError("kitchen not found");
                }
                
            }
        }
    }

    public void Play() {
        //Flame.GetComponent<Flame>().Play();
        isOn = true;
    }

    public void Stop() {
        //Flame.GetComponent<Flame>().Stop();
        isOn = false;
    }

    void OnMouseEnter() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            //(gameObject.GetComponent("Halo") as Behaviour).enabled = true;
            Transform kn = GameObject.Find("hotPlates").transform;
            foreach(Transform c in kn) {
                if (c.gameObject.name.Contains("Knob")) {
                    (c.gameObject.GetComponent("Halo") as Behaviour).enabled = true;
                }
            }
        }
    }

    void OnMouseExit() {
        //(gameObject.GetComponent("Halo") as Behaviour).enabled = false;
        Transform kn = GameObject.Find("hotPlates").transform;
        foreach (Transform c in kn) {
            if (c.gameObject.name.Contains("Knob")) {
                (c.gameObject.GetComponent("Halo") as Behaviour).enabled = false;
            }
        }
    }

}
