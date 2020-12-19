using UnityEngine;
using System.Collections;

public class LockPlayerBullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindWithTag("Player");
		if(player){
			this.transform.position = player.transform.position; // Set Position to Player
		}
	}
	
}
