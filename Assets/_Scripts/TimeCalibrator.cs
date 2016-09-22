using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UniRx;

public class TimeCalibrator : MonoBehaviour {

	public Module module;
	public Text text;

	private NearbyConnectionsClient mNearbyClient;
	private Responsibilities mResponsibilities;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	private double mStartTime;
	private double mClockDelta;

	private bool mIsCalibrated;

	void Start() {
		mNearbyClient = module.nearbyConnectionsClient();
		mResponsibilities = module.responsibilities ();

		mNearbyClient.getMessageObservable (MessageType.TIME)
			.Subscribe (message => {
				if (mResponsibilities.isHost) {
					double receivedTime = getTime();
					Debug.Log ("Responding to Handshake at time: " + receivedTime.ToString());
					mNearbyClient.SendMessage (NearbyConnectionsClient.FromDouble(receivedTime), MessageType.TIME, true);
				} else {
					double hostReceivedTime = NearbyConnectionsClient.ToDouble(message.content);
					double currentTime = getTime();
					double latency = (mStartTime - currentTime) / 2;
					mClockDelta = (hostReceivedTime - currentTime) + latency;
					text.color = Color.green;
					Debug.Log("Time Delta: " + mClockDelta.ToString());
					mIsCalibrated = true;
				}
			})
			.AddTo (mSubscriptions);

		if (!mResponsibilities.isHost) {
			StartCoroutine (HandshakeCoroutine());
		}

		InvokeRepeating ("updateText", 0, 2);
	}

	void Destroy() {
		mSubscriptions.Clear();
	}

	public double getCalibratedTime () {
		return getTime () + mClockDelta;
	}

	private void startHandshake() {
		mStartTime = getTime();
		Debug.Log ("Starting Handshake at time: " + mStartTime.ToString());
		mNearbyClient.SendMessage (NearbyConnectionsClient.FromString("time"), MessageType.TIME, true);
	}

	private void updateText() {
		double time = getTime () + mClockDelta;
		string timeString = time.ToString ("#");
		text.text = timeString.Substring(timeString.Length - 7);
	}

	private double getTime() {
		TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
		return t.TotalMilliseconds;
	}

	IEnumerator HandshakeCoroutine() {
		yield return new WaitForSeconds(10);
		startHandshake ();
	}
}
