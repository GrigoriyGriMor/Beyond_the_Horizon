using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GoodInfoController : MonoBehaviour
{
    [SerializeField] private Button b_OpenGood;

    [Header("Card Visual")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text good_name;
    [SerializeField] private TMP_Text brand_name;
    [SerializeField] private TMP_Text price;

    [Header("main good info windows")]
    [SerializeField] private GameObject mainGoodInfoWindows;

    private BrandStore_GoodInfo cardInfo;
    private Texture2D good_Texture;

    private PlayerSessionData player_data = new PlayerSessionData();

    public void Init(BrandStore_GoodInfo _cardInfo, PlayerSessionData data) {
        cardInfo = _cardInfo;
        player_data = data;

        StartCoroutine(SetInfo());

        b_OpenGood.onClick.AddListener(() => CreateMainCard());
    }

    private IEnumerator SetInfo() {
        good_name.text = cardInfo.title;
        brand_name.text = cardInfo.brand;
        price.text = cardInfo.price.ToString();

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

         cardImage.sprite = Sprite.Create(good_Texture, new Rect(0, 0, 256, 256), new Vector2(256 / 2, 256 / 2));
    }

    private void CreateMainCard() {
        GoodMainInfo go = Instantiate(mainGoodInfoWindows, GetComponentInParent<BrandStoreSystem>().transform).GetComponent<GoodMainInfo>();
        go.Init(cardInfo, player_data);
    }
}
