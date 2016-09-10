using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

class DiscoveryListener : IDiscoveryListener {

		MenuController _menuController;
		GameObject _listContent;
		GameObject _buttonPrefab;

		public DiscoveryListener(MenuController menuController, GameObject lc, GameObject bp) {
			_menuController = menuController;
			_listContent = lc;
			_buttonPrefab = bp;
		}

		#region IDiscoveryListener implementation
		public void OnEndpointFound (EndpointDetails discoveredEndpoint) {
			Debug.Log("Found Endpoint: " +
				discoveredEndpoint.DeviceId + " " +
				discoveredEndpoint.EndpointId + " " + 
				discoveredEndpoint.Name);

			_menuController.AddItem(discoveredEndpoint);
		}

		public void OnEndpointLost (string lostEndpointId) {
			Debug.Log("Endpoint lost: " + lostEndpointId);
		}
		#endregion
	}
    