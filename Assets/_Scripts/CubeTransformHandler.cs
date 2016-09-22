using UnityEngine;
using System;
using TouchScript.Gestures;

public class CubeTransformHandler : MonoBehaviour {

	public Module module;
	public TransformGesture transformGesture;

	private NearbyConnectionsClient mNearbyClient;

	private float mFixedY;

	void Start () {
		mNearbyClient = module.nearbyConnectionsClient();
		mFixedY = transform.position.y;
		transformGesture.Transformed += tranformHandler;
	}

	void Destroy () {
		transformGesture.Transformed -= tranformHandler;
	}
	
	private void tranformHandler(object sender, EventArgs eventArgs) {
		//Debug.Log(transform.position);
		transform.position = new Vector3(transform.position.x, mFixedY, transform.position.z);
		mNearbyClient.SendMessage(NearbyConnectionsClient.FromFloat(transform.position.x), MessageType.COOP, false);
	}
}
