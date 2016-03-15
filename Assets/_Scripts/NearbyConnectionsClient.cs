using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi.Nearby;

public class NearbyConnectionsClient : MonoBehaviour
{

	IDiscoveryListener mDiscoveryListener;
	IMessageListener mMessageListener;

	void Awake() {
		PlayGamesPlatform.InitializeNearby((client) => {
			Debug.Log("Nearby connections initialized");
		});
		
		mDiscoveryListener = new DiscoveryListener();
		mMessageListener = new MessageListener();

	}
		
	void Start () {
	
	}

	void Advertise () {
		List<string> appIdentifiers = new List<string> ();
		appIdentifiers.Add (PlayGamesPlatform.Nearby.GetAppBundleId ());
		PlayGamesPlatform.Nearby.StartAdvertising (
			"Awesome Game Host",  // User-friendly name
			appIdentifiers,  // App bundle Id for this game
			TimeSpan.FromSeconds (0),// 0 = advertise forever
			(AdvertisingResult result) => {
				Debug.Log ("OnAdvertisingResult: " + result);
			},
			(ConnectionRequest request) => {
				Debug.Log ("Received connection request: " +
				request.RemoteEndpoint.DeviceId + " " +
				request.RemoteEndpoint.EndpointId + " " +
				request.RemoteEndpoint.Name);
			}
		);
	}

	void Discover () {
		PlayGamesPlatform.Nearby.StartDiscovery (
			PlayGamesPlatform.Nearby.GetServiceId (),
			TimeSpan.FromSeconds (0),
			mDiscoveryListener);
	}

	void StopAdvertising() {
		PlayGamesPlatform.Nearby.StopAdvertising();
		Debug.Log("Advertising Stopped.");
	}

	void StopDiscovering() {
		PlayGamesPlatform.Nearby.StopDiscovery(serviceId);
		Debug.Log("Discovery Stopped.");
	}

	void SendConnectionRequest(EndpointDetails remote, string name, string message) {
		PlayGamesPlatform.Nearby.SendConnectionRequest(
			name,
			remote.EndpointId,
			GetBytes(message),
			(response) => {
				Debug.Log("response: " + response.ResponseStatus);
			},
			mMessageListener);
	}

	void DisconnectFromEndpoint(string endpointId) {
		PlayGamesPlatform.Nearby.DisconnectFromEndpoint(endpointId);
		Debug.Log("Disconnected from endpoint " + endpointId);
	}

	class DiscoveryListener : IDiscoveryListener {
		#region IDiscoveryListener implementation
		public void OnEndpointFound (EndpointDetails discoveredEndpoint)
		{
			Debug.Log("Found Endpoint: " +
				discoveredEndpoint.DeviceId + " " +
				discoveredEndpoint.EndpointId + " " + 
				discoveredEndpoint.Name);
		}

		public void OnEndpointLost (string lostEndpointId)
		{
			Debug.Log("Endpoint lost: " + lostEndpointId);
		}
		#endregion
	}

	class MessageListener : IMessageListener {
		#region IMessageListener implementation
		public void OnMessageReceived (string remoteEndpointId, byte[] data, bool isReliableMessage)
		{
			string reliable = isReliableMessage ? "Reliable" : "Unreliable";
			string textData = GetString(data);
			Debug.Log(reliable + " message received from " + remoteEndpointId + ": " + textData);
		}

		public void OnRemoteEndpointDisconnected (string remoteEndpointId)
		{
			Debug.Log("Remote Endpoint " + remoteEndpointId + " Disconnected");
		}
		#endregion
	}

	static byte[] GetBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	static string GetString(byte[] bytes) {
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}
}
