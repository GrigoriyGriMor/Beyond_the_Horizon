using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ClientGoodMainInfo : MonoBehaviour
{
    [SerializeField] private Button b_ResendGoodInfo;

    [SerializeField] private Button[] b_Close;

    [Header("Card Visual")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text good_name;
    [SerializeField] private TMP_Text brand_name;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text category;
    [SerializeField] private TMP_Text discription;
    [SerializeField] private TMP_Text create_date;

    [Header("ParamContent")]
    [SerializeField] private RectTransform paramContent;
    [SerializeField] private GameObject ParamPanel;

    private BrandStore_GoodInfo cardInfo;
    private Texture2D good_Texture;

    [Header("info message panel")]
    //на подтверждение мы просто привязываем кнопку ClosePanel, тем самым возвращая пользователя на главную страницу не обновляя ее
    [SerializeField] private GameObject successMessagePanel;
    [SerializeField] private Button b_backAfterResend;

    private PlayerSessionData player_data = new PlayerSessionData();
    private BrandStore_PurchasedGoodInfo clientGoodInfo;

    public void Init(BrandStore_GoodInfo _cardInfo, PlayerSessionData data, BrandStore_PurchasedGoodInfo _clientGoodInfo) {
        cardInfo = _cardInfo;
        player_data = data;
        clientGoodInfo = _clientGoodInfo;

        StartCoroutine(SetInfo());

        b_ResendGoodInfo.onClick.AddListener(() => ResendGoodInfo());

        for (int i = 0; i < b_Close.Length; i++)
            b_Close[i].onClick.AddListener(() => ClosePanel());

        b_backAfterResend.onClick.AddListener(() => successMessagePanel.SetActive(false));
    }

    private IEnumerator SetInfo() {

        good_name.text = cardInfo.title;
        brand_name.text = cardInfo.brand;
        category.text = cardInfo.object_category;
        discription.text = cardInfo.discription;
        price.text = clientGoodInfo.price.ToString();
        System.DateTime t = System.DateTime.Parse(clientGoodInfo.created_at);
        create_date.text = t.ToString("dd.MM.yyyy");

        for (int i = 0; i < cardInfo.props.Length; i++) {
            GoodParamInfo go = Instantiate(ParamPanel, paramContent).GetComponent<GoodParamInfo>();
            go.Init(cardInfo.props[i].name.title, cardInfo.props[i].data);
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

    private void ResendGoodInfo() {
        successMessagePanel.SetActive(true);
        StartCoroutine(ResendInfo());
    }

    private IEnumerator ResendInfo() {
        WWWForm form = new WWWForm();
        form.AddField("object_id", cardInfo.id);
        form.AddField("x-session", player_data.player_session);
        yield return new WaitForEndOfFrame();
       /* using (UnityWebRequest www = UnityWebRequest.Post(WebData.ByeGood, form)) {
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
        }*/
    }

    private void ClosePanel() {
        Destroy(gameObject);
    }
}
