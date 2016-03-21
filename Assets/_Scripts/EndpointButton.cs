using UnityEngine;

public class EndpointButton : MonoBehaviour {
    
    private IClickListener _listener;
    private int _index;

	public void setOnClickListener(IClickListener listener) {
		Debug.Log("Click!");
	}
    
    public void setIndex(int index) {
		_index = index;
	}
    
    public void OnClick() {
        _listener.OnClick(_index);
    }
}
