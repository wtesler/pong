using UnityEngine;

public class ClickListener : MonoBehaviour {
    
    private IClickListener _listener;
    
    public void setOnClickListener(IClickListener listener) {
        _listener = listener;
	}

	    public void OnClick() {
        _listener.OnClick();
    }
}
