using System;
using UnityEngine;
using UniRx;


public class CoopController : MonoBehaviour {

    public Module module;
    public GameObject handle;

    private NearbyConnectionsClient mNearbyClient;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

    void Start() {
        mNearbyClient = module.nearbyConnectionsClient();
        
		mNearbyClient.getMessageObservable (MessageType.COOP)
			.Subscribe (message => {
				float xPosition =  NearbyConnectionsClient.ToFloat(message.content);
				Vector3 pos = handle.transform.position;
				handle.transform.position = new Vector3(xPosition, pos.y, pos.z);
			})
			.AddTo (mSubscriptions);
    }

    void OnDestroy() {
		mSubscriptions.Clear ();
    }
}