using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class BallDropper : MonoBehaviour {

	public Module module;
	public Ball ballPrefab;
	public Boundary boundary;

	Responsibilities mResponsibilities;

	private Ball mBall;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	// Use this for initialization
	void Start () {
		mResponsibilities = module.responsibilities();

		boundary.getCollisionObservable()
			.Subscribe<int> (statusCode => {
				Destroy(mBall.transform.gameObject);
				createBall();
			})
			.AddTo(mSubscriptions);
	}

	void Destroy() {
		mSubscriptions.Dispose();
	}

	public void createBall() {
		mBall = Instantiate(ballPrefab);
		Vector3 direction = mResponsibilities.isHost ? Vector3.down : Vector3.up;
		mBall.AddForce (direction * 20);
	}
}
