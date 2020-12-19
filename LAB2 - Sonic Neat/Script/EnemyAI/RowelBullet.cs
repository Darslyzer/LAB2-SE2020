using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BulletStatus))]
[RequireComponent (typeof (Rigidbody))]

public class RowelBullet : MonoBehaviour {
	public float Speed = 20;
	public Vector3 relativeDirection= Vector3.forward;
	public float duration = 6.0f;
	public float waitForAtk = 1.8f;
	public string shooterTag = "Player";
	private GameObject hitEffect;
	private int bulletStep = 0;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().isKinematic = true;
		hitEffect = GetComponent<BulletStatus>().hitEffect;
		Destroy (gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
		if(bulletStep == 0){
			transform.position += Vector3.down *Speed* Time.deltaTime;
		}
		if(bulletStep >= 2){
			Vector3 absoluteDirection = transform.rotation * relativeDirection;
			transform.position += absoluteDirection *Speed* Time.deltaTime;
		}
		
	}
	
	void  OnTriggerEnter ( Collider other  ){
		
		if (other.gameObject.tag == "Wall" && bulletStep <= 0) {
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			bulletStep = 1;
			StartCoroutine(WaitForAttack());
		}
	}
	
	IEnumerator WaitForAttack(){
		yield return new WaitForSeconds(waitForAtk);
		GameObject target = GameObject.FindWithTag("Player");
		if(target){
			transform.LookAt(target.transform);
		}
		bulletStep = 2;
	}
}
