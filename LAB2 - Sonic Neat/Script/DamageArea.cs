using UnityEngine;
using System.Collections;

public class DamageArea : MonoBehaviour {
	public int damage = 50;
	
	void  OnTriggerEnter ( Collider other  ){
		if (other.gameObject.tag == "Player") {
			other.GetComponent<Status>().OnDamage(damage , 0);
		}
	}
}