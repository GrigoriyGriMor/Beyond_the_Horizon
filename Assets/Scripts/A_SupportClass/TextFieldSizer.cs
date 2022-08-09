using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldSizer : MonoBehaviour
{
    [Header("Панель подсказки")]
    [SerializeField] private Image answerUIPanel;
    [SerializeField] private Text answerText;

    [Header("Регулятор размерности подсказки")]
    [SerializeField] private int oneLetterParcer = 30;
    [SerializeField] private int maxLetterInOneLine = 20;
    private Vector2 panelStartSize;

    private string currentString = "";

    private void Start() {
        panelStartSize = answerUIPanel.rectTransform.sizeDelta;
        currentString = answerText.text;
    }

    private void FixedUpdate() {
        if (currentString != answerText.text) AnswerConnector(answerText.text);
    }

    private void AnswerConnector(string _text = "") {
        if (_text != "" && !answerUIPanel.gameObject.activeInHierarchy) {
            float sizeX = Mathf.Clamp(_text.Length, 5, maxLetterInOneLine) * oneLetterParcer;
            float sizeY = panelStartSize.y + ((Mathf.FloorToInt(_text.Length / maxLetterInOneLine) + 1) * oneLetterParcer);

            answerUIPanel.rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
            answerText.rectTransform.sizeDelta = new Vector2(sizeX - oneLetterParcer, sizeY);

            currentString = _text;
        }
    }
}
