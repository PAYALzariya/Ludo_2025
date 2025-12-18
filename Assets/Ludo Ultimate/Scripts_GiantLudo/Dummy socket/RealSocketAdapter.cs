using System;
using BestHTTP.SocketIO;

public class RealSocketAdapter : ISocket
{
    private Socket real;

    public RealSocketAdapter(Socket socket)
    {
        real = socket;
    }

    public void On(string eventName, Action<object[]> callback)
    {
        real.On(eventName, (socket, packet, args) =>
        {
            callback?.Invoke(args);
        });
    }

    public void Emit(string eventName, Action<object[]> ack = null, params object[] args)
    {
        real.Emit(eventName, (socket, packet, ackArgs) =>
        {
            ack?.Invoke(ackArgs);
        }, args);
    }

    public void Connect()
    {
        // BestHTTP SocketIO connects through Manager
        real.Manager.Open();
    }

    public void Close()
    {
        real.Manager.Close();
    }
}
