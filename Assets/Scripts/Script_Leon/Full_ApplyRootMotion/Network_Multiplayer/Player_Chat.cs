using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
public class Player_Chat : NetworkBehaviour
{
    public static Action<string> onMessageReceived;
    public static Action<Player_Chat> onLocalPlayerCreated;
    public static Action<string> onLocalPlayerNameChanged;
    public static int playerID = 1;


    [SyncVar(hook = "OnReceiveNewName")] // de dong bo tu nhung gi da doi tren server xuong cac client
    public string playername;

    public void OnReceiveNewName(string newName)
    {
        playername = newName;
        if (isLocalPlayer)
        {
            onLocalPlayerNameChanged(newName);
        }
    }


    public override void OnStartLocalPlayer()
    {
        if (onLocalPlayerCreated != null)
        {
            onLocalPlayerCreated(this);
        }
        CmdPlayerLogin();
    }
    [Command]
    public void CmdPlayerLogin()
    {
        playername = "Player" + playerID;
        playerID++;
    }

    [Command]
    public void CmdAddChatMessage(string chatMessage)
    {
        RpcAddChatMessage(chatMessage);
    }

    [ClientRpc]
    public void RpcAddChatMessage(string chatMessage)
    {
        if (!isLocalPlayer && onMessageReceived != null)
        {
            onMessageReceived("[" + playername + "]:" + chatMessage);
        }
    }
}
