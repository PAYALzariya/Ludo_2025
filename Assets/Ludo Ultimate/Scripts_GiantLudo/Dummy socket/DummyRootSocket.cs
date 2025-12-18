using System;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.JSON;

public class DummyRootSocket : ISocket
{
    private Dictionary<string, List<Action<object[]>>> listeners = new Dictionary<string, List<Action<object[]>>>();
    private Dictionary<string, int> broadcastIndices = new Dictionary<string, int>();

    public DummyRootSocket()
    {
        Debug.Log("DummyRootSocket created");
    }

    public void On(string eventName, Action<object[]> callback)
    {
        if (!listeners.ContainsKey(eventName))
            listeners[eventName] = new List<Action<object[]>>();
        listeners[eventName].Add(callback);
        Debug.Log("DummyRootSocket On registered: " + eventName);
    }

    public void Emit(string eventName, Action<object[]> ack = null, params object[] args)
    {
        Debug.Log("DummyRootSocket Emit called: " + eventName);
        string json = DummyEmitResponses.Get(eventName);
        if (json == null)
        {
            Debug.LogWarning("DummyEmitResponses: no data for " + eventName);
            ack?.Invoke(new object[] { null });
            return;
        }
        var parsed = new object[] { Json.Decode(json) };
        ack?.Invoke(parsed);
    }

    public void FireBroadcast(string eventName)
    {
        var list = DummyBroadcastResponses.GetList(eventName);
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("No broadcast data for: " + eventName);
            return;
        }

        int idx = 0;
        if (!broadcastIndices.ContainsKey(eventName)) broadcastIndices[eventName] = 0;
        idx = broadcastIndices[eventName];
        broadcastIndices[eventName] = (broadcastIndices[eventName] + 1) % list.Count;

        string json = list[idx];
        string wrapper = "[\"" + eventName + "\"," + json + "]";
        var parsed = new object[] { Json.Decode(wrapper) };

        if (listeners.ContainsKey(eventName))
        {
            foreach (var cb in listeners[eventName])
            {
                try { cb.Invoke(parsed); } catch (Exception ex) { Debug.LogException(ex); }
            }
        }

        Debug.Log("DummyRootSocket fired broadcast: " + eventName);
    }

    public void Connect()
    {
        Debug.Log("DummyRootSocket Connect() called - no-op"); 
    }

    public void Close()
    {
        Debug.Log("DummyRootSocket Close() called - no-op"); 
    }
}