using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BilboardController : AbstractIO
{
    [Header("BildBoard ID")]
    [SerializeField] private uint ID = 1;

    private List<BillboardInGameInfo> textures = new List<BillboardInGameInfo>();
    [Header("Image places")]
    [SerializeField] private MeshRenderer[] boards = new MeshRenderer[0];

    private BillboardInGameInfo currentInfo = new BillboardInGameInfo();

    private void Start() {
        StartCoroutine(ReuestImage());
    }

    private IEnumerator ReuestImage() {
        using (UnityWebRequest www = UnityWebRequest.Get(WebData.BillboardInfo + $"{ID}")) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "BilboardController");
                Debug.LogError("BilboardController " + www.error);
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "BilboardController ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                www.Dispose();
                yield break;
            }

            BillboardInfo data = JsonUtility.FromJson<BillboardInfo>(www.downloadHandler.text);

            for (int i = 0; i < data.data.images.Length; i++) {
                BillboardInGameInfo newData = new BillboardInGameInfo();
                if (data.data.images[i].brand_name != "") newData.brand_name = data.data.images[i].brand_name;
                if (data.data.images[i].good_id != "") newData.good_id = data.data.images[i].good_id;

                using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(WebData.Domain + data.data.images[i].url)) {
                    yield return req.SendWebRequest();
                    if (req.isNetworkError || req.isHttpError) {
                        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(req.error, "BilboardController");
                        req.Dispose();
                        yield break;
                    }

                    newData.texture = DownloadHandlerTexture.GetContent(req);

                    textures.Add(newData);

                    req.Dispose();
                }
            }

            StartCoroutine(PresentBillboardImage());
            www.Dispose();
        }
    }

    [SerializeField] private float timeForNextImage = 2;
    private IEnumerator PresentBillboardImage() {
        for (int i = 0; i < textures.Count; i++) {
            for (int j = 0; j < boards.Length; j++) 
                boards[j].materials[0].mainTexture = textures[i].texture;

            currentInfo = textures[i];

            yield return new WaitForSeconds(timeForNextImage);
        }

        StartCoroutine(PresentBillboardImage());
    }

    public BillboardInGameInfo GetInfo() {
        return currentInfo;
    }
}

[System.Serializable]
public class BillboardInfo {
    public BillboardData data;
}

[System.Serializable]
public class BillboardData {
    public int id;
    public string description;
    public Vector3 location;

    public BillboardImage[] images;
}

[System.Serializable]
public class BillboardImage {
    public int id;
    public string description;
    public string url;
    public string brand_name;
    public string good_id;
}

[System.Serializable]
public class BillboardInGameInfo {
    public string description;
    public Texture texture;
    public string brand_name;
    public string good_id;
}