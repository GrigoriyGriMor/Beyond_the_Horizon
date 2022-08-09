using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Threading;
using System.Net;
using System.Net.WebSockets;

public class ChatSystem : MonoBehaviour
{
    private string PlayerName = "";

    [Header("active window")]
    [SerializeField] private Transform ActiveWindow;
    [SerializeField] private Transform rectTransformActive;
    [SerializeField] private InputField enterPlayerText;

    [Header("deactive window")]
    [SerializeField] private Transform DeactiveWindow;
    [SerializeField] private Transform rectTransformDeactive;

    [SerializeField] private GameObject stringPrefab;

    [SerializeField] private string chatServerAdress = "ws://regserver.cloudsgoods.com:8888/chat";

    private ClientWebSocket webSocket;
    private int player_id;

    private void Start() {
        ActiveWindow.gameObject.SetActive(false);
        DeactiveWindow.gameObject.SetActive(true);
    }

    public async void Init(string playerName, int _id, string _session) {
        PlayerName = playerName;
        player_id = _id;

        // chatServerAdress = $"ws://regserver.cloudsgoods.com:8888/chat?id={_id}&session={_session}";
        chatServerAdress = $"ws://192.168.1.139:8888/chat?id={_id}&session={_session}";

        webSocket = new ClientWebSocket();
        try {
            await webSocket.ConnectAsync(new Uri(chatServerAdress), CancellationToken.None);
            if (webSocket.State == WebSocketState.Open) {
                Recieve();
                SetNewSend("System:", "Connect", SupportClass.SendType.mainSend);
            }
            else
                SetNewSend("System:", "Can't connect to chat", SupportClass.SendType.mainSend);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "chat system error");
        } 
    }

    public bool UseChat() {
        if (!ActiveWindow.gameObject.activeInHierarchy) {
            ActiveWindow.gameObject.SetActive(true);
            DeactiveWindow.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(enterPlayerText.gameObject);
            enterPlayerText.MoveTextEnd(false);
        }
        else {
            if (enterPlayerText.text.Length <= 0) {
                ActiveWindow.gameObject.SetActive(false);
                DeactiveWindow.gameObject.SetActive(true);

                return false;
            }
            else {
                if (webSocket != null) SendNewMessage();

                EventSystem.current.SetSelectedGameObject(enterPlayerText.gameObject);
                enterPlayerText.MoveTextEnd(false);
            }
        }

        return true;
    }

    private async void Recieve() {
        ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[1024]);

        while (true) {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(bytesReceived, CancellationToken.None);
            if (result.Count != 0) {
                string json = System.Text.Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);
                SendInChatFormat send = JsonUtility.FromJson<SendInChatFormat>(json);

                SupportClass.SendType s_type = SupportClass.SendType.mainSend;

                switch (send.type) {
                    case "broadcast":
                        s_type = SupportClass.SendType.mainSend;
                        break;
                }

                SetNewSend(send.id, send.message, s_type);
            }
        }
    }

    private async void SendNewMessage() {
        SendInChatFormat send = new SendInChatFormat();
        send.message = enterPlayerText.text;
        send.id = player_id.ToString();
        send.type = "broadcast";

        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(send)));

        await webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        enterPlayerText.text = "";
    }

    private void SetNewSend(string name, string send, SupportClass.SendType type) {
        ChatString chatString = Instantiate(stringPrefab, Vector2.zero, Quaternion.identity, rectTransformDeactive).GetComponent<ChatString>();
        chatString.SetStringData(name, send, type);

        chatString = Instantiate(stringPrefab, Vector2.zero, Quaternion.identity, rectTransformActive).GetComponent<ChatString>();
        chatString.SetStringData(name, send, type);
    }
}

[Serializable]
public class SendInChatFormat {
    public string type;
    public string id;
    public string message;
}