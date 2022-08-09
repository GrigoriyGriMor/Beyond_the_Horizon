using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MainWindows : MonoBehaviour
{
    [SerializeField] private GameObject GoodCard;

    [Header("Title block")]
    [SerializeField] private TMP_Text mainCoins;
    [SerializeField] private TMP_InputField SearchLine;
    [SerializeField] private Button B_Search;

    [SerializeField] private TMP_Dropdown sortFillter;

    [Header("Fillters Brands")]
    [SerializeField] private Button B_OpenBrandSortFillter;
    [SerializeField] private Animator brandAnim;
    [SerializeField] private GameObject BrandSelectToggle;
    [SerializeField] private RectTransform brandContent;
    [SerializeField] private Button B_BrandContinue;
    [SerializeField] private Button[] brandCancelButton = new Button[2];
    private List<BrandSelectToggle> brandList = new List<BrandSelectToggle>();


    [Header("Fillters Category")]
    [SerializeField] private Button B_OpenCategorySortFillter;
    [SerializeField] private Animator categoryAnim;
    [SerializeField] private GameObject CategorySelectToggle;
    [SerializeField] private RectTransform categoryContent;
    [SerializeField] private ToggleGroup contentToggleGroup;
    [SerializeField] private Button B_CategoryContinue;
    [SerializeField] private Button[] categoryCancelButton = new Button[2];
    private List<BrandSelectToggle> categoryList = new List<BrandSelectToggle>();

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
        StartCoroutine(RequestMainPageData());
    }

    private IEnumerator RequestMainPageData() {
        using (UnityWebRequest www = UnityWebRequest.Get(WebData.Brand_Cat)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "LogInController");
                Debug.LogError("brand main page request " + www.error);
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "LogInController ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                www.Dispose();
                yield break;
            }

            BrandStore_MainWindowsAllData data = JsonUtility.FromJson<BrandStore_MainWindowsAllData>(www.downloadHandler.text);

            for (int i = 0; i < data.data.brands.Length; i++) {
                BrandSelectToggle go = Instantiate(BrandSelectToggle, brandContent).GetComponent<BrandSelectToggle>();
                go.Init(data.data.brands[i].title, data.data.brands[i].title);
                brandList.Add(go);
            }

            int count = (data.data.categories.Length <= 20) ? data.data.categories.Length : 20;
            for (int i = 0; i < count; i++) {
                BrandSelectToggle go = Instantiate(CategorySelectToggle, categoryContent).GetComponent<BrandSelectToggle>();
                if (contentToggleGroup != null)
                    go.Init(data.data.categories[i].title, data.data.categories[i].id, contentToggleGroup);
                else
                    go.Init(data.data.categories[i].title, data.data.categories[i].id);

                categoryList.Add(go);
            }

            www.Dispose();
        }
     
        yield return new WaitForFixedUpdate();

        B_Search.onClick.AddListener(() => UpdatePage());

        sortFillter.onValueChanged.AddListener((value) => UpdatePage());

        B_BrandContinue.onClick.AddListener(() => {
            StartCoroutine(CloseBlock(brandAnim));
            UpdatePage();
        });
        B_CategoryContinue.onClick.AddListener(() => {
            StartCoroutine(CloseBlock(categoryAnim));
            UpdatePage();
        });

        B_OpenBrandSortFillter.onClick.AddListener(() => {
            brandAnim.gameObject.SetActive(true);

            for (int b = 0; b < brandCancelButton.Length; b++) {
                int number = b;//так сделано, так как почему-то при подписи объекта на итератор он стакает до крайнего в списке и подпись срабатывает только на крайний элемент
                brandCancelButton[number].interactable = true;
            }
        });

        B_OpenCategorySortFillter.onClick.AddListener(() => {
            categoryAnim.gameObject.SetActive(true);

            for (int b = 0; b < categoryCancelButton.Length; b++) {
                int number = b;
                categoryCancelButton[number].interactable = true;
            }
        });

        for (int b = 0; b < brandCancelButton.Length; b++)
            brandCancelButton[b].onClick.AddListener(() => {
                StartCoroutine(CloseBlock(brandAnim));

                for (int _b = 0; _b < brandCancelButton.Length; _b++) {
                    int number = _b;
                    brandCancelButton[number].interactable = false;
                }
            });

        for (int c = 0; c < categoryCancelButton.Length; c++)
            categoryCancelButton[c].onClick.AddListener(() => {
                StartCoroutine(CloseBlock(categoryAnim));

                for (int _c = 0; _c < categoryCancelButton.Length; _c++) {
                    int number = _c;
                    categoryCancelButton[number].interactable = false;
                }
            });

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

    private IEnumerator CloseBlock(Animator block) {
        block.SetTrigger("CloseFillter");

        yield return new WaitForSeconds(1f);
        block.gameObject.SetActive(false);
    }

    public void SetFillterParam(string brand_name, int good_id = -1) {
        if (brand_name != "")
            for (int i = 0; i < brandList.Count; i++)
                if (brandList[i].GetTglTag() == brand_name)
                    brandList[i].GetComponent<Toggle>().isOn = true;

        if (good_id != -1)
            selectGoodIndex = good_id;

        UpdatePage();
    }

    int selectGoodIndex = -1;
    Coroutine updatePageCoroutine;
    public void UpdatePage() {
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

        for (int i = 0; i < categoryList.Count; i++)
            if (categoryList[i].GetStatus()) {
                request_param += $"&category={categoryList[i].GetTglIntTag()}";
                break;
            }

        for (int j = 0; j < brandList.Count; j++) {
            if (brandList[j].GetStatus()) {
                request_param += $"&brand={brandList[j].GetTglTag()}";
                break;
            }
        }

        //важно, если select good index будет выше нуля, то ранее указанные данные фильтра обнуляться 
        if (selectGoodIndex != -1)
            request_param = $"?list={selectGoodIndex}";

        using (UnityWebRequest www = UnityWebRequest.Get(WebData.Good_request + request_param)) {
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

            //Debug.LogError(www.downloadHandler.text);
            BrandStore_MainWindowsSearchData data = JsonUtility.FromJson<BrandStore_MainWindowsSearchData>(www.downloadHandler.text);

            maxPageCount.text = data.meta.last_page.ToString();

            for (int i = 0; i < data.data.Length; i++) {
                GoodInfoController go = Instantiate(GoodCard, mainContent).GetComponent<GoodInfoController>();
                go.Init(data.data[i], player_data);
            }

            www.Dispose();
        }

        using (UnityWebRequest www = UnityWebRequest.Get(WebData.CashRequest + $"?x-session={player_data.player_session}")) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            mainCoins.text = "COINS: " + www.downloadHandler.text.ToString();
        }

        selectGoodIndex = -1;
        loadingVisual.SetActive(false);
        updatePageCoroutine = null;
    }

    private void OnDisable() {
        ClearContent();
    }

    private void OnEnable() {
        if (loaded) RefreshAllPage();
    }

    private void ClearContent() { 
        for(int i = 0; i < mainContent.childCount; i++)
            Destroy(mainContent.GetChild(i).gameObject);
    }

    private void RefreshAllPage() {
        selectGoodIndex = -1;
        currentPage = 1;
        SearchLine.text = "";

        for (int j = 0; j < brandList.Count; j++)
            brandList[j].OffToggle();

        for (int j = 0; j < categoryList.Count; j++)
            categoryList[j].OffToggle();

        pageNumber.text = currentPage.ToString();

        b_BackPage.interactable = false;
        b_FirstPage.interactable = false;

        b_NextPage.interactable = true;
        b_LastPage.interactable = true;

        UpdatePage();
    }
}

