using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour {

    void Start() {
        transform.Find("vapor").GetComponent<ParticleSystem>().Stop();
        transform.Find("Water").gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other) {
        if (!other.gameObject.name.Equals("kitchen")) {
            if (other.gameObject.name.Equals(MainMenu.Recipe.actions[Kitchen.actualAction].prefab)) {
                MainMenu.Recipe.actions[Kitchen.actualAction].done = true;
                GameObject.Find("HUD").GetComponent<HUD>().TP++;
                GameObject.Find("Kitchen").GetComponent<Kitchen>().NextAction();
                if (other.gameObject.name.Equals("water")) {
                    transform.Find("Water").gameObject.SetActive(true);
                } else if (other.gameObject.name.Equals("sauce")) {
                    transform.Find("Water").GetComponent<MeshRenderer>().materials[0].color = Color.red;
                } else if (other.gameObject.name.Equals("pasta")) {
                    GameObject food = Instantiate(other.gameObject.transform.Find("pasta").gameObject, transform);
                    food.name = other.gameObject.name;
                    if (gameObject.name.Equals("Saucepan")) {
                        food.transform.localScale /= 30f;
                        food.transform.localPosition = new Vector3(0, 0.113f, 0);
                    } else {
                        food.transform.localScale /= 1500;
                        food.transform.localPosition = new Vector3(0, 0, 0.00134f);
                    }
                } else {
                    GameObject food = Instantiate(other.gameObject, transform);
                    food.name = other.gameObject.name;
                    food.GetComponent<Food>().enabled = false;
                    food.GetComponent<BoxCollider>().enabled = false;
                    (food.GetComponent("Halo") as Behaviour).enabled = false;
                    if (gameObject.name.Equals("Saucepan")) {
                        food.transform.localScale /= 42.5f;
                        food.transform.localPosition = new Vector3(0, 0.1223f, 0);
                    } else {
                        food.transform.localScale /= 1700;
                        food.transform.localPosition = new Vector3(0, 0, 0.00134f);
                    }
                }
            } else {
                MainMenu.Recipe.actions[Kitchen.actualAction].fp++;
                GameObject.Find("HUD").GetComponent<HUD>().FP++;
            }
            Destroy(other.gameObject);
        }
    }

    public void Play() {
        transform.Find("vapor").GetComponent<ParticleSystem>().Play();
    }

    public void Stop() {
        transform.Find("vapor").GetComponent<ParticleSystem>().Stop();
    }

}
