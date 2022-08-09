using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private RectTransform contentMap;
    [SerializeField]
    private RectTransform markers;
    [SerializeField]
    private RectTransform pointPlayer;
    [SerializeField]
    private RectTransform pointMarker;
    [SerializeField]
    private TargetMiniMap markerMiniMap;
    [SerializeField]
    private TargetManager targetManager;
    [SerializeField]
    private Button buttonScalePlus;
    [SerializeField]
    private Button buttonScaleMinus;
    [SerializeField]
    private Button buttonGoToPlayer;
    [SerializeField]
    private TMP_Text setZoomText;
    [SerializeField]
    private Transform markerInScene;
    [SerializeField]
    private PointMission[] pointMissions;
    [SerializeField]
    private float terrainX = 1000;
    [SerializeField]
    private float terrainY = 1000;
    [SerializeField]
    private float originX = 0;
    [SerializeField]
    private float originZ = 0;
    //[SerializeField]
    private float imageWidth;
    //[SerializeField]
    private float imageHeigth;
    [SerializeField]
    private float stepZoomMap = 10;
    private float scaleZoomMap = 100;
    private float currentLerpZoom;
    private float ratioX;
    private float ratioY;
    Transform thisTransform;
    [SerializeField]
    private float timerLerpZoom = 5.0f;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        markerInScene = Instantiate(markerInScene, Vector3.zero, Quaternion.identity);
        markerInScene.gameObject.SetActive(false);
        if (contentMap)
        {
            imageWidth = contentMap.sizeDelta.x;
            imageHeigth = contentMap.sizeDelta.y;
        }

        thisTransform = transform;

        if (buttonScalePlus)
        {
            buttonScalePlus.onClick.AddListener(() => SetScaleZoomMap(stepZoomMap));
        }
        else
        {
            print("Not buttonScalePlus");
        }

        if (buttonScaleMinus)
        {
            buttonScaleMinus.onClick.AddListener(() => SetScaleZoomMap(-stepZoomMap));
        }
        else
        {
            print("Not buttonScaleMinus");
        }

        if (buttonGoToPlayer)
        {
            buttonGoToPlayer.onClick.AddListener(() => NavigationPlayer());

        }
        else
        {
            print("Not buttonGoToPlayer");
        }

        if (markerMiniMap && markerInScene)
        {
            markerMiniMap.SetTarget(markerInScene);
            markerMiniMap.SetDistanceVisibleTargetOnMiniMap(5.0f);
            markerMiniMap.gameObject.SetActive(false);

            if (pointMarker)
            {
                markerInScene.GetComponent<MarkerInScene>().SetPointMarker(pointMarker.gameObject);
                pointMarker.GetComponent<PointMarker>().SetMarkerMiniMap(markerMiniMap);
            }
            else
            {
                print("Not pointMarker");
            }
        }
        else
        {
            print("Not markerMiniMap or markerInScene");
        }

        //if (pointMarker)
        //{
        //    pointMarker.GetComponent<PointMarker>().SetMarkerMiniMap(markerMiniMap.transform);
        //}
        //else
        //{
        //    print("Not pointMarker");
        //}

        currentLerpZoom = scaleZoomMap;
        CalculationMap();
    }

    private void Update()
    {
        PositionPlayerOnMap();
        PositionMarkerOnScene();
        UpdateMouseScrollWheel();
        ShowTargetPointQuestOnMap();
    }

    private void UpdateMouseScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentLerpZoom += stepZoomMap;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentLerpZoom -= stepZoomMap;
        }
        ZoomMap();
    }

    private void SetScaleZoomMap(float stepZoomMap)
    {
        currentLerpZoom += stepZoomMap;
    }

    private void ZoomMap()
    {
        currentLerpZoom = Mathf.Clamp(currentLerpZoom, 50, 200);
        scaleZoomMap = Mathf.Lerp(scaleZoomMap, currentLerpZoom, Time.deltaTime * timerLerpZoom);
        ChangeScale();
        CalculationMap();
        PositionPlayerOnMap();
        PositionMarkerOnMap();
    }

    private void CalculationMap()
    {
        if (contentMap)
        {
            ratioX = (contentMap.sizeDelta.x) / terrainX;
            ratioY = (contentMap.sizeDelta.y) / terrainY;
        }
        else
        {
            print("Not ContentMap");
        }
    }

    private void PositionPlayerOnMap()
    {
        if (pointPlayer)
        {
            float positionX = thisTransform.position.x - originX;
            float positionY = thisTransform.position.z - originZ;
            pointPlayer.localPosition = new Vector3(positionX * ratioX - 40, positionY * ratioY - 40);
            pointPlayer.localEulerAngles = new Vector3(pointPlayer.localEulerAngles.x, pointPlayer.localEulerAngles.y, -thisTransform.eulerAngles.y);
        }
        else
        {
            print("Not PointPlayer");

        }
    }

    private void PositionMarkerOnMap()
    {
        if (markerInScene && pointMarker)
        {
            float positionX = markerInScene.position.x - originX;
            float positionY = markerInScene.position.z - originZ;
            pointMarker.localPosition = new Vector3(positionX * ratioX - 40, positionY * ratioY - 40);
        }
        else
        {
            print("Not markerInScene or pointMarker");
        }
    }

    private void PositionMarkerOnScene()
    {
        if (markerInScene && pointMarker)
        {
            markerInScene.position = new Vector3((pointMarker.localPosition.x + 40) / ratioX, 0, (pointMarker.localPosition.y + 40) / ratioY);
            markerInScene.position = new Vector3(markerInScene.position.x + originX, 0, markerInScene.position.z + originZ);
        }
        else
        {
            print("Not markerInScene or pointMarker");
        }
    }

    private void ChangeScale()
    {
        contentMap.sizeDelta = new Vector2(imageWidth * scaleZoomMap / 100, imageHeigth * scaleZoomMap / 100);
        if (setZoomText) setZoomText.text = Mathf.Round(scaleZoomMap).ToString();
    }

    public void ShowTargetPointQuestOnMap()
    {
        int i = 0;
        TargetMiniMap[] arrayTargetMiniMaps = targetManager.GetTargetMiniMaps();

        for (; i < arrayTargetMiniMaps.Length; i++)
        {
            pointMissions[i].gameObject.SetActive(arrayTargetMiniMaps[i].gameObject.activeInHierarchy);

            Transform target = arrayTargetMiniMaps[i].GetTarget();
            if (target)
            {
                pointMissions[i].SetTarget(target);
                float positionX = target.position.x - originX;
                float positionY = target.position.z - originZ;
                pointMissions[i].GetComponent<RectTransform>().localPosition = new Vector3(positionX * ratioX - 40, positionY * ratioY - 40);
            }
        }

        for (; i < pointMissions.Length; i++)
        {
            pointMissions[i].gameObject.SetActive(false);
        }
    }

    //////////////////////////
    //[SerializeField] 
    private RectTransform parent; // родительский объект для иконок карты, игрока и т.п.
    private Vector3[] worldCorners = new Vector3[4];
    //[SerializeField] 
    private RectTransform mapRect; // трансформ иконки карты
    private void NavigationPlayer()
    {
        if (pointPlayer)
        {
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);

            if (!rect.Contains(pointPlayer.position))
            {
                UpdateWorldCorners();
                Vector3 pos = new Vector3(Screen.width / 2, Screen.height / 2, 0) - pointPlayer.position;
                if (worldCorners[0].x > 0 && worldCorners[2].x < Screen.width) pos.x = 0;
                if (worldCorners[0].y > 0 && worldCorners[2].y < Screen.height) pos.y = 0;
                //parent.position += pos;
                UpdateWorldCorners();
                //parent.position = PositionCorrection(parent.position);
            }

            //float x = contentMap.sizeDelta.x / ratioX;
            //float y = contentMap.sizeDelta.y / ratioY;

            //contentMap.localPosition = new Vector3(x - pointPlayer.localPosition.x, y - pointPlayer.localPosition.y, pointPlayer.localPosition.z);
            contentMap.position = new Vector3(pointPlayer.position.x, pointPlayer.position.y, 0);

            //print("content pos" + contentMap.position);
            //print(" playerOn map  " + pointPlayer.position);

        }
        else
        {
            print("Navigation Player");
        }
    }

    Vector3 PositionCorrection(Vector3 position) // функция "прилипания" к краю экрана
    {
        Vector3 pos = Vector3.zero;

        float x = Mathf.Max(worldCorners[0].x, Screen.width - worldCorners[2].x);
        float y = Mathf.Max(worldCorners[0].y, Screen.height - worldCorners[2].y);

        if (worldCorners[0].x > 0 && worldCorners[2].x > Screen.width) pos.x = -x;
        else if (worldCorners[0].x < 0 && worldCorners[2].x < Screen.width) pos.x = x;
        if (worldCorners[0].y > 0 && worldCorners[2].y > Screen.height) pos.y = -y;
        else if (worldCorners[0].y < 0 && worldCorners[2].y < Screen.height) pos.y = y;

        return position + pos;
    }

    void UpdateWorldCorners() // получаем позиции углов иконки карты
    {
       // mapRect.GetWorldCorners(worldCorners);
        worldCorners[0].x = Round(worldCorners[0].x);
        worldCorners[0].y = Round(worldCorners[0].y);
        worldCorners[2].x = Round(worldCorners[2].x);
        worldCorners[2].y = Round(worldCorners[2].y);
    }

    float Round(float f) // необходимое округление до сотых, чтобы исключить погрешности вычислений
    {
        return ((int)(f * 100f)) / 100f;
    }

    /////////////////////////

}
