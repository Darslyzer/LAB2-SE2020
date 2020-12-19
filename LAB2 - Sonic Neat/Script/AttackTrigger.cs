using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Status))]
[RequireComponent (typeof (StatusWindow))]
[RequireComponent (typeof (HealthBar))]
//[RequireComponent (typeof (PlayerAnimation))]
[RequireComponent (typeof (PlayerInputController))]
[RequireComponent (typeof (CharacterMotor))]
[RequireComponent (typeof (Inventory))]
[RequireComponent (typeof (SkillWindow))]

public class AttackTrigger : MonoBehaviour {
	public GameObject mainModel;
	//public bool useMecanim = false;
	public Transform attackPoint; //The Point where you spawn bullet while attack
	public Transform lowerAttackPoint; //The Point where you spawn bullet while dash
	public Transform onWallAttackPoint; //The Point where you spawn bullet while you are on the wall
	public bool canCharge = false;
	public Transform attackPrefab;
	public Transform jumpAttackPrefab;
	public enum whileAtk{
		MeleeFwd = 0,
		Immobile = 1,
		WalkFree = 2
	}
	public whileAtk whileAttack = whileAtk.MeleeFwd;
	
	//Skill Animation
	[System.Serializable]
	public class ChargeAtk{
		public GameObject chargeEffect;
		public Transform chargeAttackPrefab;
		public float chargeTime = 1.0f;
		[HideInInspector]
		public float currentChargeTime = 1.0f;
	}
	public ChargeAtk[] charge = new ChargeAtk[1];
	[HideInInspector]
	public bool charging = false;
	[HideInInspector]
	public GameObject chargingEffect;

	[HideInInspector]
	public bool atkDelay = false;
	public bool  freeze = false;
	
	public float attackSpeed = 0.15f;
	[HideInInspector]
	public float nextFire = 0.0f;
	public float atkDelay1 = 0.1f;
	
	//Attacking Animation
	public AnimationClip[] attackCombo = new AnimationClip[3];
	public AnimationClip movingAttackAnimation;
	public AnimationClip jumpAttackAnimation;
	public AnimationClip dashAttackAnimation;
	public AnimationClip onWallAttackAnimation;
	public float attackAnimationSpeed = 1.0f;
	
	//Skill Animation
	[System.Serializable]
	public class SkilAtk {
		public Texture2D icon;
		public Transform skillPrefab;
		public AnimationClip skillAnimation;
		public AnimationClip jumpSkillAnimation;
		public AnimationClip onWallSkillAnimation;
		//public float skillAnimationSpeed = 1.0f;
		public float castTime = 0.3f;
		public float skillDelay = 0.3f;
		public int manaCost = 10;
	}
	public SkilAtk[] skill = new SkilAtk[3];
	public Texture2D skillSelectIcon;
	
	private bool  meleefwd = false;
	[HideInInspector]
	public bool  isCasting = false;

	[HideInInspector]
	public int c = 0;
	[HideInInspector]
	public int conCombo = 0;
	
	public Transform Maincam;
	public GameObject MaincamPrefab;
	
	private int str = 0;
	private int matk = 0;
	[HideInInspector]
	public int ch;

	private int skillEquip  = 0;

	//----------Sounds-------------
	[System.Serializable]
	public class AtkSound {
		public AudioClip[] attackComboVoice = new AudioClip[3];
		public AudioClip magicCastVoice;
		public AudioClip hurtVoice;
	}
	public AtkSound sound;
	private bool onMobile = false;
	
