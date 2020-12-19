using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Status))]
[RequireComponent (typeof (CharacterController))]

public class AIDefender : MonoBehaviour {
	public bool SnapToPlayerX = true;
	[HideInInspector]
	public GameObject mainModel;
	public float detectRange = 5.0f;
	private float distance = 0.0f;
	private GameObject target;
	
	public Transform bulletPrefab;
	public Transform attackPoint;
	public float waitTime = 2.2f;
	public float attackCast = 0.3f;
	public float attackDelay = 1.2f;
	
	public AnimationClip idleAnimation;
	public AnimationClip attackAnimation;
	private float nextFire = 0.0f;
	
	private int atk = 0;
	private int matk = 0;
	public bool  freeze = false;
	private GameObject[] gos;

	public AudioClip attackVoice;
	public AudioClip hurtVoice;
	// Use this for initialization
	void Start () {
		gameObject.tag = "Enemy"; 
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		target = GameObject.FindWithTag("Player");
		if(!attackPoint){
			attackPoint = this.transform;
		}
		if(SnapToPlayerX){
			transform.position = new Vector3(target.transform.position.x , transform.position.y , transform.position.z);
		}
		if(hurtVoice){
			GetComponent<Status>().hurtVoice = hurtVoice;
		}
		
		mainModel = GetComponent<Status>().mainModel;
	}
	
	Vector3  GetDestination (){
		Vector3 destination = target.transform.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	// Update is called once per frame
	void Update () {
		if(!target){
			gos = GameObject.FindGameObjectsWithTag("Player");  
			if (gos.Length > 0) {
				target = FindPlayer();
			}
			return;
		}
		Status stat = GetComponent<Status>();
		if(freeze || stat.freeze){
			return;
		}
		distance = (transform.position - GetDestination()).magnitude;
		if (distance <= detectRange) {
			if(Time.time > nextFire ){
				StartCoroutine(Attack());
			}
			Vector3 lookTarget = target.transform.position;
			lookTarget.y = transform.position.y;
			transform.LookAt(lookTarget);
		}
	}
	
	GameObject FindPlayer (){ 
		// Find Closest Player   
		//GameObject closest = new GameObject();
		GameObject closest = GameObject.FindWithTag("Player"); 
		gos = GameObject.FindGameObjectsWithTag("Player"); 
		if(gos.Length > 0){
			float distance = Mathf.Infinity; 
			Vector3 position = transform.position; 
			
			foreach(GameObject go in gos) { 
				Vector3 diff = (go.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					closest = go; 
					distance = curDistance;
				} 
			} 
		}
		return closest; 
	}
	
	IEnumerator  Attack (){
		Transform bulletShootout;
		Status stat = GetComponent<Status>();
		if(!stat.flinch || !GetComponent<Status>().freeze || !freeze){
			freeze = true;
			if(attackAnimation){
				mainModel.GetComponent<Animation>().Play(attackAnimation.name);
			}
			if(attackVoice){
				GetComponent<AudioSource>().clip = attackVoice;
				GetComponent<AudioSource>().Play();
			}
			yield return new WaitForSeconds(attackCast);
		
			bulletShootout = Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
			bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
			yield return new WaitForSeconds(attackDelay);
			freeze = false;
			mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
			nextFire = Time.time + waitTime;
		}
		
	}
}
