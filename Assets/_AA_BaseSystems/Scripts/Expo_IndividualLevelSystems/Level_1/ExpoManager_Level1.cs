using UnityEngine;
using UnityEngine.Serialization;

public class ExpoManager_Level1 : SingletonMN<ExpoManager_Level1>
{
    [FormerlySerializedAs("EventManager"), SerializeField]
    private SpaceManager _spaceManager = null;

    static public SpaceManager SpaceManager { get => Instance.GetManager(Instance._spaceManager); }
}
