using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kitchen : MonoBehaviour {

    [SerializeField] public GameObject FoodContainer;
    [SerializeField] public GameObject hud;
    [SerializeField] public GameObject Menu;

    private MainMenu.ActionStruct[] actions;
    public static int actualAction;
    private GameObject pot;
    private int cooker;
    public float time;

    void Start() {
        actualAction = -1;
        time = 0;
    }

    void Update() {
        if (actions!=null && actualAction >= 0 && actualAction < actions.Length && !MainMenu.Pause) {
            actions[actualAction].duration += Time.deltaTime;
            time += Time.deltaTime;
        }
    }

    public void StartRecipe(MainMenu.RecipeStruct Recipe) {
        actions = Recipe.actions;
        if (Recipe.prefab.Length > 0) {
            SpawnPot(Recipe.prefab, Recipe.cooker);
        }
        actualAction = -1;
        time = 0;
        NextAction();
    }

    public void NextAction() {
        actualAction++;
        if (actualAction < actions.Length) {
            switch (actions[actualAction].type) {
                case "movement":
                    if (actions[actualAction].prefab.Length > 0) {
                        SpawnFood(actions[actualAction].prefab, 12, 4);
                    }
                    break;
                case "wait":
                    if (actions[actualAction].temp > 0) {
                        if (hud) {
                            hud.GetComponent<HUD>().ShowTimer(actions[actualAction].temp);
                        } else {
                            Debug.LogError("hud not found");
                        }
                    }
                    break;
            }
        } else {
            //Finish game
            if (Menu) {
                Menu.GetComponent<MainMenu>().Finish();
            } else {
                Debug.LogError("Menu not found");
            }
        }
    }

    private void SpawnPot(string prefabName, int c) {
        if (pot) {
            Destroy(pot);
        }
        cooker = c;
        Transform Cooker = transform.Find("Cooker" + cooker);
        pot = Instantiate(Resources.Load<GameObject>("Prefabs/" + prefabName), Cooker);
        pot.name = prefabName;
    }

    public void ResetPot() {
        for(int i = 1; i <= 4; i++) {
            Transform Cooker = transform.Find("Cooker" + i);
            foreach (Transform child in Cooker) {
                Destroy(child.gameObject);
            }
        }
    }

    private void SpawnFood(string prefabName, int n, int c) {
        Restart();
        System.Random rnd = new System.Random();
        //GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Food");
        ArrayList list = new ArrayList();
        for (int i=0; i < actions.Length; i++) {
            GameObject pr = Resources.Load<GameObject>("Prefabs/Food/" + actions[i].prefab);
            if (actions[i].prefab.Length > 0 && !list.Contains(pr)) {
                list.Add(pr);
            }
        }
        GameObject[] prefabs = (GameObject[])list.ToArray(typeof(GameObject));
        int pref = Array.FindIndex(prefabs, p => string.Equals(p.name, prefabName));
        int newPos = rnd.Next(prefabs.Length - 1);
        ArrayList exclude = new ArrayList {pref};
        int row = 0;
        int col = 0;
        //for (int i = 0; i < n; i++) {
        for (int i = 0; i < prefabs.Length; i++) {
            GameObject food;
            int pos = rnd.Next(prefabs.Length - exclude.Count);
            if (i == newPos) {
                food = Instantiate(prefabs[pref], FoodContainer.transform);
                food.name = prefabs[pref].name;
            } else {
                IEnumerable<int> range = Enumerable.Range(0, prefabs.Length).Where(r => !exclude.Contains(r));
                int value = range.ElementAt(pos);
                food = Instantiate(prefabs[value], FoodContainer.transform);
                food.name = prefabs[value].name;
                exclude.Add(value);
            }
            food.transform.localPosition += new Vector3(3.5f * col, 0, -3.5f * row);
            if (i == c - 1 || i == ((row + 1) * c) - 1) {
                col = 0;
                row++;
            } else {
                col++;
            }
        }
    }

    public void Restart() {
        foreach(Transform child in FoodContainer.transform) {
            Destroy(child.gameObject);
        }
    }

    public void StopFlame() {
        Transform hotPlates = transform.Find("kitchen").Find("hotPlates");
        foreach (Knob knob in hotPlates.GetComponentsInChildren<Knob>()) {
            knob.Stop();
        }
        foreach (Flame flame in hotPlates.GetComponentsInChildren<Flame>()) {
            flame.Stop();
        }
    }

    public void RegisterAction(int c, bool isOn) {
        if (actions[actualAction].type.Equals("action")) {
            if (isOn) {
                transform.Find("kitchen").Find("hotPlates").Find("Flame" + cooker).GetComponent<Flame>().Play();
                transform.Find("Cooker" + cooker).GetComponentInChildren<Pot>().Play();
            } else {
                transform.Find("kitchen").Find("hotPlates").Find("Flame" + cooker).GetComponent<Flame>().Stop();
                transform.Find("Cooker" + cooker).GetComponentInChildren<Pot>().Stop();
            }
            if (/*cooker == c && */isOn == actions[actualAction].on) {
                //incrementare azioni giuste
                MainMenu.Recipe.actions[actualAction].done = true;
                NextAction();
            } else {
                //incrementare azioni sbagliate
            }
        } else {
            //incrementare azioni inutili (sbagliate?)
        }
    }

}
