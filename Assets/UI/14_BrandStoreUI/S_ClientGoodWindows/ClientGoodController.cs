using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ClientGoodController : MonoBehaviour
{
    [SerializeField] private Button b_OpenGood;

    [Header("Card Visual")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text good_name;
    [SerializeField] private TMP_Text brand_name;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text create_date;

    [Header("main good info windows")]
    [SerializeField] private GameObject mainGoodInfoWindows;

    private BrandStore_GoodInfo cardInfo;
    private Texture2D good_Texture;

    private PlayerSessionData player_data = new PlayerSessionData();
    private BrandStore_PurchasedGoodInfo clientGoodInfo;

    public void Init(BrandStore_GoodInfo _cardInfo, PlayerSessionData data, BrandStore_PurchasedGoodInfo _clientGoodInfo) {
        cardInfo = _cardInfo;
        player_data = data;
        clientGoodInfo = _clientGoodInfo;

        StartCoroutine(SetInfo()); //2022 - 08 - 05T12: 15:38.000000Z

        b_OpenGood.onClick.AddListener(() => CreateMainCard());
    }

    private IEnumerator SetInfo() {
        good_name.text = cardInfo.title;
        brand_name.text = cardInfo.brand;
        price.text = clientGoodInfo.price.ToString();
        System.DateTime t = System.DateTime.Parse(clientGoodInfo.created_at);
        create_date.text = t.ToString("dd.MM.yyyy");

        yield return new WaitForEndOfFrame();

        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(WebData.Domain + cardInfo.image)) {
            yield return req.SendWebRequest();
            if (req.isNetworkError || req.isHttpError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(req.error, "GoodInfo");
                req.Dispose();
                yield break;
            }
            Debug.LogError("123");
            good_Texture = DownloadHandlerTexture.GetContent(req);
            req.Dispose();
        }

        cardImage.sprite = Sprite.Create(good_Texture, new Rect(0, 0, 256, 256), new Vector2(256 / 2, 256 / 2));
    }

    private void CreateMainCard() {
        ClientGoodMainInfo go = Instantiate(mainGoodInfoWindows, GetComponentInParent<BrandStoreSystem>().transform).GetComponent<ClientGoodMainInfo>();
        go.Init(cardInfo, player_data, clientGoodInfo);
    }
}
