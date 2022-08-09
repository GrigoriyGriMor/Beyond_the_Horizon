using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMiniMap : MonoBehaviour
{
    [SerializeField]
    private Transform cameraCenterRotate;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private GameObject pointTarget;
    private float distanceVisibleTargetOnMiniMap;
    private CustomClass distance = new CustomClass();
    private AreaUI areaUI;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateTargetMiniMap();
    }

    private void UpdateTargetMiniMap()
    {
        if (target)
        {
            if (cameraCenterRotate)
            {
                if (rectTransform != null)
                {
                    Vector3 relativePos = target.position - cameraCenterRotate.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                    rectTransform.localEulerAngles = new Vector3(rectTransform.localEulerAngles.x, rectTransform.localEulerAngles.y, cameraCenterRotate.eulerAngles.y - rotation.eulerAngles.y);

                    SetActivePointTarget(distance.Distance(target.position, cameraCenterRotate.position) > distanceVisibleTargetOnMiniMap);
                }
            }
        }
    }

    private void SetActivePointTarget(bool isActivePointTarget)
    {
        pointTarget.SetActive(isActivePointTarget);
        //SetActiveImageArea(isActivePointTarget);          //////////////////  выкл область
    }

    public void SetCameraCenterRotate(Transform cameraCenterRotate)
    {
        this.cameraCenterRotate = cameraCenterRotate;
    }

    public void SetTarget(Transform target)
    {
        if (this.target)
        {
            if (areaUI) areaUI.SetActiveImageArea(false);
        }

        if (target)
        {
            areaUI = target.GetComponentInChildren<AreaUI>();
        }

        if (areaUI)
        {
            areaUI.SetActiveImageArea(true);
        }

        this.target = target;
    }

    public void SetActiveImageArea(bool isActiveImageArea)
    {
        if(areaUI) areaUI.SetActiveImageArea(isActiveImageArea);
    }

    public void SetDistanceVisibleTargetOnMiniMap(float distanceVisibleTargetOnMiniMap)
    {
        this.distanceVisibleTargetOnMiniMap = distanceVisibleTargetOnMiniMap * distanceVisibleTargetOnMiniMap;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetActiveTarget(bool isActive)
    {
        target.gameObject.SetActive(isActive);
    }

}
/// <summary>
/// —амодельные классы
/// </summary>
public class CustomClass
{
    /// <summary>
    /// Ќахождение рассто€ние между точками в квадрате
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public float Distance(Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude;
    }
}
