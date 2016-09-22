using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;


public class MenuController : MonoBehaviour {
    private static float mListItemStartYOffset = -400;
    private static float mListItemCurrentYOffset = 0f;

    public Module module;
    public GameObject listContent;
    public GameObject requestDialog;
    public GameObject disconnectButton;
    public GameObject beepButton;
    public GameObject buttonPrefab;
    public Beeper beeper;

    private NearbyConnectionsClient mNearbyClient;
    private List<EndpointDetails> mEndpointList = new List<EndpointDetails>();
    private ConnectionRequest mPendingConnectionRequest;
	private Responsibilities mResponsibilities;
	private CompositeDisposable mSubscriptions = new CompositeDisposable();

	private IDiscoveryListener mDiscoveryListener;
	private IIndexClickListener mEndpointclickListener;
	private IConfirmDialogListener mConfirmDialogListener;
	private IClickListener mDisconnectClickListener;
	private IClickListener mBeepClickListener;

    void Awake() {
        mNearbyClient = module.nearbyConnectionsClient();
		mResponsibilities = module.responsibilities ();

        mDiscoveryListener = new DiscoveryListener(this, listContent, buttonPrefab);
        mEndpointclickListener = new EndpointClickListener(this);
        mConfirmDialogListener = new ConfirmRequestClickListener(this);
        mDisconnectClickListener = new DisconnectClickListener(this);
        mBeepClickListener = new BeepClickListener(this);
        
        DialogButton[] buttons = requestDialog.GetComponentsInChildren<DialogButton>();
        foreach (var button in buttons) {
            button.setOnClickListener(mConfirmDialogListener);
        }

        disconnectButton.GetComponent<ClickListener>().setOnClickListener(mDisconnectClickListener);
        beepButton.GetComponent<ClickListener>().setOnClickListener(mBeepClickListener);
    }

    void Start() {
        mNearbyClient.Discover(mDiscoveryListener);
        mNearbyClient.Advertise(getConnectRequestAction());

		mNearbyClient.getMessageObservable (MessageType.MENU)
			.Subscribe (message => {
				SceneManager.LoadScene ("TimeScene");
				beeper.Beep ();
			})
			.AddTo (mSubscriptions);
				

        mNearbyClient.setRequestResponseAction((response) => {
            Debug.Log("response: " + response.ResponseStatus);
			mNearbyClient.SendMessage(response.RemoteEndpointId, NearbyConnectionsClient.FromString("Hooray!"), MessageType.MENU, true);
            beeper.Beep();
            SceneManager.LoadScene("TimeScene");
        });
    }

    void OnDestroy() {
        mNearbyClient.StopAdvertise();
        mNearbyClient.StopDiscover();
		mSubscriptions.Clear ();
    }

    public Action<ConnectionRequest> getConnectRequestAction() {
        return request => {
            Debug.Log("Received connection request: " +
            request.RemoteEndpoint.DeviceId + " " +
            request.RemoteEndpoint.EndpointId + " " +
            request.RemoteEndpoint.Name);

            mNearbyClient.StopAdvertise();

            mPendingConnectionRequest = request;
            requestDialog.transform.Find("Name").GetComponent<Text>().text = request.RemoteEndpoint.Name;
            requestDialog.SetActive(true);
        };
    }

    public void AddItem(EndpointDetails endpointDetails) {
        foreach (var details in mEndpointList) {
            if (details.DeviceId == endpointDetails.DeviceId) {
                return;
            }
        }
        mEndpointList.Add(endpointDetails);

        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.parent = listContent.transform;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = new Vector3(980, mListItemStartYOffset - mListItemCurrentYOffset);
        mListItemCurrentYOffset += 140;

        EndpointButton endpointButton = button.GetComponent<EndpointButton>();
        endpointButton.setIndex(mEndpointList.Count - 1);
        endpointButton.setOnClickListener(mEndpointclickListener);

        Debug.Log("Endpoint found!");

        UnityEngine.UI.Text uiText = button.transform.GetComponentInChildren<UnityEngine.UI.Text>();
        uiText.text = endpointDetails.Name;
    }

    public void endpointClicked(int index) {
        EndpointDetails endpoint = mEndpointList[index];
		mResponsibilities.isHost = true;
		Debug.Log ("You are the host.");
        mNearbyClient.SendRequest(endpoint, "Will Tesler", "Play a game?");
    }

    public void dialogClicked(bool accepted) {
        Debug.Log("Dialog Clicked.");
        requestDialog.SetActive(false);
        if (accepted) {
			mResponsibilities.isHost = false;
            mNearbyClient.Accept(mPendingConnectionRequest.RemoteEndpoint.EndpointId);
            mNearbyClient.StopDiscover();
        }
        else {
            mNearbyClient.Reject(mPendingConnectionRequest.RemoteEndpoint.EndpointId);
            mNearbyClient.Advertise(getConnectRequestAction());
        }
    }

    public void disconnectClicked() {
        mNearbyClient.DisconnectFromEndpoint();
    }

    public void beepClicked() {
        //beeper.Beep();
		SceneManager.LoadScene("TimeScene");
    }

    class EndpointClickListener : IIndexClickListener {
        MenuController mMenuController;

        public EndpointClickListener(MenuController menuController) {
            mMenuController = menuController;
        }

        public void OnClick(int index) {
            Debug.Log("Endpoint Clicked.");

            mMenuController.endpointClicked(index);
        }
    }

    class ConfirmRequestClickListener : IConfirmDialogListener {
        MenuController mMenuController;

        public ConfirmRequestClickListener(MenuController menuController) {
            mMenuController = menuController;
        }

        public void OnClick(bool accepted) {
            mMenuController.dialogClicked(accepted);
        }
    }

    class DisconnectClickListener : IClickListener {
        MenuController mMenuController;

        public DisconnectClickListener(MenuController menuController) {
            mMenuController = menuController;
        }

        public void OnClick() {
            mMenuController.disconnectClicked();
        }
    }

    class BeepClickListener : IClickListener  {
        MenuController mMenuController;

        public BeepClickListener(MenuController menuController) {
            mMenuController = menuController;
        }

        public void OnClick() {
            mMenuController.beepClicked();
        }
    }
}
