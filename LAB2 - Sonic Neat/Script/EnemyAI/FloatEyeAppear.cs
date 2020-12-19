using UnityEngine;
using System.Collections;

public class FloatEyeAppear : MonoBehaviour {
	public float duration = 1.0f;
	public Vector3 direction = Vector3.zero;
	// Use this for initialization
	void Start () {
		StartCoroutine(ActivateMonster());
	}
	
	// Update is called once per frame
	void Update () {
		CharacterController controller = GetComponent<CharacterController>();
		controller.Move(direction * 5 * Time.deltaTime);
	}
	
	IEnumerator ActivateMonster(){
		yield return new WaitForSeconds(duration);
		GetComponent<AIMoveToward>().enabled = true;
		enabled = false;
	}
}
