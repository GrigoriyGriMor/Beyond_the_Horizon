using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapImage : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private RectTransform markerMap;
    private RectTransform thisRectTransfrom;
    //[SerializeField]
    private PointMarker pointMarker;

    private void Start()
    {
        markerMap.gameObject.SetActive(false);
        thisRectTransfrom = GetComponent<RectTransform>();
        pointMarker = markerMap.GetComponent<PointMarker>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            DoubleClick(eventData.position);
        }
    }

    public void DoubleClick(Vector2 eventDataPosition)
    {
        if (markerMap && thisRectTransfrom)
        {
            Transform tempRectTransform = markerMap.parent;
            markerMap.gameObject.SetActive(true);
            markerMap.SetParent(thisRectTransfrom);
            markerMap.position = new Vector2(eventDataPosition.x, eventDataPosition.y);
            markerMap.SetParent(tempRectTransform);
            pointMarker.SetActiveMarkerMiniMap(true);
        }
        else
        {
            print("Not markerMap or RectTransform");
        }
    }

}