	void  Awake (){
		gameObject.tag = "Player";
		//GetComponent<Status>().useMecanim = useMecanim;
		if(MaincamPrefab){
			//Destroy old Camera in the scene
			GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera"); 
			foreach(GameObject cam2 in cam) { 
				if(cam2){
					Destroy(cam2.gameObject);
				}
			}
			//Spawn New Camera from MaincamPrefab
			GameObject newCam = GameObject.FindWithTag ("MainCamera");
			newCam = Instantiate(MaincamPrefab, transform.position , MaincamPrefab.transform.rotation) as GameObject;
			Maincam = newCam.transform;
			Maincam.GetComponent<PlatformerCamera>().target = this.transform;
		}
		//Assign Hurt Voice to Status
		if(sound.hurtVoice){
			GetComponent<Status>().hurtVoice = sound.hurtVoice;
		}

		str = GetComponent<Status>().addAtk;
		matk = GetComponent<Status>().addMatk;
		
		//--------------------------------
		//Spawn new Attack Point if you didn't assign it.
		if(!attackPoint){
			GameObject newAtkPoint = new GameObject();
			newAtkPoint.transform.position = this.transform.position;
			newAtkPoint.transform.rotation = this.transform.rotation;
			newAtkPoint.transform.parent = this.transform;
			attackPoint = newAtkPoint.transform;	
		}
		if(!lowerAttackPoint){
			lowerAttackPoint = attackPoint;
		}
		if(!onWallAttackPoint){
			onWallAttackPoint = attackPoint;
		}
		if(!jumpAttackPrefab){
			jumpAttackPrefab = attackPrefab;
		}
		GetComponent<Status>().hurt = GetComponent<PlayerAnimation>().hurt;
		//Check for Main Model from Status Script
		if(!mainModel && !GetComponent<Status>().mainModel){
			mainModel = this.gameObject;
		}else if(!mainModel){
			mainModel = GetComponent<Status>().mainModel;
		}
		//if(!useMecanim){
			//Set All Attack Combo Animation'sLayer to 15
			int animationSize = attackCombo.Length;
			int a = 0;
			if(animationSize > 0){
				while(a < animationSize && attackCombo[a]){
					mainModel.GetComponent<Animation>()[attackCombo[a].name].layer = 15;
					a++;
				}
			}

			if(jumpAttackAnimation){
				mainModel.GetComponent<Animation>()[jumpAttackAnimation.name].layer = 15;
			}else if(attackCombo[0]){
				jumpAttackAnimation = attackCombo[0];
			}
			if(dashAttackAnimation){
				mainModel.GetComponent<Animation>()[dashAttackAnimation.name].layer = 15;
			}
			if(onWallAttackAnimation){
				mainModel.GetComponent<Animation>()[onWallAttackAnimation.name].layer = 15;
			}else if(attackCombo[0]){
				onWallAttackAnimation = attackCombo[0];
			}
			//Set Moving Attack Animation'sLayer to 14
			if(movingAttackAnimation){
				mainModel.GetComponent<Animation>()[movingAttackAnimation.name].layer = 14;
			}else if(attackCombo[0]){
				movingAttackAnimation = attackCombo[0];
			}

		//CheckPlatform();
		onMobile = true;
		//}

	}

