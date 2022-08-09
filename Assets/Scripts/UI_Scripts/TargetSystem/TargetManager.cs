using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    //[SerializeField]
    //private TargetMiniMap prefabTargetMiniMap;
    [SerializeField]
    private Transform parentObject;
    [SerializeField]
    private Transform cameraCenterRotate;
    [SerializeField]
    private Camera miniMapCamera;
    public TargetMiniMap[] arrayTargetMiniMaps;
    public TargetMiniMap[] arrayTargetQuestBrandMiniMaps;
    [SerializeField]
    private float distanceVisibleTargetOnMiniMap = 5;

    public enum TypeTarget
    {
        basic,
        brand
    }

    private void Start()
    {

    }

    public void ShowTargetQuest(int index, int ID, int indexStepNumber, int indexActionTargets, TypeTarget typeQuest)
    {
        if (AreaManager.Instance)
        {
            List<TargetQuest> targetQuests;

            if (typeQuest == TypeTarget.basic)
            {
                targetQuests = AreaManager.Instance.targetQuests;
            }
            else if (typeQuest == TypeTarget.brand)
            {
                targetQuests = AreaManager.Instance.targetQuestsBrand;
            }
            else
            {
                return;
            }

            for (int indexTargetQuests = 0; indexTargetQuests < targetQuests.Count; indexTargetQuests++)
            {
                if (targetQuests.Count > index)
                {
                    if (ID == targetQuests[indexTargetQuests].ID)
                    {
                        arrayTargetMiniMaps[index].gameObject.SetActive(true);

                        if (targetQuests.Count > 0)
                        {
                            if (targetQuests[indexTargetQuests].stepTargets.Count > 0)
                            {
                                if (targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets.Count > 0)
                                {
                                    Transform target = targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets[indexActionTargets].actionTarget;
                                    if (target)
                                    {
                                        arrayTargetMiniMaps[index].SetTarget(target);
                                    }
                                    else
                                    {
                                        print(" Not target");
                                    }
                                }
                            }
                        }
                        else
                        {
                            print(" Not AreaManager.Instance.targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets");
                        }

                        arrayTargetMiniMaps[index].SetCameraCenterRotate(cameraCenterRotate);
                        arrayTargetMiniMaps[index].SetDistanceVisibleTargetOnMiniMap(distanceVisibleTargetOnMiniMap);
                        break;
                    }
                }
            }
        }
        else

        {
            print("Not AreaManager.Instance");
        }
    }

    public TargetMiniMap[] GetTargetMiniMaps()
    {
        return arrayTargetMiniMaps;
    }

    //for (int indexTargetQuests = 0; indexTargetQuests < AreaManager.Instance.targetQuests.Length; indexTargetQuests++)
    //{
    //    if (AreaManager.Instance.targetQuests.Length > index)
    //    {
    //        if (ID == AreaManager.Instance.targetQuests[indexTargetQuests].ID)
    //        {
    //            arrayTargetMiniMaps[index].gameObject.SetActive(true);
    //            if ((AreaManager.Instance.targetQuests.Length > 0) && (AreaManager.Instance.targetQuests[indexTargetQuests].stepTargets.Length > 0) &&
    //                (AreaManager.Instance.targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets.Length > 0))
    //            {
    //                Transform target = AreaManager.Instance.targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets[indexActionTargets].actionTarget;
    //                arrayTargetMiniMaps[index].SetTarget(target);
    //            }
    //            else
    //            {
    //                print(" Not AreaManager.Instance.targetQuests[indexTargetQuests].stepTargets[indexStepNumber].actionTargets");
    //            }
    //            arrayTargetMiniMaps[index].SetCameraCenterRotate(cameraCenterRotate);
    //            arrayTargetMiniMaps[index].SetDistanceVisibleTargetOnMiniMap(distanceVisibleTargetOnMiniMap);
    //            break;
    //        }
    //    }
    //}

}

