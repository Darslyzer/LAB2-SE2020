using UnityEngine;
using System.Collections;

public class BossHealthBar : MonoBehaviour {
	public GameObject boss;
	public Texture2D maxHpBar;
	public Texture2D hpBar;
	public Vector2 maxHpBarPosition = new Vector2(5,5);
	public int maxHpBarWidth = 380;
	public int maxHpBarHeigh = 110;
	public Vector2 hpBarPosition = new Vector2(13,49);
	public int barHeight = 13;
	public float barMultiply = 2.7f;
	private float hptext = 100;
	// Use this for initialization
	void Start () {
		if(!boss){
			boss = this.gameObject;
		}
		hptext = 100 * barMultiply;
	}
	
	// Update is called once per frame
	void OnGUI () {
		if(!boss){
			return;
		}
		float maxHp = boss.GetComponent<Status>().maxHealth;
		float hp = boss.GetComponent<Status>().health * 100 / maxHp *barMultiply;
		
		GUI.DrawTexture( new Rect(Screen.width - maxHpBarWidth - maxHpBarPosition.x , Screen.height - maxHpBarHeigh -maxHpBarPosition.y ,maxHpBarWidth,maxHpBarHeigh), maxHpBar);
		GUI.DrawTexture( new Rect(Screen.width - hptext - hpBarPosition.x , Screen.height - barHeight - hpBarPosition.y ,hp,barHeight), hpBar);
	}
}
