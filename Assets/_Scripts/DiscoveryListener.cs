using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

class DiscoveryListener : IDiscoveryListener {

		MainMenu _mainMenu;
		GameObject _listContent;
		GameObject _buttonPrefab;

		public DiscoveryListener(MainMenu mainMenu, GameObject lc, GameObject bp) {
			_mainMenu = mainMenu;
			_listContent = lc;
			_buttonPrefab = bp;
		}

		#region IDiscoveryListener implementation
		public void OnEndpointFound (EndpointDetails discoveredEndpoint) {
			Debug.Log("Found Endpoint: " +
				discoveredEndpoint.DeviceId + " " +
				discoveredEndpoint.EndpointId + " " + 
				discoveredEndpoint.Name);

			_mainMenu.AddItem(discoveredEndpoint);
		}

		public void OnEndpointLost (string lostEndpointId) {
			Debug.Log("Endpoint lost: " + lostEndpointId);
		}
		#endregion
	}
    