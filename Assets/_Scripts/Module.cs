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
}