using UnityEngine;
using System.Collections;

public class DemoBoss : MonoBehaviour {
	public GameObject target;
	public bool SnapToPlayerX = true;
	[HideInInspector]
	public GameObject mainModel;
	public AnimationClip idleAnimation;
	
	[System.Serializable]
	public class bossMove{
		public GameObject attackPrefab;
		public GameObject attackPoint;
		public float castTime = 0.2f;
		public float delay = 1.0f;
		public GameObject castEffect;
		public AnimationClip attackAnimation;
		public AudioClip attackVoice;
	}
	public AudioClip hurtVoice;

	public bossMove[] moveSet = new bossMove[2];
	public float randomDelay = 1.0f;
	private float nextAttack = 0.0f;
	private GameObject[] gos;
	private int atk = 0;
	private int matk = 0;
	public bool  freeze = false;
	private GameObject eff;
	// Use this for initialization
	void Start () {
		if(hurtVoice){
			GetComponent<Status>().hurtVoice = hurtVoice;
		}
		atk = GetComponent<Status>().atk;
		matk = GetComponent<Status>().matk;
		target = GameObject.FindWithTag("Player");
		mainModel = GetComponent<Status>().mainModel;
		if(SnapToPlayerX){
			transform.position = new Vector3(target.transform.position.x , transform.position.y , transform.position.z);
		}
		nextAttack = Time.time + randomDelay;
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
		if(freeze){
			return;
		}
		
		if(Time.time > nextAttack ){
				RandomMoveSet();
		}
	}
	
	GameObject FindPlayer (){ 
		// Find Closest Player   
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
	
	void RandomMoveSet(){
		int ran = Random.Range(0 , moveSet.Length);
		if(ran <= moveSet.Length){
			StartCoroutine(Attack(ran));
		}
	}
	
	IEnumerator Attack(int moveId){
		GameObject bulletShootout;
		if(!freeze){
			freeze = true;
			if(moveSet[moveId].attackAnimation){
				mainModel.GetComponent<Animation>().Play(moveSet[moveId].attackAnimation.name);
			}
			if(moveSet[moveId].castEffect){
				//Spawn Cast Effect on Attack Point
				eff = Instantiate(moveSet[moveId].castEffect , moveSet[moveId].attackPoint.transform.position , moveSet[moveId].castEffect.transform.rotation) as GameObject;
				eff.transform.parent = this.transform;
			}
			yield return new WaitForSeconds(moveSet[moveId].castTime);

			if(moveSet[moveId].attackVoice){
				GetComponent<AudioSource>().clip = moveSet[moveId].attackVoice;
				GetComponent<AudioSource>().Play();
			}
		
			bulletShootout = Instantiate(moveSet[moveId].attackPrefab, moveSet[moveId].attackPoint.transform.position , moveSet[moveId].attackPoint.transform.rotation) as GameObject;
			bulletShootout.GetComponent<BulletStatus>().Setting(atk , matk , "Enemy" , moveSet[moveId].attackPoint);
			if(eff){
				//Destroy Cast Effect
				Destroy(eff.gameObject);
			}
			yield return new WaitForSeconds(moveSet[moveId].delay);
			freeze = false;
			mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
			nextAttack = Time.time + randomDelay;
		}
	}
}
