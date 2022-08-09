using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GoodMainInfo : MonoBehaviour
{
    [SerializeField] private Button b_ByeGood;

    [SerializeField] private Button[] b_Close;

    [Header("Card Visual")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text good_name;
    [SerializeField] private TMP_Text brand_name;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text category;
    [SerializeField] private TMP_Text discription;

    [SerializeField] private TMP_Text player_coins;

    [Header("ParamContent")]
    [SerializeField] private RectTransform paramContent;
    [SerializeField] private GameObject ParamPanel;

    private BrandStore_GoodInfo cardInfo;
    private Texture2D good_Texture;

    [Header("Bye accept panel")]
    [SerializeField] private GameObject ByeAcceptPanel;
    [SerializeField] private Button b_ByeAccept;
    [SerializeField] private TMP_Text endPrice;
    [SerializeField] private TMP_Text playerHaveCoins;
    [SerializeField] private Button[] b_ByeAcceptClose;

    [Header("info message panel")]
    //на подтверждение мы просто привязываем кнопку ClosePanel, тем самым возвращая пользователя на главную страницу не обновляя ее
    [SerializeField] private GameObject successMessagePanel;
    [SerializeField] private GameObject failMessagePanel;
    [SerializeField] private Button b_backAfterTraidFail;

    [Header("Bye Accept Panel")]
    [SerializeField] private Button b_EndTransaction;

    private PlayerSessionData player_data = new PlayerSessionData();

    public void Init(BrandStore_GoodInfo _cardInfo, PlayerSessionData data) {
        cardInfo = _cardInfo;
        player_data = data;

        StartCoroutine(SetInfo());

        b_ByeGood.onClick.AddListener(() => ByeGood());
        for (int i = 0; i < b_Close.Length; i++)
            b_Close[i].onClick.AddListener(() => ClosePanel());

        for (int i = 0; i < b_ByeAcceptClose.Length; i++)
            b_ByeAcceptClose[i].onClick.AddListener(() => ByeAcceptPanel.SetActive(false));

        b_ByeAccept.onClick.AddListener(() => {
            StartCoroutine(Bye());
            b_ByeAccept.interactable = false;
        });

        b_backAfterTraidFail.onClick.AddListener(() => {
            failMessagePanel.SetActive(false);
            ByeAcceptPanel.SetActive(false);
        });

        b_EndTransaction.onClick.AddListener(() => {
            FindObjectOfType<MainWindows>().UpdatePage();
            ClosePanel();
        });
    }
    private IEnumerator SetInfo() {

        good_name.text = cardInfo.title;
        brand_name.text = cardInfo.brand;
        category.text = cardInfo.object_category;
        discription.text = cardInfo.discription;
        price.text = cardInfo.price.ToString();

        for (int i = 0; i < cardInfo.props.Length; i++) {
            GoodParamInfo go = Instantiate(ParamPanel, paramContent).GetComponent<GoodParamInfo>();
            go.Init(cardInfo.props[i].name.title, cardInfo.props[i].data);
        }

        using (UnityWebRequest www = UnityWebRequest.Get(WebData.CashRequest + $"?x-session={player_data.player_session}")) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            player_coins.text = www.downloadHandler.text.ToString();
            playerHaveCoins.text = www.downloadHandler.text.ToString();
        }

        yield return new WaitForEndOfFrame();
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(WebData.Domain + cardInfo.image)) {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(req.error, "GoodInfo");
                req.Dispose();
                yield break;
            }

            good_Texture = DownloadHandlerTexture.GetContent(req);
            req.Dispose();
        }

        cardImage.sprite = Sprite.Create(good_Texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }

    private void ByeGood() {
        ByeAcceptPanel.SetActive(true);
        endPrice.text = cardInfo.price.ToString();
        playerHaveCoins.text = player_coins.text;
    }

    private IEnumerator Bye() {
        WWWForm form = new WWWForm();
        form.AddField("object_id", cardInfo.id);
        form.AddField("x-session", player_data.player_session);

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.ByeGood, form)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                Debug.LogError("brand main page request " + www.error);
                www.Dispose(); 
                failMessagePanel.SetActive(true);
                b_ByeAccept.interactable = true;
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                Debug.LogError("www.downloadHandler.text.Length < 1");
                www.Dispose();
                failMessagePanel.SetActive(true);
                b_ByeAccept.interactable = true;
                yield break;
            }

            Debug.LogError(www.downloadHandler.text);
            BrandStore_TraidResult data = JsonUtility.FromJson<BrandStore_TraidResult>(www.downloadHandler.text);

            if (data.data.error != null && data.data.error.Length > 0) {
                Debug.LogError("some error:" + data.data.error);
                failMessagePanel.SetActive(true);
                www.Dispose();
                b_ByeAccept.interactable = true;
                yield break;
            }

            successMessagePanel.SetActive(true);

            www.Dispose();
        }
    }

    private void ClosePanel() {
        Destroy(gameObject);
    }
}
