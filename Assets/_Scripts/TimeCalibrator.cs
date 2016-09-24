using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UniRx;

public class TimeCalibrator {

	private NearbyConnectionsClient mNearbyClient;
	private Responsibilities mResponsibilities;

	private Subject<int> mCalibrationFinishedSubject = new Subject<int>();
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	private double mHandshakeStartTime;
	private double mClockDelta;

	private double mMatchStartTime;

	private bool mIsCalibrated;

	private bool mHandshakeAcknowledged;

	public TimeCalibrator(
		NearbyConnectionsClient nearbyClient,
		Responsibilities responsibilities) {
			mNearbyClient = nearbyClient;
			mResponsibilities = responsibilities;
	}

	public void Start() {
		if (mResponsibilities.isHost) {
			mNearbyClient.getMessageObservable (MessageType.TIME)
				.Subscribe (message => {
					if (!mHandshakeAcknowledged) {
						double receivedTime = getTime();
						Debug.Log ("Responding to Handshake at time: " + receivedTime.ToString ());
						mHandshakeAcknowledged = true;
						mNearbyClient.SendMessage (NearbyConnectionsClient.FromDouble (receivedTime), MessageType.TIME, true);
					} else {
						mMatchStartTime = NearbyConnectionsClient.ToDouble(message.content);
						mIsCalibrated = true;
						mCalibrationFinishedSubject.OnNext(0);
					}
				})
				.AddTo (mSubscriptions);
		} else {
			mNearbyClient.getMessageObservable (MessageType.TIME)
				.Subscribe (message => {
					double currentTime = getTime();
					double latency = (mHandshakeStartTime - currentTime) / 2;

					double hostReceivedTime = NearbyConnectionsClient.ToDouble(message.content);
					mClockDelta = (hostReceivedTime - currentTime) + latency;

					Debug.Log("Time Delta: " + mClockDelta.ToString());
					mIsCalibrated = true;
					mMatchStartTime = getCalibratedTime() + 5000;
					mNearbyClient.SendMessage (NearbyConnectionsClient.FromDouble (mMatchStartTime), MessageType.TIME, true);
					mCalibrationFinishedSubject.OnNext(0);
				})
				.AddTo (mSubscriptions);
		}
	}

	public void Destroy() {
		mSubscriptions.Clear();
	}

	public double getCalibratedTime () {
		return getTime () + mClockDelta;
	}

	public double getMatchStartTime () {
		return mMatchStartTime;
	}

	public void startHandshake() {
		mHandshakeStartTime = getTime();
		Debug.Log ("Starting Handshake at time: " + mHandshakeStartTime.ToString());
		mNearbyClient.SendMessage (NearbyConnectionsClient.FromString("time"), MessageType.TIME, true);
	}

	public IObservable<int> getCalibrationFinishedObservable() {
		return mCalibrationFinishedSubject.AsObservable<int>();
	}

	private double getTime() {
		TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
		return t.TotalMilliseconds;
	}
}
