using GooglePlayGames.BasicApi.Nearby;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MessageDataStream : IMessageListener {

	private Subject<Message> mMessageSubject = new Subject<Message>();
	private Subject<string> mDisconnectSubject = new Subject<string>();

	public MessageDataStream() { }

    #region IMessageListener implementation
    public void OnMessageReceived(string remoteEndpointId, byte[] message, bool isReliable) {
		string reliable = isReliable ? "Reliable" : "Unreliable";
		Debug.Log(reliable + " message received from " + remoteEndpointId + ": ");
		mMessageSubject.OnNext (new Message(remoteEndpointId, message, isReliable));
    }

    public void OnRemoteEndpointDisconnected(string remoteEndpointId) {
        Debug.Log("Remote Endpoint " + remoteEndpointId + " Disconnected.");
		mDisconnectSubject.OnNext (remoteEndpointId);
    }
    #endregion

	public IObservable<Message> getMessageObservable() {
		return mMessageSubject.AsObservable<Message> ();
	}

	public IObservable<string> getDisconnectObservable() {
		return mDisconnectSubject.AsObservable<string> ();
	}
}
