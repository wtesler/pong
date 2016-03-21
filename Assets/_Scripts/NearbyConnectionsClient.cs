using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi.Nearby;

public class NearbyConnectionsClient : MonoBehaviour
{
    private bool _nearbyInitialized;
    private bool _advertiseDeferred;
    private bool _discoverDeferred;
    private IDiscoveryListener _discoverListenerDeferred;
    private Action<ConnectionRequest> _connectionActionDeferred;
    
	void Awake() {
		PlayGamesPlatform.InitializeNearby((client) => {
            _nearbyInitialized = true;
			Debug.Log("Nearby connections initialized");
            
            if (_advertiseDeferred) {
                _advertiseDeferred = false;
                Advertise(_connectionActionDeferred);
            }
            
            if (_discoverDeferred) {
                _discoverDeferred = false;
                Discover(_discoverListenerDeferred);
            }
		});
	}
		
	void Start () {

	}

	public void Advertise (Action<ConnectionRequest> connectionRequestAction) {
        if (!_nearbyInitialized) {
          _advertiseDeferred = true; 
          _connectionActionDeferred = connectionRequestAction;
          return; 
        }
        
		List<string> appIdentifiers = new List<string> ();
		Debug.Log ("Starting to advertise");
		appIdentifiers.Add("tesler.will.pong");
		PlayGamesPlatform.Nearby.StartAdvertising (
			"Samsung Edge",  // User-friendly name
			appIdentifiers,  // App bundle Id for this game
			TimeSpan.FromSeconds (0),// 0 = advertise forever
			(AdvertisingResult result) => {
				Debug.Log ("OnAdvertisingResult: " + result);
			}, 
            connectionRequestAction
		);
	}

	public void Discover (IDiscoveryListener listener) {
		PlayGamesPlatform.Nearby.StartDiscovery (
			PlayGamesPlatform.Nearby.GetServiceId (),
			TimeSpan.FromSeconds (0),
			listener);
	}

	public void StopAdvertising() {
		PlayGamesPlatform.Nearby.StopAdvertising();
		Debug.Log("Advertising Stopped.");
	}

	public void StopDiscovering(string serviceId) {
		PlayGamesPlatform.Nearby.StopDiscovery(serviceId);
		Debug.Log("Discovery Stopped.");
	}

	public void SendConnectionRequest(EndpointDetails remote, string name, string message, IMessageListener listener) {
		PlayGamesPlatform.Nearby.SendConnectionRequest(
			name,
			remote.EndpointId,
			GetBytes(message),
			(response) => {
				Debug.Log("response: " + response.ResponseStatus);
			},
			listener);
	}

    public void Accept(string endpointId, IMessageListener listener) {
        PlayGamesPlatform.Nearby.AcceptConnectionRequest(
        endpointId,
        GetBytes("Let's Play!"),
        listener);
    }
    
    public void Reject(string endpointId) {
        PlayGamesPlatform.Nearby.RejectConnectionRequest(endpointId);
    }
    
    public void SendReliableMessage(List<string> endpointIds, byte[] payload) {
        PlayGamesPlatform.Nearby.SendReliable(endpointIds, payload);
    }
    
    
    public void SendUnreliableMessage(List<string> endpointIds, byte[] payload) {
        PlayGamesPlatform.Nearby.SendUnreliable(endpointIds, payload);
    }
    
    public void StopAdvertise() {
         PlayGamesPlatform.Nearby.StopAdvertising();
    }
    
    public void StopDiscover() {
         PlayGamesPlatform.Nearby.StopAdvertising();
    }

	public void DisconnectFromEndpoint(string endpointId) {
		PlayGamesPlatform.Nearby.DisconnectFromEndpoint(endpointId);
		Debug.Log("Disconnected from endpoint " + endpointId);
	}

	public void StopAllConnections() {
		PlayGamesPlatform.Nearby.StopAllConnections();
		Debug.Log("Stopped all connections.");
	}

	public static byte[] GetBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static string GetString(byte[] bytes) {
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}
}
