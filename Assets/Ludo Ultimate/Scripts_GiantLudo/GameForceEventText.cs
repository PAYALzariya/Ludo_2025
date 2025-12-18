using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO;
using UnityEngine.UI;

public class GameForceEventText : MonoTemplate
{
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            MoveToken();
        }
    }

    public int MoveTokenId;
    private void MoveToken()
    {
        GetSocketManager.PlayerAction(ServerSocketManager.instance.playerId, MoveTokenId, (socket, packet, args)=> {
            Debug.Log($"PlayerAction Response: {packet}");
        });
    }

    public InputField input;
    public void MoveTokenTest()
    {
        if(input.text.Length > 0)
        {
            GetSocketManager.PlayerAction(ServerSocketManager.instance.playerId, int.Parse(input.text), (socket, packet, args) => {
                Debug.Log($"PlayerAction Response: {packet}");
            });
        }
    }
}
