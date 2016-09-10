using UnityEngine;

public class AndroidInitializer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
}
