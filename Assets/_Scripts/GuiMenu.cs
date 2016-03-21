using UnityEngine;

/// <summary>
/// Centered GUI menu example
/// </summary>
/// <remarks>Author: Danko Kozar</remarks>
public class GuiMenu : MonoBehaviour {
	public Rect ButtonSize = new Rect (0, 0, 150, 100);
	public float GapSize = 10;
	private string[] Buttons = new[] { "Option 1", "Option 2", "Option 3", "Option 4", "Option 5", "Option 6", "Option 7", "Option 8", "Option 9", "Option 10", "Option 11" };
	public GUISkin Skin;

	private Rect _menuBounds;

	void OnGUI () {
		int count = Buttons.Length;
		float width = ButtonSize.width; // the width of a single button
		float height = ButtonSize.height * count + GapSize * Mathf.Max (count - 1, 0); // sum of button heights and gaps
		float x = (Screen.width - width) * 0.5f;
		float y = (Screen.height - height) * 0.5f;

		//Vector2 sizeDelta = transform.parent.parent.parent.GetComponent<RectTransform> ().sizeDelta;
		//transform.parent.parent.parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (sizeDelta.x, height);

		_menuBounds = new Rect (x, y, width, height);

		if (null != Skin) // apply skin
			GUI.skin = Skin;

		GUI.BeginClip (_menuBounds); // group start

		int index = 0;
		foreach (string s in Buttons) {
			if (GUI.Button (new Rect (0, ((ButtonSize.height + GapSize) * index) - transform.parent.localPosition.y, ButtonSize.width, ButtonSize.height), s)) { // relative to group, so x and y start from 0
				ClickHandler (index);
			}
			index++;
		}

		GUI.EndClip (); // group end
	}

	void ClickHandler (int index) {
		Debug.Log ("Clicked button at index " + index);
	}
}
