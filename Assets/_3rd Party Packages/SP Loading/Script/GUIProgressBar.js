#pragma strict
//Progress Bar Example Script by EvilSystem

var progressBar : GUIStyle;
var fontStyleLoading : GUIStyle;
var fontStylePercentage : GUIStyle;

var marginLeft : int = 0; // Progress Bar Margin Left
var marginTop : int = 0; // Progress Bar Margin Top

var frameImage : Texture2D; // Frame Image (Background Image)
var frameImageWidth : int = 1114; // Frame Width
var frameImageHeight: int = 71; // Frame Height

var fillImage : Texture2D; // Fill Bar Image
var fillImageWidth : int = 1043; // Fill Bar  Width
var fillImageHeight: int = 31; // Fill Bar  Height
var fillMarginLeft: int = 30; // Fill Bar Margin Left
var fillMarginTop: int = 20; // Fill Bar Margin Top

//Text Margins
var loadingMarginLeft: int = 40; // Loading Margin Left
var loadingMarginTop: int = 4; // Loading Margin Top
var percMarginLeft: int = 1050; // Percentage Margin Left
var percMarginTop: int = 8; // Percentage Margin Top

static var playerEnergy : float = 0.0f; // a float between 0.0 and 1.0

function Update()
{
	if (playerEnergy < 1.0f)
	{
		playerEnergy += 0.003f;
	}
	else
	{
		playerEnergy = 0.0f;
	}
}

function OnGUI ()
{
	GUI.BeginGroup (Rect (marginLeft, marginTop, frameImageWidth, frameImageHeight)); // Creating one Group for whole loading bar
	
	GUI.BeginGroup (Rect (0, 0, frameImageWidth, frameImageHeight)); //Group for both images
	GUI.Box (Rect (0, 0, frameImageWidth, frameImageHeight), frameImage, progressBar); // Draw frame image
	
	GUI.BeginGroup (Rect (fillMarginLeft, fillMarginTop, playerEnergy * fillImageWidth, fillImageHeight)); // Create second Group for the fill bar image
	GUI.Box (Rect (0, 0, fillImageWidth, fillImageHeight), fillImage, progressBar); // Draw the fill bar image
	GUI.EndGroup ();
	
	GUI.EndGroup ();
	
	var percentage : float = playerEnergy * 100;
	var percentageStr : String = parseInt(percentage).ToString();
	
	//GUI.Label (Rect (loadingMarginLeft, loadingMarginTop, 100, 20), "Loading...", fontStyleLoading); 
	//GUI.Label (Rect (percMarginLeft, percMarginTop, 100, 20), percentageStr + "%", fontStylePercentage);
	
	GUI.EndGroup ();
}