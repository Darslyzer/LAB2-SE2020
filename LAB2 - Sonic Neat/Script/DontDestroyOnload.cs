using UnityEngine;
using System.Collections;

public class DontDestroyOnload : MonoBehaviour {
	
	void  Awake (){
		DontDestroyOnLoad (transform.gameObject);
	}
}