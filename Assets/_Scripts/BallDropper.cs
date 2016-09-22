using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class BallDropper : MonoBehaviour {

	public Module module;
	public Ball ballPrefab;
	public Boundary boundary;

	private Ball mBall;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	// Use this for initialization
	void Start () {
		boundary.getCollisionObservable()
			.Subscribe<int> (statusCode => {
				Destroy(mBall.transform.gameObject);
				createBall();
			})
			.AddTo(mSubscriptions);
		
		createBall();
	}

	void Destroy() {
		mSubscriptions.Dispose();
	}

	void createBall() {
		mBall = Instantiate(ballPrefab);
	}
}
