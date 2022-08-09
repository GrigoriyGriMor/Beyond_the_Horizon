using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatString : MonoBehaviour
{
    [SerializeField] private Text senderName;
    [SerializeField] private Text SendText;

    [SerializeField] private Color mainChatColor = Color.white;
    [SerializeField] private Color groupChatColor = Color.green;
    [SerializeField] private Color privateChatColor = Color.red;

    public void SetStringData(string name, string text, SupportClass.SendType type) {
        senderName.text = name + ':';
        SendText.text = text;

        switch (type) {
            case SupportClass.SendType.mainSend:
                senderName.color = mainChatColor;
                break;
            case SupportClass.SendType.groupSend:
                senderName.color = groupChatColor;
                break;
            case SupportClass.SendType.privateSend:
                senderName.color = privateChatColor;
                break;
        }
    }
}
