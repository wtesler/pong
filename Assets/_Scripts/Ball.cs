using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	float mConstantSpeed = 4.0f;
 	float mSmoothingFactor = .5f;

	private Rigidbody mRigidbody;

	void Awake() {
		mRigidbody = GetComponent<Rigidbody>();
	}

	void LateUpdate() {
		var current = mRigidbody.velocity;
		var normal = current.normalized * mConstantSpeed;
		mRigidbody.velocity = normal;
 	}

	void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
        	Debug.DrawRay(contact.point, contact.normal, Color.red);
			mRigidbody.AddForce (new Vector3(-contact.normal.x, -contact.normal.y, -contact.normal.z) * 20);
			//mRigidbody.AddForce (Vector3.down * 10);
			Debug.DrawRay(contact.point, contact.normal * 10, Color.green);
        }
    }

	public void AddForce(Vector3 force) {
		mRigidbody.AddForce (force);
	}
}
