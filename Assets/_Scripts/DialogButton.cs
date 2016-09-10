using UnityEngine;

public class DialogButton : MonoBehaviour {
    
    private IConfirmDialogListener _listener;

	public void setOnClickListener(IConfirmDialogListener listener) {
        _listener = listener;
	}
    
    public void OnAcceptClick() {
        _listener.OnClick(true);
    }
    
    public void OnCancelClick() {
        _listener.OnClick(false);
    }
}
