using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsoleScript : MonoBehaviour {
    private static ConsoleScript instance;
    public static ConsoleScript Instance => instance;

    [SerializeField] private GameObject Console;
    [SerializeField] private GameObject Content;
    [SerializeField] private List<TMP_Text> ConsoleText = new List<TMP_Text>();

    [SerializeField] private Button ClearConsole;

    [Header("FPS Target Frame")]
    [SerializeField] private int fpsTargetFrame = 144;

    void Awake() {
        instance = this;

        DontDestroyOnLoad(gameObject);

        ClearConsole.onClick.AddListener(() => CleareConsoleText());

        for (int i = 0; i < Content.transform.childCount; i++) {
            ConsoleText.Add(Content.transform.GetChild(i).GetComponent<TMP_Text>());
        }
    }

    private void Start() {
        if (fpsTargetFrame >= 30) Application.targetFrameRate = fpsTargetFrame;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.F1)) {
            if (Console.activeInHierarchy) {
                Console.SetActive(false);
            }
            else {
                Console.SetActive(true);
            }
        }

        //if (ConsoleText.text.Length > 10000)
        //    ConsoleText.text = ConsoleText.text.Remove(0, 5000);
    }

    private void OnEnable() {
        Application.logMessageReceivedThreaded += AddConsoleText;
    }

    private void OnDisable() {
        Application.logMessageReceivedThreaded -= AddConsoleText;
    }

    public void CleareConsoleText() {
        for (int i = 0; i < ConsoleText.Count; i++) 
            ConsoleText[i].text = "";

        ConsoleText[ConsoleText.Count - 1].text = "Console be clean";
    }

    int _i = 0;
    public void AddConsoleText(string error, string name = "Enter: ") {
        _i += 1;

        ConsoleText[0].text = $"\n{_i}) {name} {error}";
        ConsoleText[0].transform.SetAsLastSibling();

        TMP_Text t;
        for (int i = 1; i < ConsoleText.Count; i++) {
            t = ConsoleText[i - 1];
            ConsoleText[i - 1] = ConsoleText[i];
            ConsoleText[i] = t;
        }
    }

    public void AddConsoleText(string logString, string stackTrace, LogType type) {
        _i += 1;


        ConsoleText[0].text = $"\n{_i}) {type} {logString} \n {stackTrace}";
        ConsoleText[0].transform.SetAsLastSibling();

        TMP_Text t;
        for (int i = 1; i < ConsoleText.Count; i++) {
            t = ConsoleText[i - 1];
            ConsoleText[i - 1] = ConsoleText[i];
            ConsoleText[i] = t;
        }
    }
}
