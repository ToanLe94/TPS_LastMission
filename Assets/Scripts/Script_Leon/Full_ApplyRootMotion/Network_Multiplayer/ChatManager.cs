using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewBehaviourScript : MonoBehaviour {

    public Text textChatLog;
    public InputField inputChatMessage;
    public InputField inputPlayername;
    public Player_Chat playerChat;

    private string chatLog = "";
    // Use this for initialization
    void Start()
    {
        textChatLog.text = chatLog;
        Player_Chat.onLocalPlayerCreated = (newLocalPlayer) => { playerChat = newLocalPlayer; };
        Player_Chat.onMessageReceived = AddToChatLog;
        Player_Chat.onLocalPlayerNameChanged = (newName) => { inputPlayername.text = newName; };

    }


    public void OnButtonSendClicked()
    {
        var newMessage = inputChatMessage.text;
        chatLog = inputChatMessage.text + "\r\n" + chatLog;
        textChatLog.text = chatLog;
        playerChat.CmdAddChatMessage(chatLog);

    }
    private void AddToChatLog(string newMessage)
    {
        chatLog = newMessage + "\r\n" + chatLog;
        textChatLog.text = chatLog;
    }
}
