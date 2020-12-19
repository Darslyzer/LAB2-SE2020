using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BulletStatus))]

public class AbnormalStatAttack : MonoBehaviour {
	
	public enum AbStat{
		Poison = 0,
		Silence = 1,
		Stun = 2,
	}
	
	public AbStat inflictStatus = AbStat.Poison;
	
	public int chance = 100;
	public float statusDuration = 5.5f;
	
	private string shooterTag = "Player";
	
	void  Start (){
		shooterTag = GetComponent<BulletStatus>().shooterTag;
	}
	
	
	void  OnTriggerEnter ( Collider other  ){  	
		//When Player Shoot at Enemy		   
		if(shooterTag == "Player" && other.tag == "Enemy"){
			InflictAbnormalStats(other.gameObject);
			//When Enemy Shoot at Player
		}else if(shooterTag == "Enemy" && other.tag == "Player"){  	
			InflictAbnormalStats(other.gameObject);
		}
	}
	
	public void  InflictAbnormalStats ( GameObject target  ){
		if(chance > 0){
			int ran= Random.Range(0,100);
			if(ran <= chance){
				//Call Function ApplyAbnormalStat in Status Script
				target.GetComponent<Status>().ApplyAbnormalStat((int)inflictStatus ,statusDuration);
			}
		}
		
	}
	

}