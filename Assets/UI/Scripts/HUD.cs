using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject RecipePrefab;
    [SerializeField] public GameObject FoodContainer;
    [SerializeField] public GameObject kitchen;
    [SerializeField] public GameObject clock;
    [SerializeField] public float hintTime = 3;

    public int HintCount { get; private set; }
    public int ShowRecipeCount { get; private set; }

    public int TP;

    public int FP;

    private GameObject recipePnl;

    // Use this for initialization
    void Start () {
        if (clock) {
            clock.SetActive(false);
        }else{
            Debug.LogError("clock not found");
        }
        HintCount = 0;
        ShowRecipeCount = 0;
        TP = 0;
        FP = 0;
        transform.Find("PauseBtn").gameObject.GetComponent<Button>().onClick.AddListener(Pause);
        transform.Find("HintBtn").gameObject.GetComponent<Button>().onClick.AddListener(Hint);
        transform.Find("ShowRecipeBtn").gameObject.GetComponent<Button>().onClick.AddListener(ShowRecipe);
    }

    void Pause() {
        if (!Menu) {
            Debug.LogError("Menu not found");
            return;
        }
        CloseRecipe();
        gameObject.SetActive(false);
        Menu.transform.Find("PauseMenu").gameObject.SetActive(true);
        MainMenu.Pause = true;
        MainMenu.Interactable = false;
    }

    IEnumerator DoHint(GameObject prefab) {
        (prefab.GetComponent("Halo") as Behaviour).enabled = true;
        yield return new WaitForSeconds(hintTime);
        if (prefab) {
            (prefab.GetComponent("Halo") as Behaviour).enabled = false;
        }
    }

    void Hint() {
        if (!MainMenu.Pause && MainMenu.Interactable && Kitchen.actualAction < MainMenu.Recipe.actions.Length) {
            switch (MainMenu.Recipe.actions[Kitchen.actualAction].type) {
                case "movement":
                    if (MainMenu.Recipe.actions[Kitchen.actualAction].prefab.Length > 0) {
                        HintCount++;
                        if (FoodContainer) {
                            GameObject food = FoodContainer.transform.Find(MainMenu.Recipe.actions[Kitchen.actualAction].prefab).gameObject;
                            StartCoroutine(DoHint(food));
                        } else {
                            Debug.LogError("FoodContainer not found");
                        }
                    }
                    break;
                case "action":
                    if (MainMenu.Recipe.actions[Kitchen.actualAction].target.Equals("hotPlates")) {
                        HintCount++;
                        if (kitchen) {
                            //GameObject knob = kitchen.transform.Find("kitchen").Find("hotPlates").Find("Knob1"/* + MainMenu.Recipe.cooker*/).gameObject;
                            //StartCoroutine(DoHint(knob));
                            StartCoroutine(DoHint(kitchen.transform.Find("kitchen").Find("hotPlates").Find("Knob1").gameObject));
                            StartCoroutine(DoHint(kitchen.transform.Find("kitchen").Find("hotPlates").Find("Knob2").gameObject));
                            StartCoroutine(DoHint(kitchen.transform.Find("kitchen").Find("hotPlates").Find("Knob3").gameObject));
                            StartCoroutine(DoHint(kitchen.transform.Find("kitchen").Find("hotPlates").Find("Knob4").gameObject));
                        } else {
                            Debug.LogError("kitchen not found");
                        }
                    }
                    break;
            }
        }
    }

    public void ShowRecipe() {
        if (!MainMenu.Pause && MainMenu.Interactable) {
            if (!RecipePrefab) {
                Debug.LogError("RecipePrefab not found");
                return;
            }
            if (!Menu) {
                Debug.LogError("Menu not found");
                return;
            }
            if (transform.Find("ShowRecipeBtn").GetComponent<Button>().interactable) {
                transform.Find("ShowRecipeBtn").GetComponent<Button>().interactable = false;
                ShowRecipeCount++;
                RecipePrefab.GetComponent<Recipe>().Menu = Menu;
                recipePnl = Instantiate(RecipePrefab, transform);
                recipePnl.name = "RecipePnl";
            }
        }
    }

    public void CloseRecipe() {
        if (!transform.Find("ShowRecipeBtn").GetComponent<Button>().interactable) {
            Destroy(recipePnl);
            transform.Find("ShowRecipeBtn").GetComponent<Button>().interactable = true;
        }
    }

    public void Restart() {
        HintCount = 0;
        ShowRecipeCount = 0;
        TP = 0;
        FP = 0;
        if (clock) {
            clock.SetActive(false);
        }
    }

    public void ShowTimer(int min) {
        if (clock == null) {
            Debug.LogError("clock not found");
            return;
        }
        MainMenu.Interactable = false;
        clock.transform.Find("Panel").Find("clock").GetComponent<Clock>().minutes = min;
        clock.SetActive(true);
    }

    public void CloseTimer() {
        if (kitchen) {
            MainMenu.Recipe.actions[Kitchen.actualAction].done = true;
            kitchen.GetComponent<Kitchen>().NextAction();
        } else {
            Debug.LogError("kitchen not found");
        }
    }

}
