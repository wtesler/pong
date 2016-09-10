using UnityEngine;

public class Module : MonoBehaviour {

    static Component component;

    void Awake() {
        if (component == null) {
            component = new Component();
        }
    }

    public NearbyConnectionsClient nearbyConnectionsClient() {
        if (component.nearbyConnectionsClient == null) {
            component.nearbyConnectionsClient = new NearbyConnectionsClient();
        }
        return component.nearbyConnectionsClient;
    }
}