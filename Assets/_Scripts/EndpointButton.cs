using UnityEngine;

public class EndpointButton : MonoBehaviour {
    
    private IIndexClickListener _listener;
    private int _index;

	public void setOnClickListener(IIndexClickListener listener) {
		Debug.Log("Click!");
        _listener = listener;
	}
    
    public void setIndex(int index) {
		_index = index;
	}
    
    public void OnClick() {
        _listener.OnClick(_index);
    }
}
