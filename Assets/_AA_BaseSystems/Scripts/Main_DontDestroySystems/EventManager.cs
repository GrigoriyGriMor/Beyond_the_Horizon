using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public Action<string> playerStartGame { get; set; } //����� playerStartGame?.Invoke(string);
}
