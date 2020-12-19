using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	public Texture2D maxHpBar;
	public Texture2D hpBar;
	public Texture2D mpBar;
	public Texture2D expBar;
	public Vector2 maxHpBarPosition = new Vector2(20,20);
	public int maxHpBarWidth = 420;
	public int maxHpBarHeigh = 150;
	public Vector2 hpBarPosition = new Vector2(161,85);
	public Vector2 mpBarPosition = new Vector2(161,114);
	public Vector2 expBarPosition = new Vector2(146,142);
	public Vector2 levelPosition = new Vector2(276,41);
	public int barHeight = 13;
	public int expBarHeight = 7;
	public GUIStyle levelTextStyle;
	public GUIStyle hpTextStyle;
	
	public float barMultiply = 2.7f;
	
	public GameObject player;
	private float hptext = 100;
	
	void  Awake (){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
		hptext = 100 * barMultiply;
	}
	
	void  OnGUI (){
		if(!player){
			return;
		}
		float maxHp = player.GetComponent<Status>().maxHealth;
		float hp = player.GetComponent<Status>().health * 100 / maxHp *barMultiply;
		float maxMp = player.GetComponent<Status>().maxMana;
		float mp = player.GetComponent<Status>().mana * 100 / maxMp *barMultiply;
		float maxExp = player.GetComponent<Status>().maxExp;
		float exp = player.GetComponent<Status>().exp * 100 / maxExp *barMultiply;
		float lv = player.GetComponent<Status>().level;
		
		int currentHp = player.GetComponent<Status>().health;
		int currentMp = player.GetComponent<Status>().mana;
		
		GUI.DrawTexture( new Rect(maxHpBarPosition.x ,maxHpBarPosition.y ,maxHpBarWidth,maxHpBarHeigh), maxHpBar);
		GUI.DrawTexture( new Rect(hpBarPosition.x ,hpBarPosition.y ,hp,barHeight), hpBar);
		GUI.DrawTexture( new Rect(mpBarPosition.x ,mpBarPosition.y ,mp,barHeight), mpBar);
		GUI.DrawTexture( new Rect(expBarPosition.x ,expBarPosition.y ,exp,expBarHeight), expBar);
		
		GUI.Label ( new Rect(levelPosition.x, levelPosition.y, 50, 50), lv.ToString() , levelTextStyle);
		GUI.Label ( new Rect(hpBarPosition.x, hpBarPosition.y, hptext, barHeight), currentHp.ToString() + "/" + maxHp.ToString() , hpTextStyle);
		GUI.Label ( new Rect(mpBarPosition.x, mpBarPosition.y, hptext, barHeight), currentMp.ToString() + "/" + maxMp.ToString() , hpTextStyle);
	}
	
}
