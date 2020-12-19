using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BulletStatus))]
[RequireComponent (typeof (Rigidbody))]

public class BulletShot : MonoBehaviour {
	public float Speed = 20;
	public Vector3 relativeDirection= Vector3.forward;
	public float duration = 1.0f;
	public string shooterTag = "Player";
	public bool  penetrate = false;
	private GameObject hitEffect;
	
	void  Start (){
		GetComponent<Rigidbody>().isKinematic = true;
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		Destroy (gameObject, duration);
	}
	
	
	void  Update (){
		
		Vector3 absoluteDirection = transform.rotation * relativeDirection;
		transform.position += absoluteDirection *Speed* Time.deltaTime;
		
		
	}
	
	void  OnTriggerEnter ( Collider other  ){
		
		if (other.gameObject.tag == "Wall") {
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(!penetrate){
				//Destroy this object if it not Penetrate
				Destroy (gameObject);
			}
			
		}
	}

}