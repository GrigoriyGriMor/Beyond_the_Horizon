using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : SingletonMN<GameManager>
{
    [FormerlySerializedAs("EventManager"), SerializeField]
    private EventManager _eventManager = null;

    static public EventManager EventManager { get => Instance.GetManager(Instance._eventManager); }
}
