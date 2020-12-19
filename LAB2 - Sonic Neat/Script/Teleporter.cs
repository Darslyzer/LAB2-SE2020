using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {
	
	public string teleportToMap = "Level1";
	//Vector3 spawnPosition;
	
	void  OnTriggerEnter ( Collider other  ){
		if(other.tag == "Player"){
			ChangeMap();
		}
		
	}
	
	void  ChangeMap (){
		Application.LoadLevel (teleportToMap);
	}
}