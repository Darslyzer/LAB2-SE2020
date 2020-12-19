using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {
	
	public float delay = 3.0f;
	public GameObject playerRespawn;
	private bool  menu = false;
	private Vector3 lastPosition;
	private Transform mainCam;
	private GameObject oldPlayer;
	
	void Start(){
		StartCoroutine(Delay());
	}

	IEnumerator Delay(){
		yield return new WaitForSeconds(delay);
		menu = true;
	}
	
	void OnGUI(){
		if(menu){
			GUI.Box(new Rect(Screen.width /2 -100,Screen.height /2 -120,200,160), "Game Over");
			if(GUI.Button(new Rect(Screen.width /2 -80,Screen.height /2 -80,160,40), "Retry")) {
				LoadData();
			}
			if(GUI.Button(new Rect(Screen.width /2 -80,Screen.height /2 -20,160,40), "Quit Game")) {
				mainCam = GameObject.FindWithTag ("MainCamera").transform;
				Destroy(mainCam.gameObject); //Destroy Main Camera
				Application.LoadLevel ("Title");
				//Application.Quit();
			}
		}
	}
	
	void  LoadData (){
		oldPlayer = GameObject.FindWithTag ("Player");
		if(oldPlayer){
			Destroy(gameObject);
		}else{
			lastPosition.x = PlayerPrefs.GetFloat("PlayerX");
			lastPosition.y = PlayerPrefs.GetFloat("PlayerY");
			lastPosition.z = PlayerPrefs.GetFloat("PlayerZ");
			GameObject respawn = Instantiate(playerRespawn, lastPosition , transform.rotation) as GameObject;
			respawn.GetComponent<Status>().level = PlayerPrefs.GetInt("TempPlayerLevel");
			respawn.GetComponent<Status>().atk = PlayerPrefs.GetInt("TempPlayerATK");
			respawn.GetComponent<Status>().def = PlayerPrefs.GetInt("TempPlayerDEF");
			respawn.GetComponent<Status>().matk = PlayerPrefs.GetInt("TempPlayerMATK");
			respawn.GetComponent<Status>().mdef = PlayerPrefs.GetInt("TempPlayerMDEF");
			respawn.GetComponent<Status>().exp = PlayerPrefs.GetInt("TempPlayerEXP");
			respawn.GetComponent<Status>().maxExp = PlayerPrefs.GetInt("TempPlayerMaxEXP");
			respawn.GetComponent<Status>().maxHealth = PlayerPrefs.GetInt("TempPlayerMaxHP");
			respawn.GetComponent<Status>().health = PlayerPrefs.GetInt("TempPlayerMaxHP");
			respawn.GetComponent<Status>().maxMana = PlayerPrefs.GetInt("TempPlayerMaxMP");
			respawn.GetComponent<Status>().mana = PlayerPrefs.GetInt("TempPlayerMaxMP");	
			respawn.GetComponent<Status>().statusPoint = PlayerPrefs.GetInt("TempPlayerSTP");
			//-------------------------------
			respawn.GetComponent<Inventory>().cash = PlayerPrefs.GetInt("TempCash");
			int itemSize = respawn.GetComponent<Inventory>().itemSlot.Length;
			int a = 0;
			if(itemSize > 0){
				while(a < itemSize){
					respawn.GetComponent<Inventory>().itemSlot[a] = PlayerPrefs.GetInt("TempItem" + a.ToString());
					respawn.GetComponent<Inventory>().itemQuantity[a] = PlayerPrefs.GetInt("TempItemQty" + a.ToString());
					//-------
					a++;
				}
			}
			
			int equipSize = respawn.GetComponent<Inventory>().equipment.Length;
			a = 0;
			if(equipSize > 0){
				while(a < equipSize){
					respawn.GetComponent<Inventory>().equipment[a] = PlayerPrefs.GetInt("TempEquipm" + a.ToString());
					a++;
				}
			}
			respawn.GetComponent<Inventory>().weaponEquip = 0;
			respawn.GetComponent<Inventory>().armorEquip = PlayerPrefs.GetInt("TempArmoEquip");
			if(PlayerPrefs.GetInt("TempWeaEquip") == 0){
				respawn.GetComponent<Inventory>().RemoveWeaponMesh();
			}else{
				respawn.GetComponent<Inventory>().EquipItem(PlayerPrefs.GetInt("TempWeaEquip") , respawn.GetComponent<Inventory>().equipment.Length + 5);
			}
			//----------------------------------

			//Load Skill Slot
			a = 0;
			while(a <= 2){
				respawn.GetComponent<SkillWindow>().skill[a] = PlayerPrefs.GetInt("TempSkill" + a.ToString());
				a++;
			}
			//Skill List Slot
			a = 0;
			while(a < respawn.GetComponent<SkillWindow>().skillListSlot.Length){
				respawn.GetComponent<SkillWindow>().skillListSlot[a] = PlayerPrefs.GetInt("TempSkillList" + a.ToString());
				a++;
			}
			respawn.GetComponent<SkillWindow>().AssignAllSkill();
			DontDestroyOnload dst = respawn.GetComponent<DontDestroyOnload>();
			if(!dst){
				respawn.gameObject.AddComponent<DontDestroyOnload>();
			}
			
			Destroy(gameObject);
			//Application.LoadLevel(Application.loadedLevelName);
		}
	}
	
}