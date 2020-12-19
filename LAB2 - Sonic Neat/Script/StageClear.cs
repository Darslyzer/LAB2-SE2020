using UnityEngine;
using System.Collections;

public class StageClear : MonoBehaviour {
	public GUIStyle textStyle;

	void OnGUI() {
        GUI.Label(new Rect(Screen.width /2 -200 , Screen.height /2 -75 , 400, 150), "Stage Clear!! Thank You for Playing." , textStyle);
    }
}
