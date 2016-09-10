using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{

    private static float mListItemStartYOffset = -400;
    private static float mListItemCurrentYOffset = 0f;

    private NearbyConnectionsClient ncc;
    public Module module;
    public GameObject listContent;
    public GameObject requestDialog;
    public GameObject disconnectButton;
    public GameObject beepButton;
    public GameObject buttonPrefab;

    public Beeper beeper;

    public bool ______________;

    private List<EndpointDetails> _endpointList = new List<EndpointDetails>();

    private ConnectionRequest _pendingConnectionRequest;

    private IDiscoveryListener _discoveryListener;
    private IIndexClickListener _endpointclickListener;
    private IConfirmDialogListener _confirmDialogListener;

    private IClickListener _disconnectClickListener;

    private IClickListener _beepClickListener;

    private Action<String, String> mMessageAction;

    void Awake()
    {
        ncc = module.nearbyConnectionsClient();

        // Setup Listeners
        _discoveryListener = new DiscoveryListener(this, listContent, buttonPrefab);
        _endpointclickListener = new EndpointClickListener(this);
        _confirmDialogListener = new ConfirmRequestClickListener(this);
        _disconnectClickListener = new DisconnectClickListener(this);
        _beepClickListener = new BeepClickListener(this);
        // Setup Dialog buttons
        DialogButton[] buttons = requestDialog.GetComponentsInChildren<DialogButton>();
        foreach (DialogButton db in buttons)
        {
            db.setOnClickListener(_confirmDialogListener);
        }

        disconnectButton.GetComponent<ClickListener>().setOnClickListener(_disconnectClickListener);
        beepButton.GetComponent<ClickListener>().setOnClickListener(_beepClickListener);

        mMessageAction = ((endpointId, data) => {
            beeper.Beep();
        });
    }

    void Start() {
        ncc.Discover(_discoveryListener);
        ncc.Advertise(getConnectRequestAction());

        ncc.registerMessageAction(mMessageAction);

        ncc.setRequestResponseAction((response) => {
            Debug.Log("response: " + response.ResponseStatus);
            ncc.SendMessage(response.RemoteEndpointId, NearbyConnectionsClient.GetBytes("Hooray!"), true);
            beeper.Beep();
        });
    }

    void OnDestroy() {
        ncc.unregisterMessageAction(mMessageAction);
    }

    public Action<ConnectionRequest> getConnectRequestAction() {
        return (ConnectionRequest request) =>
        {
            Debug.Log("Received connection request: " +
            request.RemoteEndpoint.DeviceId + " " +
            request.RemoteEndpoint.EndpointId + " " +
            request.RemoteEndpoint.Name);

            ncc.StopAdvertise();

            _pendingConnectionRequest = request;
            requestDialog.transform.Find("Name").GetComponent<Text>().text = request.RemoteEndpoint.Name;
            requestDialog.SetActive(true);
        };
    }

    public void AddItem(EndpointDetails endpointDetails)
    {
        foreach (EndpointDetails ed in _endpointList)
        {
            if (ed.DeviceId == endpointDetails.DeviceId)
            {
                return;
            }
        }
        _endpointList.Add(endpointDetails);

        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.parent = listContent.transform;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = new Vector3(980, mListItemStartYOffset - mListItemCurrentYOffset);
        mListItemCurrentYOffset += 140;

        EndpointButton endpointButton = button.GetComponent<EndpointButton>();
        endpointButton.setIndex(_endpointList.Count - 1);
        endpointButton.setOnClickListener(_endpointclickListener);

        Debug.Log("Endpoint found!");

        UnityEngine.UI.Text uiText = button.transform.GetComponentInChildren<UnityEngine.UI.Text>();
        uiText.text = endpointDetails.Name;
    }

    public void endpointClicked(int index) {
        EndpointDetails endpoint = _endpointList[index];
        ncc.SendRequest(endpoint, "Will Tesler", "Play a game?");
    }

    public void dialogClicked(bool accepted)
    {
        Debug.Log("Dialog Clicked.");
        requestDialog.SetActive(false);
        if (accepted)
        {
            ncc.Accept(_pendingConnectionRequest.RemoteEndpoint.EndpointId);
            ncc.StopDiscover();
        }
        else
        {
            ncc.Reject(_pendingConnectionRequest.RemoteEndpoint.EndpointId);
            ncc.Advertise(getConnectRequestAction());
        }
    }

    public void disconnectClicked()
    {
        ncc.DisconnectFromEndpoint();
    }

    public void beepClicked()
    {
        ncc.SendMessage(NearbyConnectionsClient.GetBytes("Beep!"), false);
        beeper.Beep();
    }

    class EndpointClickListener : IIndexClickListener
    {
        MenuController _menuController;

        public EndpointClickListener(MenuController menuController) {
            _menuController = menuController;
        }

        public void OnClick(int index) {
            Debug.Log("Endpoint Clicked.");

            _menuController.endpointClicked(index);
        }
    }

    class ConfirmRequestClickListener : IConfirmDialogListener
    {
        MenuController _menuController;

        public ConfirmRequestClickListener(MenuController menuController)
        {
            _menuController = menuController;
        }

        public void OnClick(bool accepted)
        {
            _menuController.dialogClicked(accepted);
        }
    }

    class DisconnectClickListener : IClickListener
    {
        MenuController _menuController;

        public DisconnectClickListener(MenuController menuController)
        {
            _menuController = menuController;
        }

        public void OnClick()
        {
            _menuController.disconnectClicked();
        }
    }

    class BeepClickListener : IClickListener
    {
        MenuController _menuController;

        public BeepClickListener(MenuController menuController)
        {
            _menuController = menuController;
        }

        public void OnClick()
        {
            _menuController.beepClicked();
        }
    }
}
