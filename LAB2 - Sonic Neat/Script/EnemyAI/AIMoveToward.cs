using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Status))]
[RequireComponent (typeof (CharacterController))]

public class AIMoveToward : MonoBehaviour {
	public bool SnapToPlayerX = true;
	[HideInInspector]
	public GameObject mainModel;
	public bool ignorePlayerCollider = false;
	public float towardHeight = 0.0f;
	public float speed = 5.0f;
	public float approachDistance = 5.0f;
	public float detectRange = 15.0f;
	public float lostSight = 30.0f;
	private float distance = 0.0f;
	private GameObject target;
	
	public Transform bulletPrefab;
	public Transform attackPoint;
	public Transform attackPoint2;
	public float attackCast = 0.3f;
	public float attackDelay = 0.5f;
	private int atk = 0;
	private int matk = 0;
	
	[HideInInspector]
	public bool cancelAttack = false;
	private bool attacking = false;
	public bool freeze = false;
	
	public AnimationClip movingAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip hurtAnimation;
	private GameObject[] gos;

	public AudioClip attackVoice;
	public AudioClip hurtVoice;
	// Use this for initialization
	void Start () {
		gameObject.tag = "Enemy"; 
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		target = GameObject.FindWithTag("Player");
		if(ignorePlayerCollider){
			Physics.IgnoreCollision(target.GetComponent<Collider>(), GetComponent<Collider>());
		}
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
		if(hurtAnimation){
			GetComponent<Status>().hurt = hurtAnimation;
		}
	}
	
	Vector3 GetDestination(){
		Vector3 destination = target.transform.position;
		destination.y = transform.position.y;
		return destination;
	}
	// Update is called once per frame
	void Update(){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();
		if(!target){
			gos = GameObject.FindGameObjectsWithTag("Player");  
			if (gos.Length > 0) {
				target = FindPlayer();
			}
			return;
		}
		if(stat.flinch){
			cancelAttack = true;
			Vector3 lui = transform.TransformDirection(Vector3.back);
			controller.Move(lui * 3 * Time.deltaTime);
			return;
		}
		if(freeze || stat.freeze){
			return;
		}
		distance = (transform.position - GetDestination()).magnitude;
		if ((target.transform.position - transform.position).magnitude >= lostSight){
			mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
			return;
		}
		
		if (distance <= detectRange) {
			//Follow Player when enter the detect range
				Vector3 lookTarget = target.transform.position;
				lookTarget.y = transform.position.y;
				transform.LookAt(lookTarget);
				
					Vector3 destinyheight = target.transform.position;
					destinyheight.y = transform.position.y - destinyheight.y;
				//When Enemy Approach
				if (distance < approachDistance  && Mathf.Abs(destinyheight.y) <= towardHeight + 0.3) {
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
					if(!attacking && bulletPrefab){
						StartCoroutine(Attack());
					}
					return;
				}
			    Vector3 targetPoint = target.transform.position;
				targetPoint = new Vector3(targetPoint.x , targetPoint.y + towardHeight , targetPoint.z);
			     
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			    MoveTowardsTarget (targetPoint);
		}
				
    }
     
    void MoveTowardsTarget(Vector3 target) {
		    CharacterController controller = GetComponent<CharacterController>();
		    Vector3 offset = target - transform.position;
		    if(offset.magnitude > .1f) {
			    offset = offset.normalized * speed;
			    controller.Move(offset * Time.deltaTime);
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
		cancelAttack = false;
		Transform bulletShootout;
		Status stat = GetComponent<Status>();
		if(!stat.flinch || !GetComponent<Status>().freeze || !freeze || !attacking){
			freeze = true;
			attacking = true;
			if(attackAnimation){
				mainModel.GetComponent<Animation>().Play(attackAnimation.name);
			}
			if(attackVoice){
				GetComponent<AudioSource>().clip = attackVoice;
				GetComponent<AudioSource>().Play();
			}
			yield return new WaitForSeconds(attackCast);
		
			if(!cancelAttack){
				bulletShootout = Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
				//If The Enemy have 2 Attack Points. It will Spawn another Bullet
				if(attackPoint2){
					bulletShootout = Instantiate(bulletPrefab, attackPoint2.transform.position , attackPoint2.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , this.gameObject);
				}
				yield return new WaitForSeconds(attackDelay);
				freeze = false;
				attacking = false;
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				freeze = false;
				attacking = false;
			}

		}
		
	}
}