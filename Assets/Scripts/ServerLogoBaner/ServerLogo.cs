using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLogo : MonoBehaviour
{
    [SerializeField]
    private DataLogo dataLogo1;

    [SerializeField]
    private DataBillboard dataBillboard;

    private string SaveLogo(DataLogo dataLogo)
    {
        Debug.Log(" Save data Quest");

        return JsonUtility.ToJson(dataLogo);
    }

    private DataLogo LoadLogo(string value)
    {
        Debug.Log(" Load data Quest");

        DataLogo dataLogo = new DataLogo();
        dataLogo = JsonUtility.FromJson<DataLogo>(value);
        return dataLogo;
    }



    // {"data":{"id":1,
    // "description":"\u0422\u0435\u0441\u0442\u043e\u0432\u044b\u0439 \u043b\u043e\u0433\u043e\u0442\u0438\u043f",
    // "url":"\/logo\/1.jpg"}}
    [System.Serializable]
    public class DataLogo
    {
        public int id;
        public string description;
        public string url;
    }

    //{"data":{"id":1,"description":"\u0422\u0435\u0441\u0442\u043e\u0432\u044b\u0439 \u0431\u0438\u043b\u043b\u0431\u043e\u0430\u0440\u0434",
    //"location":{"x":0,"y":0,"z":0},
    //"images":[{"id":1,"description":"\u0422\u0435\u0441\u0442\u043e\u0432\u043e\u0435 \u0438\u0437\u043e\u0431\u0440\u0430\u0436\u0435\u043d\u0438\u0435",
    //"url":"\/billboards\/1.jpg"}]}}

    [System.Serializable]
    public class DataBillboard
    {
        public int id;
        public string description;
        public Location location;
        public Images images;
    }

    [System.Serializable]
    public class Location
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class Images
    {
        public int id;
        public string description;
        public string url;
    }


}
