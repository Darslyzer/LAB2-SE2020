using UnityEngine;
using System.Collections;

public class BulletStatus : MonoBehaviour {
	
	public int damage = 10;
	public int damageMax = 20;
	
	private int playerAttack = 5;
	public int totalDamage = 0;
	public int variance = 15;
	public string shooterTag = "Player";
	[HideInInspector]
	public GameObject shooter;
	
	public Transform Popup;
	
	public GameObject hitEffect;
	public bool  flinch = false;
	public bool  penetrate = false;
	private string popDamage = "";

	public enum AtkType {
		Physic = 0,
		Magic = 1,
	}
	
	public AtkType AttackType = AtkType.Physic;

	public enum Elementala{
		Normal = 0,
		Fire = 1,
		Ice = 2,
		Earth = 3,
		Lightning = 4,
	}
	public Elementala element = Elementala.Normal;
	
	void Start(){
		if(variance >= 100){
			variance = 100;
		}
		if(variance <= 1){
			variance = 1;
		}

	}
	
	public void Setting(int str , int mag , string tag  , GameObject owner){
		if(AttackType == AtkType.Physic){
			playerAttack = str;
		}else{
			playerAttack = mag;
		}
		shooterTag = tag;
		shooter = owner;
		int varMin = 100 - variance;
		int varMax = 100 + variance;
		int randomDmg = Random.Range(damage, damageMax);
		totalDamage = (randomDmg + playerAttack) * Random.Range(varMin ,varMax) / 100;
	}

	
	void OnTriggerEnter(Collider other){  	
		//When Player Shoot at Enemy
		if(shooterTag == "Player" && other.tag == "Enemy"){	  
			Transform dmgPop = Instantiate(Popup, other.transform.position , transform.rotation) as Transform;
			
			if(AttackType == AtkType.Physic){
				//If Attack Type = Physic Call OnDamage Function in Status Script
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element);
			}else{
				//If Attack Type = Magic Call OnMagicDamage Function in Status Script
				popDamage = other.GetComponent<Status>().OnMagicDamage(totalDamage , (int)element);
			}
			dmgPop.GetComponent<DamagePopup>().damage = popDamage.ToString();	
			
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(flinch){
				other.GetComponent<Status>().Flinch();
			}
			if(!penetrate){
				//Destroy this object if it not Penetrate
				Destroy (gameObject);
			}
			//When Enemy Shoot at Player
		}else if(shooterTag == "Enemy" && other.tag == "Player"){
			if(other.GetComponent<Status>().mercyInvis){
				return;
			}
			if(AttackType == AtkType.Physic){
				//If Attack Type = Physic Call OnDamage Function in Status Script
				popDamage = other.GetComponent<Status>().OnDamage(totalDamage , (int)element);
			}else{
				//If Attack Type = Magic Call OnMagicDamage Function in Status Script
				popDamage = other.GetComponent<Status>().OnMagicDamage(totalDamage , (int)element);
			}
			Transform dmgPop = Instantiate(Popup, transform.position , transform.rotation) as Transform;	
			dmgPop.GetComponent<DamagePopup>().damage = popDamage.ToString();
			
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			if(flinch){
				other.GetComponent<Status>().Flinch();
			}
			if(!penetrate){
				//Destroy this object if it not Penetrate
				Destroy (gameObject);
			}
		}else if(shooterTag == "Player" && other.tag == "Guard"){
			//When Shoot at Guard tag object will do not deal damage and destroy this obkect
			Transform dmgPop = Instantiate(Popup, transform.position , transform.rotation) as Transform;	
			dmgPop.GetComponent<DamagePopup>().damage = "Guard";
			if(hitEffect){
				Instantiate(hitEffect, transform.position , transform.rotation);
			}
			Destroy (gameObject);
		}
	}
}