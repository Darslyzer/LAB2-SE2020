using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Status))]
[RequireComponent (typeof (CharacterMotor))]

public class AIFollow : MonoBehaviour {
	
	public bool SnapToPlayerX = true;
	public enum AIState { Moving = 0, Pausing = 1 , Idle = 2 , Patrol = 3}
	[HideInInspector]
	public GameObject mainModel;
	public Transform followTarget;
	public float approachDistance = 2.0f;
	public float detectRange = 15.0f;
	public float lostSight = 100.0f;
	public float speed = 4.0f;
	public AnimationClip movingAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip hurtAnimation;
	
	public bool  stability = false;
	
	public bool  freeze = false;
	
	public Transform bulletPrefab;
	public Transform attackPoint;

	public float attackCast = 0.3f;
	public float attackDelay = 0.5f;
	
	private AIState followState;
	private float distance = 0.0f;
	private int atk = 0;
	private int matk = 0;
	[HideInInspector]
	public bool cancelAttack = false;
	private bool attacking = false;
	private bool castSkill = false;
	private GameObject[] gos;

	public AudioClip attackVoice;
	public AudioClip hurtVoice;
	
	void Start(){
		gameObject.tag = "Enemy"; 
		followTarget = GameObject.FindWithTag("Player").transform;
		if(SnapToPlayerX){
			transform.position = new Vector3(followTarget.transform.position.x , transform.position.y , transform.position.z);
		}
		if(!attackPoint){
			attackPoint = this.transform;
		}
		
		if(!mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}
		if(hurtVoice){
			GetComponent<Status>().hurtVoice = hurtVoice;
		}
		//Set ATK = Monster's Status
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		
		followState = AIState.Idle;
		mainModel.GetComponent<Animation>().Play(idleAnimation.name);
		if(hurtAnimation){
			GetComponent<Status>().hurt = hurtAnimation;
			mainModel.GetComponent<Animation>()[hurtAnimation.name].layer = 10;
		}
		
	}
	
	Vector3 GetDestination(){
		Vector3 destination = followTarget.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	void Update(){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();
		gos = GameObject.FindGameObjectsWithTag("Player");  
			if (gos.Length > 0) {
				followTarget = FindClosest().transform;
			}
		
		if (stat.flinch){
			cancelAttack = true;
			Vector3 lui = transform.TransformDirection(Vector3.back);
			controller.Move(lui * 3 * Time.deltaTime);
			return;
		}
		
		if(freeze || stat.freeze){
			return;
		}
		
		if(!followTarget){
			return;
		}
		//-----------------------------------
		
		if (followState == AIState.Moving) {
			if ((followTarget.position - transform.position).magnitude <= approachDistance) {
				followState = AIState.Pausing;
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
				//----Attack----
				//Attack();
				StartCoroutine(Attack());
			}else if ((followTarget.position - transform.position).magnitude >= lostSight)
			{//Lost Sight
				GetComponent<Status>().health = GetComponent<Status>().maxHealth;
				followState = AIState.Idle;
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);  
			}else {
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				controller.Move(forward * speed * Time.deltaTime);
				
				Vector3 destiny = followTarget.position;
				destiny.y = transform.position.y;
				transform.LookAt(destiny);
			}
		}
		else if (followState == AIState.Pausing){
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			
			distance = (transform.position - GetDestination()).magnitude;
			if (distance > approachDistance) {
				followState = AIState.Moving;
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}
		}
		//----------------Idle Mode--------------
		else if (followState == AIState.Idle){
			Vector3 destinyheight = followTarget.position;
			destinyheight.y = transform.position.y - destinyheight.y;
			int getHealth = GetComponent<Status>().maxHealth - GetComponent<Status>().health;
			
			distance = (transform.position - GetDestination()).magnitude;
			if (distance < detectRange && Mathf.Abs(destinyheight.y) <= 4 || getHealth > 0){
				followState = AIState.Moving;
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}
		}
		//-----------------------------------
	}
	
		
	IEnumerator  Attack (){
		cancelAttack = false;
		Transform bulletShootout;
		Status stat = GetComponent<Status>();
		if(!stat.flinch || !GetComponent<Status>().freeze || !freeze || !attacking){
			freeze = true;
			attacking = true;
			mainModel.GetComponent<Animation>().Play(attackAnimation.name);
			if(attackVoice){
				GetComponent<AudioSource>().clip = attackVoice;
				GetComponent<AudioSource>().Play();
			}
			yield return new WaitForSeconds(attackCast);
		
			//attackPoint.transform.LookAt(followTarget);
			if(!cancelAttack){
				bulletShootout = Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
				yield return new WaitForSeconds(attackDelay);
				freeze = false;
				attacking = false;
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
				CheckDistance();
			}else{
				freeze = false;
				attacking = false;
			}

		}
		
	}
	
	void  CheckDistance (){
		if(!followTarget){
			mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);  
			followState = AIState.Idle;
			return;
		}
		float distancea = (followTarget.position - transform.position).magnitude;
		if (distancea <= approachDistance){
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			StartCoroutine(Attack());
			//Attack();
		}else{
			followState = AIState.Moving;
			mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
		}
	}
	
	
	GameObject FindClosest (){ 
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

	public void ActivateSkill( Transform skill  ,   float castTime  ,   float delay  ,   string anim  ,   float dist  ){
		StartCoroutine(UseSkill(skill ,attackCast, attackDelay , anim , dist));
	}


	public IEnumerator  UseSkill ( Transform skill  ,   float castTime  ,   float delay  ,   string anim  ,   float dist  ){
		cancelAttack = false;
		Status stat = GetComponent<Status>();
		if(!stat.flinch && followTarget && (followTarget.position - transform.position).magnitude < dist && !GetComponent<Status>().silence && !GetComponent<Status>().freeze  && !castSkill){
			freeze = true;
			castSkill = true;
			mainModel.GetComponent<Animation>().Play(anim);
			//Transform bulletShootout;
			yield return new WaitForSeconds(castTime);
			//attackPoint.transform.LookAt(followTarget);
			if(!cancelAttack){
				Transform bulletShootout = Instantiate(skill, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
				yield return new WaitForSeconds(delay);
				freeze = false;
				castSkill = false;
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				freeze = false;
				castSkill = false;
			}
			

		}

		
	}
	

}