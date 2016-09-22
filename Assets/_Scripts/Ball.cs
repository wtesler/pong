using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	float mConstantSpeed = 4.0f;
 	float mSmoothingFactor = .5f;

	private Rigidbody mRigidbody;

	void Awake() {
		mRigidbody = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		mRigidbody.AddForce (Vector3.down);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate() {
		var current = mRigidbody.velocity;
		var normal = current.normalized * mConstantSpeed;
		mRigidbody.velocity = normal;// Vector3.Lerp(current, normal, Time.deltaTime * mSmoothingFactor);
 	}

	void OnCollisionEnter(Collision collision) {
        foreach (ContactPoint contact in collision.contacts) {
        	Debug.DrawRay(contact.point, contact.normal, Color.red);
			mRigidbody.AddForce (new Vector3(-contact.normal.x, -contact.normal.y, -contact.normal.z) * 20);
			//mRigidbody.AddForce (Vector3.down * 10);
			Debug.DrawRay(contact.point, contact.normal * 10, Color.green);
        }
    }
}
