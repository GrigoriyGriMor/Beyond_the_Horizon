using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MarkerInScene : MonoBehaviour
{
    //[SerializeField]
    private GameObject pointMarker;

    [SerializeField]
    private ParticleSystem partSystem;

    public void SetPointMarker(GameObject markerMiniMap)
    {
        this.pointMarker = markerMiniMap;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pointMarker)
        {
            pointMarker.GetComponent<PointMarker>().SetActiveMarkerMiniMap(false);
            pointMarker.SetActive(false);
        }
        else
        {
            print("Not markerMiniMap");
        }
    }
}
