using GooglePlayGames.BasicApi.Nearby;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageHandler : IMessageListener
{
    private List<Action<String, String>> mActions = new List<Action<String, String>>();

    public MessageHandler() {
    }

    #region IMessageListener implementation
    public void OnMessageReceived(string remoteEndpointId, byte[] data, bool isReliableMessage)
    {
        string reliable = isReliableMessage ? "Reliable" : "Unreliable";
        string textData = NearbyConnectionsClient.GetString(data);
        foreach (var action in mActions) {
            action(remoteEndpointId, textData);
        }
        Debug.Log(reliable + " message received from " + remoteEndpointId + ": " + textData);
    }

    public void OnRemoteEndpointDisconnected(string remoteEndpointId)
    {
        Debug.Log("Remote Endpoint " + remoteEndpointId + " Disconnected");
    }
    #endregion

    public void register(Action<String, String> action) {
        mActions.Add(action);
    }

    public void unregister(Action<String, String> action) {
        mActions.Remove(action);
    }
}