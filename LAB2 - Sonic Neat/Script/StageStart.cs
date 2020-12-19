using UnityEngine;
using System.Collections;

public class StageStart : MonoBehaviour {
	public GameObject playerPrefab;
	public GameObject player;
	// Use this for initialization
	void Awake () {
		player = GameObject.FindWithTag("Player");
		if(player){
			//Set new originalX variable in PlayerInputController to Spawn Point position.x
			//For prevent the auto Set position of Player in PlayerInputController
			player.GetComponent<PlayerInputController>().originalX = this.transform.position.x;
			//Move Player to Spawn Point Position
			player.transform.position = this.transform.position;
			
			//Check for Player's Camera
			GameObject oldCam = player.GetComponent<AttackTrigger>().Maincam.gameObject;
			if(!oldCam){
				return;
			}
			//Destroy all Other Camera
			GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera"); 
			foreach(GameObject cam2 in cam) { 
				if(cam2 != oldCam){
					Destroy(cam2.gameObject);
				}
			}
		}else{
			//If there are no Player then spawn new Player
			player = Instantiate(playerPrefab , transform.position , playerPrefab.transform.rotation) as GameObject;
		}
	}


}