	void  CheckPlatform (){
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
			onMobile = true;
		}else{
			onMobile = false;
		}
		
	}
	
	
	void  Update (){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();
		if(freeze || atkDelay || Time.timeScale == 0.0f || stat.freeze){
			//Cancel Charge
			if(Input.GetButtonUp("Fire1") && charging && !onMobile || Input.GetKeyUp("j") && charging){
				charging = false;
				if(chargingEffect){
					Destroy(chargingEffect.gameObject);
				}
			}
			return;
		}

			if (stat.flinch){
				Vector3 lui = transform.TransformDirection(Vector3.back);
				controller.Move(lui * 6* Time.deltaTime);
				//Cancel Charge When Get Button Up
			if(Input.GetButtonUp("Fire1") && charging && !onMobile || Input.GetKeyUp("j") && charging){
					charging = false;
					if(chargingEffect){
						Destroy(chargingEffect.gameObject);
					}
				}
				return;
			}
			//Move Player forward a little while Melee Dash
			if (meleefwd){
				Vector3 lui = transform.TransformDirection(Vector3.forward);
				controller.Move(lui * 3 * Time.deltaTime);
			}
			//----------------------------
			//When press attack Button
		if (Input.GetButton("Fire1") && Time.time > nextFire && !isCasting && !charging && !onMobile || Input.GetKey("j") && Time.time > nextFire && !isCasting && !charging) {
				if(Time.time > (nextFire + 0.5f)){
					c = 0;
				}
				//Attack Combo
				if(attackCombo.Length >= 1){
					//conCombo++;
					if(controller.collisionFlags == CollisionFlags.None){
						StartCoroutine(AttackCombo(jumpAttackPrefab));
					}else{
						StartCoroutine(AttackCombo(attackPrefab));
						//TriggerAttack(attackPrefab);
					}

				}
				//Charging Weapon if the Weapon can charge and player hold the Attack Button
				if(canCharge && !charging && Time.time > nextFire /2){
						charging = true;
						int b = charge.Length -1;
						while(b >= 0){
							charge[b].currentChargeTime = Time.time + charge[b].chargeTime;
							b--;
						}
				}
			}
			
			//Charging Effect
		if(Input.GetButton("Fire1") && charging && !onMobile || Input.GetKey("j") && charging) {
				int b = charge.Length -1;
				while(b >= 0){
						if(Time.time > charge[b].currentChargeTime){
							if(charge[b].chargeEffect && chargingEffect != charge[b].chargeEffect){
							
								if(!chargingEffect || ch != b){
									if(chargingEffect){
										Destroy(chargingEffect.gameObject);
									}
									chargingEffect = Instantiate(charge[b].chargeEffect , transform.position, transform.rotation) as GameObject;
									chargingEffect.transform.parent = this.transform;
									ch = b;
								}
							}
							b = -1;
						}else{
							b--;
						}
					}

			}
			
			//Release Charging
		if(Input.GetButtonUp("Fire1") && charging && !onMobile || Input.GetKeyUp("j") && charging){
					charging = false;
					int b = charge.Length -1;
					if(chargingEffect){
						Destroy(chargingEffect.gameObject);
					}
					while(b >= 0){
						if(Time.time > charge[b].currentChargeTime){
							//Charge Shot!!
							if(Time.time > (nextFire + 0.5f)){
								c = 0;
							}
							//conCombo = 1;
							StartCoroutine(AttackCombo(charge[b].chargeAttackPrefab));
							b = -1;
						}else{
							b--;
						}
					}
			}
		if(charging && !Input.GetKey("j") && !Input.GetButton("Fire1") && !onMobile){
				charging = false;
			}
			//Magic
		if (Input.GetButtonDown("Fire2") && Time.time > nextFire && !isCasting && skill[skillEquip].skillPrefab && !stat.silence && !onMobile || Input.GetKeyDown("i") && Time.time > nextFire && !isCasting && skill[skillEquip].skillPrefab && !stat.silence) {
				//MagicSkill(skillEquip);
				//StartCoroutine(MagicSkill(skillEquip));
				TriggerSkill(skillEquip);
			}
			if(Input.GetKeyDown("1") && !isCasting && skill[0].skillPrefab){
				skillEquip = 0;
			}
			if(Input.GetKeyDown("2") && !isCasting && skill[1].skillPrefab){
				skillEquip = 1;
			}
			if(Input.GetKeyDown("3") && !isCasting && skill[2].skillPrefab){
				skillEquip = 2;
			}
		
		//Stop Stand Attack Animation While Moving
		if(movingAttackAnimation && whileAttack == whileAtk.WalkFree && isCasting){
			//if(Input.GetButton("Horizontal") || controller.collisionFlags == CollisionFlags.None && jumpAttackAnimation || GetComponent<PlayerInputController>().dashing && dashAttackAnimation){
			if(GetComponent<PlayerInputController>().moveHorizontal != 0 || controller.collisionFlags == CollisionFlags.None && jumpAttackAnimation || GetComponent<PlayerInputController>().dashing && dashAttackAnimation){
				mainModel.GetComponent<Animation>().Stop(attackCombo[c].name);
			}
		}
		//Stop Dashing Animation
		if(dashAttackAnimation && !GetComponent<PlayerInputController>().dashing){
			mainModel.GetComponent<Animation>().Stop(dashAttackAnimation.name);
		}
		
	}
	void OnGUI(){
		//-------------------------------------------
		if(skill[0].skillPrefab && skill[0].icon){
			GUI.DrawTexture ( new Rect(50,Screen.height -120, 60 , 60), skill[0].icon);
		}
		if(skillEquip == 0 && skill[skillEquip].skillPrefab && skillSelectIcon){
			GUI.DrawTexture ( new Rect(50,Screen.height -120, 60 , 60), skillSelectIcon);
		}
		//-------------------------------------------
		if(skill[1].skillPrefab && skill[1].icon){
			GUI.DrawTexture ( new Rect(110,Screen.height -120, 60 , 60), skill[1].icon);
		}
		if(skillEquip == 1 && skill[skillEquip].skillPrefab && skillSelectIcon){
			GUI.DrawTexture ( new Rect(110,Screen.height -120, 60 , 60), skillSelectIcon);
		}
		//-------------------------------------------
		if(skill[2].skillPrefab && skill[2].icon){
			GUI.DrawTexture ( new Rect(170 ,Screen.height -120, 60 , 60), skill[2].icon);
		}
		if(skillEquip == 2 && skill[skillEquip].skillPrefab && skillSelectIcon){
			GUI.DrawTexture ( new Rect(170,Screen.height -120, 60 , 60), skillSelectIcon);
		}
		//-------------------------------------------
	}
	

	public IEnumerator AttackCombo(Transform atkBullet){
		if(attackCombo[c]){
			str = GetComponent<Status>().addAtk;
			matk = GetComponent<Status>().addMatk;
			Transform bulletShootout;
			isCasting = true;
			CharacterController controller = GetComponent<CharacterController>();
			// If Melee Dash
			if(whileAttack == whileAtk.MeleeFwd && !GetComponent<PlayerInputController>().onWallSlide){
				GetComponent<CharacterMotor>().canControl = false;
				//MeleeDash();
				StartCoroutine(MeleeDash());
			}
			// If Immobile
			if(whileAttack == whileAtk.Immobile && !GetComponent<PlayerInputController>().onWallSlide && controller.collisionFlags != CollisionFlags.None){
				GetComponent<CharacterMotor>().canControl = false;
			}
			if(sound.attackComboVoice.Length > c && sound.attackComboVoice[c]){
				GetComponent<AudioSource>().clip = sound.attackComboVoice[c];
				GetComponent<AudioSource>().Play();
			}

			float wait = 0.0f;
			//print (conCombo);
			//while(conCombo > 0){
				//if(!useMecanim){
					if(GetComponent<PlayerInputController>().onWallSlide){
						//Play Attack Animation while player on the wall
						mainModel.GetComponent<Animation>().PlayQueued(onWallAttackAnimation.name, QueueMode.PlayNow).speed = attackAnimationSpeed;
					}else if(controller.collisionFlags == CollisionFlags.None && jumpAttackAnimation){
						//Play Attack Animation while player in Mid Air
						mainModel.GetComponent<Animation>().PlayQueued(jumpAttackAnimation.name, QueueMode.PlayNow).speed = attackAnimationSpeed;
					}else if(GetComponent<PlayerInputController>().dashing && dashAttackAnimation){
						//Play Attack Animation while player dashing
						mainModel.GetComponent<Animation>().PlayQueued(dashAttackAnimation.name, QueueMode.PlayNow).speed = attackAnimationSpeed;
					}else{
						//Play Attack Animation while player standing
						if(c >= 1){
							mainModel.GetComponent<Animation>().PlayQueued(attackCombo[c].name, QueueMode.PlayNow).speed = attackAnimationSpeed;
						}else{
							mainModel.GetComponent<Animation>().PlayQueued(attackCombo[c].name, QueueMode.PlayNow).speed = attackAnimationSpeed;
						}
						if(movingAttackAnimation && whileAttack == whileAtk.WalkFree){
							mainModel.GetComponent<Animation>().PlayQueued(movingAttackAnimation.name, QueueMode.PlayNow).speed = attackAnimationSpeed;
						}
					}
					
					wait = mainModel.GetComponent<Animation>()[attackCombo[c].name].length -0.02f;
				//}
				
				yield return new WaitForSeconds(atkDelay1);
				if(controller.collisionFlags != CollisionFlags.None){
					c++;
				}
				
				nextFire = Time.time + attackSpeed;
				if(GetComponent<PlayerInputController>().dashing){
					//Spawn Bullet in Lower Attack Point (While Dashing)
					bulletShootout = Instantiate(atkBullet, lowerAttackPoint.transform.position , lowerAttackPoint.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}else if(GetComponent<PlayerInputController>().onWallSlide){
					//Spawn Bullet in On Wall Attack Point
					bulletShootout = Instantiate(atkBullet, onWallAttackPoint.transform.position , onWallAttackPoint.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}else{
					//Spawn Bullet in Attack Point
					bulletShootout = Instantiate(atkBullet, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}
				
				if(c >= attackCombo.Length){
					c = 0;
					atkDelay = true;
					yield return new WaitForSeconds(wait);
					atkDelay = false;
				}else{
					yield return new WaitForSeconds(attackSpeed);
				}
				
			//}
			
			isCasting = false;
			GetComponent<CharacterMotor>().canControl = true;
		} else {
			print ("Please assign attack animation in Attack Combo");
		}

	}

	
	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
		
	}
	
	//---------------------
	IEnumerator MagicSkill(int skillID){
		if(skill[skillID].skillAnimation){
			str = GetComponent<Status>().addAtk;
			matk = GetComponent<Status>().addMatk;
			CharacterController controller = GetComponent<CharacterController>();
			if(GetComponent<Status>().mana >= skill[skillID].manaCost && !GetComponent<Status>().silence){
				if(sound.magicCastVoice){
					GetComponent<AudioSource>().clip = sound.magicCastVoice;
					GetComponent<AudioSource>().Play();
				}
				isCasting = true;
				if(!GetComponent<PlayerInputController>().onWallSlide){
					GetComponent<CharacterMotor>().canControl = false;
				}

				//if(!useMecanim){
					if(GetComponent<PlayerInputController>().onWallSlide){
						//Play Skill Animation while player on the wall
						mainModel.GetComponent<Animation>()[skill[skillID].onWallSkillAnimation.name].layer = 14;
						mainModel.GetComponent<Animation>().Play(skill[skillID].onWallSkillAnimation.name);
					}else if(controller.collisionFlags == CollisionFlags.None && jumpAttackAnimation){
						//Play Skill Animation while player in Mid Air
						mainModel.GetComponent<Animation>()[skill[skillID].jumpSkillAnimation.name].layer = 14;
						mainModel.GetComponent<Animation>().Play(skill[skillID].jumpSkillAnimation.name);
					}else{
						//Play Skill Animation while on the ground
						mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].layer = 16;
						mainModel.GetComponent<Animation>().Play(skill[skillID].skillAnimation.name);
					}
				//}
				
				nextFire = Time.time + skill[skillID].skillDelay;
				//Transform bulletShootout;
				yield return new WaitForSeconds(skill[skillID].castTime);
				if(GetComponent<PlayerInputController>().onWallSlide){
					//Spawn Bullet in On Wall Attack Point
					Transform bulletShootout = Instantiate(skill[skillID].skillPrefab, onWallAttackPoint.transform.position , onWallAttackPoint.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}else{
					Transform bulletShootout = Instantiate(skill[skillID].skillPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
					bulletShootout.GetComponent<BulletStatus>().Setting(str , matk , "Player" , this.gameObject);
				}
				
				yield return new WaitForSeconds(skill[skillID].skillDelay);
				isCasting = false;
				GetComponent<CharacterMotor>().canControl = true;
				GetComponent<Status>().mana -= skill[skillID].manaCost;
			}
		}else{
			print("Please assign skill animation in Skill Animation");
		}

	}

	public void TriggerAttack(Transform atkPrefab){
		if(freeze || atkDelay || Time.timeScale == 0.0f || GetComponent<Status>().freeze){
			return;
		}
		StartCoroutine(AttackCombo(atkPrefab));
	}

	public void TriggerSkill(int sk){
		if(freeze || atkDelay || Time.timeScale == 0.0f || GetComponent<Status>().freeze){
			return;
		}
		if (Time.time > nextFire && !isCasting && skill[sk].skillPrefab) {
			StartCoroutine(MagicSkill(sk));
		}
		
	}
		
	public void WhileAttackSet(int watk){
		if(watk == 2) {
			whileAttack = whileAtk.WalkFree;
		}else if(watk == 1) {
			whileAttack = whileAtk.Immobile;
		}else{
			whileAttack = whileAtk.MeleeFwd;
		}
	}
	
	public void ResetWeapon(int ch){
		charging = false;
		if(chargingEffect){
			Destroy(chargingEffect.gameObject);
		}
		charge = new ChargeAtk[ch];
	}
			
}
