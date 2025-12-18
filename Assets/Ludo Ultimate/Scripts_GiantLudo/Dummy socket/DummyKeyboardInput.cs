using UnityEngine;

public class DummyKeyboardInput : MonoBehaviour
{
    public DummyRootSocket dummy;
    public string emitname, boardcastname;
    public ServerSocketManager server;
    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.E))
        {
            // fire dummy emit
            server.AppSocket.Emit(emitname, null);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // fire dummy broadcast (example)
            (server.AppSocket as DummyRootSocket)?.FireBroadcast(boardcastname);
        }*/
    }
}


