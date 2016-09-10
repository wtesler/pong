using UnityEngine;
using System.Collections;

public class BallDropper : MonoBehaviour {

	public GameObject ballPrefab;

	// Use this for initialization
	void Start () {
		InvokeRepeating("createBall", 0, 3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void createBall() {
		GameObject ball = Instantiate(ballPrefab);
		Destroy(ball, 8);
	}
}
