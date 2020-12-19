//This Script use for save position of player in the scene.
//For Retry when Gam Over the Player will Spawn at the Last Check Point Position.
using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {
	
	private GameObject player;
	
	void  OnTriggerEnter ( Collider other  ){
		if (other.gameObject.tag == "Player") {
			player = other.gameObject;
			SaveData();
		}
	}
	
	void  SaveData (){
		PlayerPrefs.SetInt("PreviousSave", 10);
		PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
		PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
		PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);

		print("Saved");
	}
}