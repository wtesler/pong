using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using System;
using GooglePlayGames.BasicApi.Nearby;
using UniRx;

public class NearbyConnectionsClient
{
    private bool mNearbyInitialized;
    private bool mAdvertiseDeferred;
    private bool mDiscoverDeferred;
    private IDiscoveryListener mDiscoveryListenerDeferred;
	private MessageDataStream mMessageDataStream;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();
    private Action<ConnectionRequest> mConnectionActionDeferred;
	Action<ConnectionResponse> mConnectionResponseAction;
	private String mCurrentConnectionEndpoint;
    
	public NearbyConnectionsClient() {
		mMessageDataStream = new MessageDataStream();

		getMessageObservable (MessageType.NCC)
			.Subscribe (message => mCurrentConnectionEndpoint = message.endpointId)
			.AddTo (mSubscriptions);

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

		Debug.Log ("Starting to advertise");
        
		List<string> appIdentifiers = new List<string> ();
		appIdentifiers.Add("tesler.will.pong");
		PlayGamesPlatform.Nearby.StartAdvertising (
			"Will Tesler",
			appIdentifiers,
			TimeSpan.FromSeconds (0),// 0 = advertise forever
			result => {
				Debug.Log ("OnAdvertisingResult: " + result.Status);
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

		Debug.Log ("Starting to discover");
        
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
		PlayGamesPlatform.Nearby.SendConnectionRequest(name, remote.EndpointId, FromString(message),
			connectionResponse => {
				Debug.Log("Connected to " + connectionResponse.RemoteEndpointId);
				mCurrentConnectionEndpoint = connectionResponse.RemoteEndpointId;
				mConnectionResponseAction(connectionResponse);
			}, mMessageDataStream);
	}

    public void Accept(string endpointId) {
		mCurrentConnectionEndpoint = endpointId;
		PlayGamesPlatform.Nearby.AcceptConnectionRequest(endpointId, FromString("Let's Play!"), mMessageDataStream);
    }
    
    public void Reject(string endpointId) {
		PlayGamesPlatform.Nearby.RejectConnectionRequest(endpointId);
    }

	public void SendMessage(byte[] payload, byte[] messageType, bool reliable) {
		SendMessage(mCurrentConnectionEndpoint, payload, messageType, reliable);
    }

	public void SendMessage(string endpointId, byte[] payload, byte[] messageType, bool reliable) {
        List<string> endpointIdList = new List<string>();
        endpointIdList.Add(endpointId);
		SendMessage(endpointIdList, payload, messageType, reliable);
    }
    
	public void SendMessage(List<string> endpointIds, byte[] payload, byte[] messageType, bool reliable) {
		byte[] message = new byte[messageType.Length + payload.Length];
		Array.Copy (messageType, message, 4);
		Array.Copy (payload, 0, message, 4, payload.Length);

		if (reliable) {
			PlayGamesPlatform.Nearby.SendReliable(endpointIds, message);
		} else {
			PlayGamesPlatform.Nearby.SendUnreliable(endpointIds, message);
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

	public IObservable<Message> getMessageObservable(byte[] messageType) {
		return mMessageDataStream.getMessageObservable()
			.Where (message => {
				byte[] type = new byte[4];
				Array.Copy (message.content, type, 4);
				return type == messageType;
			})
			.Select (message => {
				byte[] data = new byte[message.content.Length - 4];
				Array.Copy (message.content, 4, data, 0, message.content.Length - 4);
				message.content = data;
				return message;
			});
    }

	public IObservable<string> getDisconnectObservable() {
		return mMessageDataStream.getDisconnectObservable();
	}

	public void setRequestResponseAction(Action<ConnectionResponse> action) {
		mConnectionResponseAction = action;
	}

	public static byte[] FromString(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static string ToString(byte[] bytes) {
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	public static float ToFloat(byte[] bytes) {
		return System.BitConverter.ToSingle(bytes, 0);
	}

	public static byte[] FromFloat(float f) {
		return System.BitConverter.GetBytes(f);
	}

	public static double ToDouble(byte[] bytes) {
		return System.BitConverter.ToDouble(bytes, 0);
	}

	public static byte[] FromDouble(double d) {
		return System.BitConverter.GetBytes(d);
	}
}
