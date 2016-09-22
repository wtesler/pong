using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class Boundary : MonoBehaviour {

	private Subject<int> mCollisionSubject = new Subject<int>();

	void OnCollisionEnter(Collision collision) {
		mCollisionSubject.OnNext (0);
	}

	public IObservable<int> getCollisionObservable() {
		return mCollisionSubject.AsObservable<int>();
	}
}
