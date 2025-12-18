using System;

public interface ISocket
{
    void On(string eventName, Action<object[]> callback);
    void Emit(string eventName, Action<object[]> ack = null, params object[] args);
    void Connect();
    void Close();
}