using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;

class DiscoveryListener : IDiscoveryListener {

		private MenuController mMenuController;
		private GameObject mListContent;
		private GameObject mButtonPrefab;

		public DiscoveryListener(MenuController menuController, GameObject lc, GameObject bp) {
			mMenuController = menuController;
			mListContent = lc;
			mButtonPrefab = bp;
		}

		#region IDiscoveryListener implementation
		public void OnEndpointFound (EndpointDetails discoveredEndpoint) {
			Debug.Log("Found Endpoint: " +
				discoveredEndpoint.DeviceId + " " +
				discoveredEndpoint.EndpointId + " " + 
				discoveredEndpoint.Name);

			mMenuController.AddItem(discoveredEndpoint);
		}

		public void OnEndpointLost (string lostEndpointId) {
			Debug.Log("Endpoint lost: " + lostEndpointId);
		}
		#endregion
	}
    