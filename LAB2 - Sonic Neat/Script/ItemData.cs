using UnityEngine;
using System.Collections;

public class ItemData : MonoBehaviour {
	
	[System.Serializable]
	public class Usable {
		public string itemName = "";
		public Texture2D icon;
		public GameObject model;
		public string description = "";
		public int price = 10;
		public int hpRecover = 0;
		public int mpRecover = 0;
		public int atkPlus = 0;
		public int defPlus = 0;
		public int matkPlus = 0;
		public int mdefPlus = 0;
	} 
	[System.Serializable]
	public class Equip {
		public string itemName = "";
		public Texture2D icon;
		public GameObject model;
		public bool  assignAllWeapon = true;
		public string description = "";
		public int price = 10;
		public int attack = 5;
		public int defense = 0;
		public int magicAttack = 0;
		public int magicDefense = 0;
		
		public enum EqType {
			Weapon = 0,
			Armor = 1,
		}
		public EqType EquipmentType = EqType.Weapon;
		public int weaponType = 0; //Use for Mecanim
		
		//Ignore if the equipment type is not weapons
		public GameObject attackPrefab;
		public GameObject jumpAttackPrefab;
		public bool canCharge = false;
		public AnimationClip[] attackCombo = new AnimationClip[3];
		public AnimationClip movingAttackAnimation;
		public AnimationClip jumpAttackAnimation;
		public AnimationClip dashAttackAnimation;
		public AnimationClip onWallAttackAnimation;
		public AnimationClip idleAnimation;
		public AnimationClip runAnimation;
		public AnimationClip jumpAnimation;
		public AnimationClip dashAnimation;
		public AnimationClip wallSlideAnimation;
		public AnimationClip wallKickAnimation;
		public enum whileAtk{
			MeleeFwd = 0,
			Immobile = 1,
			WalkFree = 2
		}
		public whileAtk whileAttack = whileAtk.Immobile;
		
		[System.Serializable]
		public class ChargeAtkk {
			public GameObject chargeEffect;
			public Transform chargeAttackPrefab;
			public float chargeTime = 1.0f;
		}
		public ChargeAtkk[] charge = new ChargeAtkk[1];
		
		public float attackSpeed = 0.18f;
		public float attackDelay = 0.12f;
	} 
	
	
	public Usable[] usableItem = new Usable[3];
	public Equip[] equipment = new Equip[3];
	
}