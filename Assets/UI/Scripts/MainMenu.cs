using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public struct ActionStruct {
        public string name;
        public string prefab;
        public string target;
        public string type;
        public int temp;
        public bool done;
        public bool on;
        public float duration;
        public int fp;
    }

    public struct RecipeStruct {
        public string title;
        public string prefab;
        public string place;
        public ActionStruct[] actions;
        public int cooker;
    }

    [SerializeField] public Camera cam1;
    [SerializeField] public Camera cam2;
    [SerializeField] public TextAsset recipesXML;
    [SerializeField] public GameObject recipeBtnPrefab;
    [SerializeField] public GameObject HUD;
    [SerializeField] public GameObject kitchen;

    public static RecipeStruct[] recipes;
    public static  RecipeStruct Recipe { get; private set; }

    public static bool Pause = true;
    public static bool Interactable = false;
    public DateTime init;

    public string Name {
        get {
            name = getElement("NameInputField");
            if (name.Length > 0) {
                return name;
            } else {
                return "u-" + DateTime.Now.GetHashCode().ToString();
            }
        }
    }

    public string Sex {
        get {
            if (transform.Find("MainMenu").Find("Panel").Find("SexInput").Find("MaleToggle").GetComponent<Toggle>().isOn) {
                return "male";
            } else if (transform.Find("MainMenu").Find("Panel").Find("SexInput").Find("FemaleToggle").GetComponent<Toggle>().isOn) {
                return "female";
            } else {
                return "";
            }
        }
    }

    public string Age {
        get {
            return getElement("AgeInputField");
        }
    }

    public string Disease {
        get {
            return getElement("DiseaseInputField");
        }
    }

    public string Annotation {
        get {
            return getElement("AnnotationInputField");
        }
    }
    
    void Start () {
        gameObject.SetActive(true);
        transform.Find("MainMenu").gameObject.SetActive(true);
        transform.Find("RecipesMenu").gameObject.SetActive(false);
        transform.Find("PauseMenu").gameObject.SetActive(false);
        transform.Find("FinishMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").Find("Panel").Find("PlayBtn").gameObject.GetComponent<Button>().onClick.AddListener(Play);
        transform.Find("MainMenu").Find("Panel").Find("QuitBtn").gameObject.GetComponent<Button>().onClick.AddListener(Quit);
        transform.Find("RecipesMenu").Find("Panel").Find("BackBtn").gameObject.GetComponent<Button>().onClick.AddListener(Back);
        transform.Find("RecipesMenu").Find("Panel").Find("RandomBtn").gameObject.GetComponent<Button>().onClick.AddListener(PlayRandom);
        transform.Find("PauseMenu").Find("Panel").Find("ResumeBtn").gameObject.GetComponent<Button>().onClick.AddListener(Resume);
        transform.Find("PauseMenu").Find("Panel").Find("ChangeBtn").gameObject.GetComponent<Button>().onClick.AddListener(Change);
        transform.Find("PauseMenu").Find("Panel").Find("ExitBtn").gameObject.GetComponent<Button>().onClick.AddListener(Exit);
        transform.Find("FinishMenu").Find("Panel").Find("SaveBtn").gameObject.GetComponent<Button>().onClick.AddListener(Save);
        transform.Find("FinishMenu").Find("Panel").Find("HomeBtn").gameObject.GetComponent<Button>().onClick.AddListener(Home);
        if (recipesXML) {
            recipes = ParseXML(recipesXML);
            if (recipes.Length > 0) {
                CreateRecipeBtns();
            }
        }
        if (HUD) {
            HUD.SetActive(false);
        }
    }

    void Play() {
        transform.Find("MainMenu").gameObject.SetActive(false);
        transform.Find("RecipesMenu").gameObject.SetActive(true);
    }

    void Quit() {
        Application.Quit();
    }

    void Back() {
        transform.Find("RecipesMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
    }

    void PlayRandom() {
        System.Random rnd = new System.Random();
        if (recipes.Length > 0) {
            Restart();
            int i = rnd.Next(recipes.Length);
            Recipe = recipes[i];
            transform.Find("RecipesMenu").gameObject.SetActive(false);
            StartPlay();
        } else {
            Debug.Log("No recipes");
        }
    }

    void Resume() {
        transform.Find("PauseMenu").gameObject.SetActive(false);
        if (!HUD) {
            Debug.LogError("HUD not found");
            return;
        }
        HUD.SetActive(true);
        Pause = false;
        Interactable = true;
    }

    void Change() {
        transform.Find("PauseMenu").gameObject.SetActive(false);
        transform.Find("RecipesMenu").gameObject.SetActive(true);
        if (!HUD) {
            Debug.LogError("HUD not found");
            return;
        }
    }

    void Exit() {
        transform.Find("PauseMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
        if (cam1 && cam2) {
            cam1.enabled = true;
            cam2.enabled = false;
        }
        Restart();
    }

    private string getElement(string element) {
        return transform.Find("MainMenu").Find("Panel").Find(element).GetComponent<TMP_InputField>().text.ToString().Trim();
    }

    RecipeStruct[] ParseXML(TextAsset xmlTextAsset) {
        return ParseXML(xmlTextAsset.text);
    }

    RecipeStruct[] ParseXML(string xml) {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNodeList list = doc.GetElementsByTagName("recipe");
        List<RecipeStruct> recs = new List<RecipeStruct>();
        RecipeStruct rec = new RecipeStruct();
        foreach (XmlNode element in list) {
            if (element.Attributes["title"] != null && element.Attributes["place"] != null) {
                rec.title = element.Attributes["title"] != null ? element.Attributes["title"].Value : "";
                rec.place = element.Attributes["place"] != null ? element.Attributes["place"].Value : "";
                rec.prefab = element.Attributes["pot"] != null ? element.Attributes["pot"].Value : "";
                if (element.Attributes["cooker"] != null && Convert.ToInt32(element.Attributes["cooker"].Value) >= 1 && Convert.ToInt32(element.Attributes["cooker"].Value) <= 4) {
                    rec.cooker = Convert.ToInt32(element.Attributes["cooker"].Value);
                } else {
                    rec.cooker = 1;
                }
                if (rec.title.Length > 0 && rec.place.Length > 0 && (rec.prefab.Length == 0 || ExistsPrefab(rec.prefab, ""))) {
                    List<ActionStruct> acts = new List<ActionStruct>();
                    ActionStruct act = new ActionStruct();
                    bool found = false;
                    foreach (XmlNode child in element.ChildNodes) {
                        act.name = child.InnerText;
                        act.type = child.Attributes["type"] != null ? child.Attributes["type"].Value : "";
                        act.target = child.Attributes["target"] != null ? child.Attributes["target"].Value : "";
                        act.prefab = child.Attributes["prefab"] != null ? child.Attributes["prefab"].Value : "";
                        act.temp = child.Attributes["for"] != null ? Convert.ToInt32(child.Attributes["for"].Value) : 0;
                        found = act.name.Length > 0 && act.type.Length > 0 && 
                            (act.target.Length > 0 || (act.type=="wait" && act.temp>0)) && 
                            (act.prefab.Length == 0 || ExistsPrefab(act.prefab, "Food/"));
                        if (!found) {
                            break;
                        }
                        if (act.type.ToLower().Equals("action") && child.Attributes["on"] != null) {
                            act.on = child.Attributes["on"].Value.ToLower().Equals("true") ? true : false;
                        }
                        act.duration = 0;
                        act.done = false;
                        acts.Add(act);
                        rec.actions = acts.ToArray();
                    }
                    if (found) {
                        recs.Add(rec);
                    }
                }
            }
        }
        return recs.ToArray();
    }

    public bool ExistsPrefab(string name, string path) {
        return Resources.Load("Prefabs/" + path + name, typeof(GameObject)) != null;
    }

    private void CreateRecipeBtns() {
        if (!recipeBtnPrefab) {
            Debug.LogError("recipeBtnPrefab not found");
            return;
        }
        GameObject container = transform.Find("RecipesMenu").Find("Panel").gameObject;
        int row = 0;
        int col = 0;
        for (int i = 0; i < recipes.Length && i < 12; i++) {    //max 12 recipes
            string title = recipes[i].title;
            RecipeStruct rec = recipes[i];
            GameObject recipeBtn = Instantiate(recipeBtnPrefab, container.transform);
            recipeBtn.name = title + "Btn";
            recipeBtn.GetComponentInChildren<TextMeshProUGUI>().text = title;
            recipeBtn.transform.localPosition += new Vector3(215 * col, - 50 * row);
            recipeBtn.GetComponent<Button>().onClick.AddListener(() => {
                Restart();
                Recipe = rec;
                transform.Find("RecipesMenu").gameObject.SetActive(false);
                StartPlay();
            });
            if (i == 3 || i == 7) {
                row = 0;
                col++;
            } else {
                row++;
            }
        }
    }

    void StartPlay() {
        if (!HUD) {
            Debug.LogError("HUD not found");
            return;
        }
        init = DateTime.Now;
        HUD.SetActive(true);
        Pause = false;
        Interactable = true;
        HUD.GetComponent<HUD>().ShowRecipe();
        if (cam1 && cam2) {
            cam2.enabled = true;
            cam1.enabled = false;
        }
        if (!kitchen) {
            Debug.LogError("Kitchen not found");
            return;
        }
        kitchen.GetComponent<Kitchen>().StopFlame();
        kitchen.GetComponent<Kitchen>().StartRecipe(Recipe);
    }

    void Restart() {
        Pause = true;
        Interactable = false;
        HUD.GetComponent<HUD>().Restart();
        kitchen.GetComponent<Kitchen>().StopFlame();
        kitchen.GetComponent<Kitchen>().ResetPot();
        kitchen.GetComponent<Kitchen>().Restart();
        Kitchen.actualAction = -1;
        recipes = ParseXML(recipesXML);
        if (recipes.Length > 0) {
            CreateRecipeBtns();
        }
    }

    public void Finish() {
        if (HUD) {
            HUD.SetActive(false);
        }
        if (cam1 && cam2) {
            cam2.enabled = false;
            cam1.enabled = true;
        }
        kitchen.GetComponent<Kitchen>().StopFlame();
        kitchen.GetComponent<Kitchen>().ResetPot();
        kitchen.GetComponent<Kitchen>().Restart();
        transform.Find("FinishMenu").gameObject.SetActive(true);
        ShowResult();
    }

    void ShowResult() {
        Transform container = transform.Find("FinishMenu").Find("Panel").Find("Scroll View").Find("Viewport").Find("Content");
        container.Find("UsernameLbl").GetComponent<Text>().text = Name;
        container.Find("SexLbl").GetComponent<Text>().text = Sex;
        container.Find("AgeLbl").GetComponent<Text>().text = Age;
        container.Find("DiseaseLbl").GetComponent<Text>().text = Disease;
        container.Find("TimeLbl").GetComponent<Text>().text = kitchen.GetComponent<Kitchen>().time.ToString("0.00") + "s";
        if (HUD) {
            container.Find("HintLbl").GetComponent<Text>().text = HUD.GetComponent<HUD>().HintCount.ToString();
            container.Find("RecipeViewsLbl").GetComponent<Text>().text = HUD.GetComponent<HUD>().ShowRecipeCount.ToString();
            container.Find("TPLbl").GetComponent<Text>().text = HUD.GetComponent<HUD>().TP.ToString();
            container.Find("FPLbl").GetComponent<Text>().text = HUD.GetComponent<HUD>().FP.ToString();
            float precision = (float)HUD.GetComponent<HUD>().TP / (float)(HUD.GetComponent<HUD>().TP + HUD.GetComponent<HUD>().FP);
            container.Find("PrecisionLbl").GetComponent<Text>().text = precision.ToString("0.00");
        }
    }

    void Home() {
        Restart();
        transform.Find("FinishMenu").gameObject.SetActive(false);
        transform.Find("MainMenu").gameObject.SetActive(true);
    }

    void Save() {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "log.csv"))) {
            File.AppendAllText(Path.Combine(Application.persistentDataPath, "log.csv"), "date,name,sex,age,disease,annotation,recipe,duration,hint," +
                    "recipe_views,global_tp,global_fp,global_precision,action,time,fp,precision" + '\n');
        }
        string line = init.ToString() + "," + Name + "," + Sex + "," + Age + "," + Disease + "," + Annotation + "," + Recipe.title + ",";
        line += kitchen.GetComponent<Kitchen>().time.ToString("0.00") + ",";
        if (HUD) {
            line += HUD.GetComponent<HUD>().HintCount.ToString() + ",";
            line += HUD.GetComponent<HUD>().ShowRecipeCount.ToString() + ",";
            line += HUD.GetComponent<HUD>().TP.ToString() + ",";
            line += HUD.GetComponent<HUD>().FP.ToString() + ",";
            float precision = (float)HUD.GetComponent<HUD>().TP / (float)(HUD.GetComponent<HUD>().TP + HUD.GetComponent<HUD>().FP);
            line += precision.ToString("0.00") + ",";
        }
        for (int i = 0; i < Recipe.actions.Length; i++) {
            string newLine = line;
            newLine += i.ToString() + ",";
            newLine += Recipe.actions[i].duration.ToString("0.00") + "s,";
            newLine += Recipe.actions[i].fp.ToString() + ",";
            newLine += ((float)1 / (float)(1 + Recipe.actions[i].fp)).ToString("0.00") + '\n';
            File.AppendAllText(Path.Combine(Application.persistentDataPath, "log.csv"), newLine);
        }
    }

}
