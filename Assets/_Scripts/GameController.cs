using System;
using UnityEngine;
using UniRx;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

    public Module module;
    public GameObject handle;
	public BallDropper ballDropper;
	public Text countdownText;

    private NearbyConnectionsClient mNearbyClient;
	private Responsibilities mResponsibilities;
	private TimeCalibrator mTimeCalibrator;

	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	private double mMatchStartTime;
	private bool mIsMatchStarted;

    void Start() {
        mNearbyClient = module.nearbyConnectionsClient();
		mResponsibilities = module.responsibilities();
		mTimeCalibrator = module.timeCalibrator();
        
		mNearbyClient.getMessageObservable (MessageType.COOP)
			.Subscribe (message => {
				float xPosition =  NearbyConnectionsClient.ToFloat(message.content);
				Vector3 pos = handle.transform.position;
				handle.transform.position = new Vector3(xPosition, pos.y, pos.z);
			})
			.AddTo (mSubscriptions);

		mMatchStartTime = mTimeCalibrator.getMatchStartTime();	
    }

	void Update() {
		if (!mIsMatchStarted) {
			if (mTimeCalibrator.getCalibratedTime() >= mMatchStartTime) {
				mIsMatchStarted = true;
				countdownText.enabled = false;
				ballDropper.createBall();
			} else {
				double timeRemaining = mMatchStartTime - mTimeCalibrator.getCalibratedTime();
				int secondsRemaining = (int) timeRemaining / 1000;
				countdownText.text = secondsRemaining.ToString ();
			}
		}
	}

    void OnDestroy() {
		mSubscriptions.Clear ();
    }
}