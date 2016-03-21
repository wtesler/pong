using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

class MessageListener : IMessageListener
{
    #region IMessageListener implementation
    public void OnMessageReceived(string remoteEndpointId, byte[] data, bool isReliableMessage)
    {
        string reliable = isReliableMessage ? "Reliable" : "Unreliable";
        string textData = NearbyConnectionsClient.GetString(data);
        Debug.Log(reliable + " message received from " + remoteEndpointId + ": " + textData);
    }

    public void OnRemoteEndpointDisconnected(string remoteEndpointId)
    {
        Debug.Log("Remote Endpoint " + remoteEndpointId + " Disconnected");
    }
    #endregion
}