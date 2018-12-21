using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour {

    [SerializeField] public GameObject Menu;
    [SerializeField] public GameObject ActionPrefab;
    public string Title { get; private set; }
    public MainMenu.ActionStruct[] Actions { get; private set; }

    // Use this for initialization
    void Start() {
        if (!Menu) {
            Debug.LogError("Menu not found");
            return;
        }
        MainMenu.Interactable = false;
        LoadRecipe(MainMenu.Recipe.title, MainMenu.Recipe);
        transform.Find("CloseBtn").gameObject.GetComponent<Button>().onClick.AddListener(Close);
    }

    public void LoadRecipe(string t, MainMenu.RecipeStruct r) {
        Title = t;
        transform.Find("TitleLbl").gameObject.GetComponent<TextMeshProUGUI>().text = Title;
        Transform content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (40 * r.actions.Length));
        GameObject actionGO;
        Actions = r.actions; 
        for (int i = 0; i < Actions.Length; i++) {
            actionGO = Instantiate(ActionPrefab, content);
            actionGO.name = "Action" + i;
            actionGO.transform.Find("ActionLbl").GetComponent<Text>().text = Actions[i].name;
            actionGO.transform.localPosition -= new Vector3(0, 40 * i);
            actionGO.transform.Find("CheckImg").gameObject.SetActive(Actions[i].done);
        }
    }

    void Close() {
        gameObject.transform.parent.GetComponent<HUD>().CloseRecipe();
        MainMenu.Interactable = true;
    }

}