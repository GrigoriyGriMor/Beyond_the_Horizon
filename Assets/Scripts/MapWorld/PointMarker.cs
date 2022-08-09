using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PointMarker : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField]
    private TargetMiniMap markerMiniMap;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            DoubleClick(eventData.position);
        }
    }

    public void DoubleClick(Vector2 eventDataPosition)
    {
        gameObject.SetActive(false);
        SetActiveMarkerMiniMap(false);
    }

    public void SetMarkerMiniMap(TargetMiniMap markerMiniMap)
    {
        if (markerMiniMap)
        {
            this.markerMiniMap = markerMiniMap;
        }
        else
        {
            print(" Not markerMiniMap");
        }
    }

    public void SetActiveMarkerMiniMap(bool isActive)
    {
        if (markerMiniMap)
        {
            markerMiniMap.gameObject.SetActive(isActive);
            markerMiniMap.SetActiveTarget(isActive);

        }
        else
        {
            print(" Not markerMiniMap");
        }
    }
}
