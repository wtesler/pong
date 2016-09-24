using UnityEngine;
using System.Collections;
using System.Net;
using UniRx;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

	public Module module;

	private Responsibilities mResponsibilities;
	private TimeCalibrator mTimeCalibrator;

	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	void Start () {
		mResponsibilities = module.responsibilities ();
		mTimeCalibrator = module.timeCalibrator();

		mTimeCalibrator.Start();

		mTimeCalibrator.getCalibrationFinishedObservable()
			.Subscribe(statusCode => {
				Debug.Log("Opening Game Scene.");
				SceneManager.LoadScene("GameScene");
			})
			.AddTo (mSubscriptions);

		if (!mResponsibilities.isHost) {
			StartCoroutine (LoadingCoroutine ());
		}
	}

	void Destroy() {
		mSubscriptions.Clear();
		mTimeCalibrator.Destroy();
	}

	IEnumerator LoadingCoroutine() {
		yield return new WaitForSeconds(2);
		mTimeCalibrator.startHandshake();
	}
}

