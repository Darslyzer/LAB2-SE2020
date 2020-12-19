using UnityEngine;
using System.Collections;

public class PlatformRaycast : MonoBehaviour {
	public float raycastRange = 0.5f;
	public float delay = 0.7f;
	private float wait = 0;
	private bool dd = false;

	// Update is called once per frame
	void Update(){
		CharacterMotor motor = transform.root.GetComponent<CharacterMotor>();
		if(motor.grounded){
			if(Input.GetAxis("Vertical") < -0.1 && Input.GetButton("Jump")){
				Physics.IgnoreLayerCollision(8 , 9 , true);
				dd = true;
				return;
			}
		}

		if(dd){
			if(wait >= delay){
				dd = false;
				wait = 0;
			}else{
				wait += Time.deltaTime;
			}
			return;
		}

		RaycastHit hit;
		
		if(Physics.Raycast(transform.position, Vector3.up, out hit , raycastRange)){
			if(hit.transform.gameObject.layer == 8) {
				Physics.IgnoreLayerCollision(8 , 9 , true);
			}
		}/*else{
			Physics.IgnoreLayerCollision(8 , 9 , false);
		}*/

		if(Physics.Raycast(transform.root.position, Vector3.down, out hit , raycastRange)){
			if(hit.transform.gameObject.layer == 8) {
				Physics.IgnoreLayerCollision(8 , 9 , false);
			}
		}
	}

}
