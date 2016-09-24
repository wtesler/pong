using UnityEngine;

public class Module : MonoBehaviour {

    public NearbyConnectionsClient nearbyConnectionsClient() {
        if (MainComponent.nearbyConnectionsClient == null) {
            MainComponent.nearbyConnectionsClient = new NearbyConnectionsClient();
        }
        return MainComponent.nearbyConnectionsClient;
    }

	public Responsibilities responsibilities() {
		if (MainComponent.responsibilities == null) {
			MainComponent.responsibilities = new Responsibilities();
		}
		return MainComponent.responsibilities;
	}

	public TimeCalibrator timeCalibrator() {
		if (MainComponent.timeCalibrator == null) {
			MainComponent.timeCalibrator = new TimeCalibrator(
				nearbyConnectionsClient(), 
				responsibilities());
		}
		return MainComponent.timeCalibrator;
	}
}