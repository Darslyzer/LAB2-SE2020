using UnityEngine;
using System.Collections;

public class SpineLookatMouse : MonoBehaviour {
	public Transform spine;
	public float sensitivityZ = 15.0f;
	public float minimumZ = -60.0f;
	public float maximumZ = 60.0f;
	private float rotationZ = 0.0f;

	public GameObject particle;

	void  LateUpdate (){
		rotationZ += Input.GetAxis("Mouse Y") * sensitivityZ;
		//rotationZ = Input.mousePosition.y;
		rotationZ = Mathf.Clamp (rotationZ, minimumZ, maximumZ);
		spine.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y,rotationZ);
	}


	/*void Update() {
		float mousex = (Input.mousePosition.x);
		float mousey = (Input.mousePosition.y);
		Vector3 mouseposition = Camera.main.ScreenToWorldPoint(new Vector3 (mousex , mousey ,0));
		Vector3 mouseposition2 = new Vector3 (transform.position.x , mouseposition.y ,transform.position.z);
		//Vector3 mouseposition = new Vector3 (transform.position.x , mousey ,transform.position.z);
		particle.transform.position = mouseposition2;

	}*/

}
