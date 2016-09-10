using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi.Nearby;

public class NearbyConnectionsClient
{
    private bool mNearbyInitialized;
    private bool mAdvertiseDeferred;
    private bool mDiscoverDeferred;
    private IDiscoveryListener mDiscoveryListenerDeferred;
	private MessageHandler mMessageHandler;
    private Action<ConnectionRequest> mConnectionActionDeferred;
	Action<ConnectionResponse> mConnectionResponseAction;
	private String mCurrentConnectionEndpoint;
    
	public NearbyConnectionsClient() {
		mMessageHandler = new MessageHandler();
		mMessageHandler.register((endpointId, stringData) => {
			mCurrentConnectionEndpoint = endpointId;
		});

		PlayGamesPlatform.InitializeNearby((client) => {
            mNearbyInitialized = true;
			Debug.Log("Nearby connections initialized");
            
            if (mAdvertiseDeferred) {
                mAdvertiseDeferred = false;
                Advertise(mConnectionActionDeferred);
            }
            
            if (mDiscoverDeferred) {
                mDiscoverDeferred = false;
                Discover(mDiscoveryListenerDeferred);
            }
		});
	}

	public void Advertise (Action<ConnectionRequest> connectionRequestAction) {
        if (!mNearbyInitialized) {
          mAdvertiseDeferred = true; 
          mConnectionActionDeferred = connectionRequestAction;
          return; 
        }
        
		List<string> appIdentifiers = new List<string> ();
		Debug.Log ("Starting to advertise");
		appIdentifiers.Add("tesler.will.pong");
		PlayGamesPlatform.Nearby.StartAdvertising (
			"Will Tesler",
			appIdentifiers,
			TimeSpan.FromSeconds (0),// 0 = advertise forever
			(AdvertisingResult result) => {
				Debug.Log ("OnAdvertisingResult: " + result);
			}, 
            connectionRequestAction
		);
	}

	public void Discover (IDiscoveryListener listener) {
        if (!mNearbyInitialized) {
          mDiscoverDeferred = true; 
          mDiscoveryListenerDeferred = listener;
          return; 
        }
        
		PlayGamesPlatform.Nearby.StartDiscovery (
			PlayGamesPlatform.Nearby.GetServiceId(),
			TimeSpan.FromSeconds(0),
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

	public void SendRequest(EndpointDetails remote, string name, string message) {
		PlayGamesPlatform.Nearby.SendConnectionRequest(name, remote.EndpointId, GetBytes(message), 
			mConnectionResponseAction, mMessageHandler);
	}

    public void Accept(string endpointId) {
		mCurrentConnectionEndpoint = endpointId;
        PlayGamesPlatform.Nearby.AcceptConnectionRequest(endpointId, GetBytes("Let's Play!"), mMessageHandler);
    }
    
    public void Reject(string endpointId) {
        PlayGamesPlatform.Nearby.RejectConnectionRequest(endpointId);
    }

	public void SendMessage(byte[] payload, bool reliable) {
		SendMessage(mCurrentConnectionEndpoint, payload, reliable);
    }

	public void SendMessage(string endpointId, byte[] payload, bool reliable) {
        List<string> endpointIdList = new List<string>();
        endpointIdList.Add(endpointId);
        SendMessage(endpointIdList, payload, reliable);
    }
    
    public void SendMessage(List<string> endpointIds, byte[] payload, bool reliable) {
		if (reliable) {
        	PlayGamesPlatform.Nearby.SendReliable(endpointIds, payload);
		} else {
			PlayGamesPlatform.Nearby.SendUnreliable(endpointIds, payload);
		}
    }
    
    public void StopAdvertise() {
         PlayGamesPlatform.Nearby.StopAdvertising();
    }
    
    public void StopDiscover() {
         PlayGamesPlatform.Nearby.StopDiscovery(PlayGamesPlatform.Nearby.GetServiceId()) ;
    }

	public void DisconnectFromEndpoint() {
		if (mCurrentConnectionEndpoint == null) {
			return;
		}
		PlayGamesPlatform.Nearby.DisconnectFromEndpoint(mCurrentConnectionEndpoint);
		Debug.Log("Disconnected from endpoint " + mCurrentConnectionEndpoint);
	}

	public void StopAllConnections() {
		PlayGamesPlatform.Nearby.StopAllConnections();
		mCurrentConnectionEndpoint = null;
		Debug.Log("Stopped all connections.");
	}

	public void registerMessageAction(Action<String, String> action) {
        mMessageHandler.register(action);
    }

	public void unregisterMessageAction(Action<String, String> action) {
        mMessageHandler.unregister(action);
    }

	public void setRequestResponseAction(Action<ConnectionResponse> action) {
        mConnectionResponseAction = action;
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
