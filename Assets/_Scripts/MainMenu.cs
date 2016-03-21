using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Nearby;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    
	private static float mListItemStartYOffset = -400;
	private static float mListItemCurrentYOffset = 0f;
    
    public GameObject listContent;
    
    public GameObject requestDialog;

	public GameObject buttonPrefab;
    
    public NearbyConnectionsClient _ncc;
    
    public bool ______________;
    
    private List<EndpointDetails> _endpointList = new List<EndpointDetails>();
    
    private ConnectionRequest _pendingConnectionRequest;
    
    private IDiscoveryListener _discoveryListener;
	private IMessageListener _messageListener;
    private IClickListener _clickListener;
    private IConfirmDialogListener _confirmDialogListener;
    
    void Awake() {
        _discoveryListener = new DiscoveryListener(this, listContent, buttonPrefab);
		_messageListener = new MessageListener();
        _clickListener = new EndpointClickListener(this);
        _confirmDialogListener = new ConfirmRequestClickListener(this);
    }
    
    void Start() {
        _ncc.Discover(_discoveryListener);
        _ncc.Advertise((ConnectionRequest request) => {
				Debug.Log ("Received connection request: " +
				request.RemoteEndpoint.DeviceId + " " +
				request.RemoteEndpoint.EndpointId + " " +
				request.RemoteEndpoint.Name);
                
                _pendingConnectionRequest = request;
                
                requestDialog.SetActive(true);
                requestDialog.transform.Find("Name").GetComponent<Text>().text = request.RemoteEndpoint.Name;
                DialogButton[] buttons = requestDialog.GetComponentsInChildren<DialogButton>();
                foreach (DialogButton db in buttons) {
                    db.setOnClickListener(_confirmDialogListener);
                }
			});
    }

    public void AddItem(EndpointDetails endpointDetails)
    {
        _endpointList.Add(endpointDetails);

        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.parent = listContent.transform;
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = new Vector3(980, mListItemStartYOffset - mListItemCurrentYOffset);
        mListItemCurrentYOffset += 140;
        
        EndpointButton endpointButton = button.GetComponent<EndpointButton>();
        endpointButton.setIndex(_endpointList.Count - 1);
        endpointButton.setOnClickListener(_clickListener);

        UnityEngine.UI.Text uiText = button.transform.GetComponentInChildren<UnityEngine.UI.Text>();
        uiText.text = endpointDetails.Name;
    }
    
    public void endpointClicked(int index) {
        EndpointDetails endpoint = _endpointList[index];
        _ncc.SendConnectionRequest(endpoint, "Will Tesler", "Play a game?", _messageListener);
    }
    
    public void dialogClicked(bool accepted) {
        Debug.Log("Dialog Clicked.");
        requestDialog.SetActive(false);
        if (accepted) {
            _ncc.Accept(_pendingConnectionRequest.RemoteEndpoint.EndpointId, _messageListener);
        } else {
            _ncc.Reject(_pendingConnectionRequest.RemoteEndpoint.EndpointId);
        }
    }

    class EndpointClickListener : IClickListener
    {
        MainMenu _mainMenu;
        
        public EndpointClickListener(MainMenu mainMenu) {
            _mainMenu = mainMenu;
        }
        
        public void OnClick(int index)
        {
            Debug.Log("Endpoint Clicked.");
            
            _mainMenu.endpointClicked(index);
        }
    }
    
    class ConfirmRequestClickListener : IConfirmDialogListener
    {
        MainMenu _mainMenu;
        
        public ConfirmRequestClickListener(MainMenu mainMenu) {
            _mainMenu = mainMenu;
        }
        
        public void OnClick(bool accepted) {
            _mainMenu.dialogClicked(accepted);
        }
    }
}