[System.Serializable]
public class BrandStore_TraidResult {
    public BrandStore_TraidData data;
}

[System.Serializable]
public class BrandStore_TraidData {
    public string error;
    public int id;
}

[System.Serializable]
public class BrandStore_MainWindowsAllData {
    public string error;

    public BrandStore_MainWindowsData data;
}

[System.Serializable]
public class BrandStore_MainWindowsData {
    public int client_coins;
    public int page_count;

    public BrandStore_FillterToggle[] brands;
    public BrandStore_FillterToggle[] categories;
}

[System.Serializable]
public class BrandStore_FillterToggle {
    public string title;
    public int id;
}

[System.Serializable]
public class BrandStore_MainWindowsSearchData {
    public BrandStore_GoodInfo[] data;
    public BrandStore_MainWindowsSearchMetaData meta;
}

[System.Serializable]
public class BrandStore_MainWindowsSearchMetaData {
    public int last_page;
}

[System.Serializable]
public class BrandStore_GoodInfo {
    public int id;
    public string title;
    public string image;
    public string brand;
    public int price;
    public string object_category;
    public string discription;

    public BrandStore_GoodParametrs[] props;
}

[System.Serializable]
public class BrandStore_GoodParametrs {
    public int id;
    public string data;

    public BrandStore_GoodParametrsName name;
}

[System.Serializable]
public class BrandStore_GoodParametrsName {
    public int id;
    public string title;
}