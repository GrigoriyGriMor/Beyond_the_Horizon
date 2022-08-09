using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class ClientGoodsWindows : MonoBehaviour
{
    [SerializeField] private GameObject GoodCard;

    [Header("Title block")]
    [SerializeField] private TMP_InputField SearchLine;
    [SerializeField] private Button B_Search;

    [SerializeField] private TMP_Dropdown sortFillter;

    [Header("Main block")]
    [SerializeField] private RectTransform mainContent;

    [Header("Bottom block")]
    [SerializeField] private TMP_InputField pageNumber;
    [SerializeField] private TMP_Text maxPageCount;

    [SerializeField] private Button b_NextPage;
    [SerializeField] private Button b_BackPage;
    [SerializeField] private Button b_LastPage;
    [SerializeField] private Button b_FirstPage;

    private int currentPage = 1;

    private PlayerSessionData player_data = new PlayerSessionData();

    private bool loaded = false;

    [Header("Loading visual")]
    [SerializeField] private GameObject loadingVisual;

    public void Init(PlayerSessionData data) {
        player_data = data;
    }

    private void Start() {
        StartCoroutine(SetParam());
    }

    private IEnumerator SetParam() {
        yield return new WaitForFixedUpdate();

        B_Search.onClick.AddListener(() => UpdatePage());
        sortFillter.onValueChanged.AddListener((value) => UpdatePage());

        currentPage = 1;

        b_BackPage.interactable = false;
        b_FirstPage.interactable = false;

        b_NextPage.interactable = true;
        b_LastPage.interactable = true;

        pageNumber.text = currentPage.ToString();

        b_NextPage.onClick.AddListener(() => {
            if (currentPage + 1 <= int.Parse(maxPageCount.text)) {
                currentPage += 1;
                pageNumber.text = currentPage.ToString();
                pageNumber.onEndEdit.Invoke(pageNumber.text);

                if (currentPage == int.Parse(maxPageCount.text)) {
                    b_NextPage.interactable = false;
                    b_LastPage.interactable = false;
                }

                if (currentPage == 2) {
                    b_BackPage.interactable = true;
                    b_FirstPage.interactable = true;
                }
            }
        });

        b_BackPage.onClick.AddListener(() => {
            if (currentPage - 1 >= 1) {
                currentPage -= 1;
                pageNumber.text = currentPage.ToString();
                pageNumber.onEndEdit.Invoke(pageNumber.text);

                if (currentPage == int.Parse(maxPageCount.text) - 1) {
                    b_NextPage.interactable = true;
                    b_LastPage.interactable = true;
                }

                if (currentPage == 1) {
                    b_BackPage.interactable = false;
                    b_FirstPage.interactable = false;
                }
            }
        });

        b_LastPage.onClick.AddListener(() => {
            if (currentPage == 1) {
                b_BackPage.interactable = true;
                b_FirstPage.interactable = true;
            }

            currentPage = int.Parse(maxPageCount.text);
            pageNumber.text = currentPage.ToString();
            pageNumber.onEndEdit.Invoke(pageNumber.text);

            b_NextPage.interactable = false;
            b_LastPage.interactable = false;
        });

        b_FirstPage.onClick.AddListener(() => {
            if (currentPage == int.Parse(maxPageCount.text)) {
                b_LastPage.interactable = true;
                b_NextPage.interactable = true;
            }

            currentPage = 1;
            pageNumber.text = currentPage.ToString();
            pageNumber.onEndEdit.Invoke(pageNumber.text);

            b_BackPage.interactable = false;
            b_FirstPage.interactable = false;
        });

        pageNumber.onEndEdit.AddListener((value) => {
            if (int.Parse(pageNumber.text) > int.Parse(maxPageCount.text))
                pageNumber.text = maxPageCount.text;

            if (int.Parse(pageNumber.text) < 1)
                pageNumber.text = "1";

            UpdatePage();

            if (currentPage != int.Parse(pageNumber.text)) {
                currentPage = int.Parse(pageNumber.text);

                if (currentPage == int.Parse(maxPageCount.text)) {
                    b_LastPage.interactable = false;
                    b_NextPage.interactable = false;

                    b_BackPage.interactable = true;
                    b_FirstPage.interactable = true;
                }

                if (currentPage == 1) {
                    b_BackPage.interactable = false;
                    b_FirstPage.interactable = false;

                    b_NextPage.interactable = true;
                    b_LastPage.interactable = true;
                }

                if (currentPage > 1 && currentPage < int.Parse(maxPageCount.text)) {
                    b_BackPage.interactable = true;
                    b_FirstPage.interactable = true;

                    b_NextPage.interactable = true;
                    b_LastPage.interactable = true;
                }
            }
        });

        loaded = true;

        UpdatePage();
    }

    private void OnDisable() {
        ClearContent();
    }

    private void OnEnable() {
        if (loaded) RefreshAllPage();
    }

    private void ClearContent() {
        for (int i = 0; i < mainContent.childCount; i++)
            Destroy(mainContent.GetChild(i).gameObject);
    }

    private void RefreshAllPage() {
        currentPage = 1;
        SearchLine.text = "";
        pageNumber.text = currentPage.ToString();

        b_BackPage.interactable = false;
        b_FirstPage.interactable = false;

        b_NextPage.interactable = true;
        b_LastPage.interactable = true;

        UpdatePage();
    }

    Coroutine updatePageCoroutine;
    private void UpdatePage() {
        if (updatePageCoroutine != null) return;

        loadingVisual.SetActive(true);

        for (int i = 0; i < mainContent.childCount; i++)
            Destroy(mainContent.GetChild(i).gameObject);

        updatePageCoroutine = StartCoroutine(RequestUpdatePage());
    }

    private IEnumerator RequestUpdatePage() {
        string request_param = "";

        request_param += $"?page={pageNumber.text}";
        request_param += $"&order={sortFillter.value}";

        if (SearchLine.text != "")
            request_param += $"&name={SearchLine.text}";

        using (UnityWebRequest www = UnityWebRequest.Get(WebData.ByeGood + request_param)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                Debug.LogError("brand main page request " + www.error);
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                Debug.LogError("www.downloadHandler.text.Length < 1");
                www.Dispose();
                yield break;
            }

            BrandStore_PurchasedGoodSearchData data = JsonUtility.FromJson<BrandStore_PurchasedGoodSearchData>(www.downloadHandler.text);

            maxPageCount.text = data.meta.last_page.ToString();

            string id_list = "?list=";
            for (int i = 0; i < data.data.Length; i++)
                id_list += $"{data.data[i].object_id},";

            using (UnityWebRequest req = UnityWebRequest.Get(WebData.Good_request + id_list)) {
                req.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
                yield return req.SendWebRequest();

                if (req.isHttpError || req.isNetworkError) {
                    Debug.LogError("brand main page request " + req.error);
                    req.Dispose();
                    yield break;
                }

                if (req.downloadHandler.text.Length < 1) {
                    Debug.LogError("www.downloadHandler.text.Length < 1");
                    req.Dispose();
                    yield break;
                }

                BrandStore_MainWindowsSearchData data_1 = JsonUtility.FromJson<BrandStore_MainWindowsSearchData>(req.downloadHandler.text);

                for (int i = 0; i < data_1.data.Length; i++) {
                    for (int j = 0; j < data.data.Length; j++) {
                        if (data.data[j].object_id == data_1.data[i].id) {
                            ClientGoodController go = Instantiate(GoodCard, mainContent).GetComponent<ClientGoodController>();
                            go.Init(data_1.data[i], player_data, data.data[j]);
                        }
                    }
                }
                req.Dispose();
            }
            www.Dispose();
        }

        loadingVisual.SetActive(false);
        updatePageCoroutine = null;
    }
}


[System.Serializable]
public class BrandStore_PurchasedGoodSearchData {
    public BrandStore_PurchasedGoodInfo[] data;
    public BrandStore_MainWindowsSearchMetaData meta;
}

[System.Serializable]
public class BrandStore_PurchasedGoodInfo {
    public int id;
    public int object_id;
    public int price;
    public string created_at;

    public BrandStore_GoodInfo goodInfo;
}