using UnityEngine;
using System.Collections;

public class PlatformerCamera : MonoBehaviour {
/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	public float cameraHeight = 2.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;

	[HideInInspector]
	public float shakeValue = 0.0f;
	[HideInInspector]
	public bool onShaking = false;
	private float shakingv = 0.0f;
	//public bool lockOn = false;
	public bool lockHeight = false;

	public bool onLimit = false;
	public bool onSizeLimit = false;
	public float yLimitMin = 0;
	public float yLimitMax = 0;
	public float zLimitMin = 0;
	public float zLimitMax = 0;
	//private float heightLock = 0.0f;

	void Start(){
		if(!target){
			target = GameObject.FindWithTag("Player").transform;
		}
	}
		
	void  LateUpdate (){
		// Early out if we don't have a target
		if (!target)
			return;
		if(lockHeight){
			float z = Mathf.Lerp(transform.position.z, target.position.z, 0.75f);
			//float y = Mathf.Lerp(transform.position.y, target.position.y, 0.75f);
			transform.position = new Vector3(transform.position.x , transform.position.y , z);
			return;
		}
			
		float wantedHeight= target.position.y + height;
			
		float currentRotationAngle= transform.eulerAngles.y;
		float currentHeight= transform.position.y;
		
		// Damp the height
		//currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		currentHeight = wantedHeight;

		// Convert the angle into a rotation
		Quaternion currentRotation= Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3 (transform.position.x , currentHeight , transform.position.z);
		
		// Always look at the target
		Vector3 lookOn = new Vector3(target.position.x , target.position.y + cameraHeight , target.position.z);
		transform.LookAt (lookOn);

		if(onLimit){
			transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, yLimitMin, yLimitMax), transform.position.z);
		}
		if(onSizeLimit){
			transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, zLimitMin, zLimitMax));
		}
		//transform.LookAt (target);
		//Quaternion rotation = Quaternion.Euler(0, 0, 0);
		if(onShaking){
			shakeValue = Random.Range(-shakingv , shakingv)* 0.2f;
			transform.position += new Vector3(0,shakeValue,0);
		}
	}

	public void Shake(float val , float dur){
		if(onShaking){
			return;
		}
		shakingv = val;
		StartCoroutine(Shaking(dur));
	}
	
	public IEnumerator Shaking(float dur){
		onShaking = true;
		yield return new WaitForSeconds(dur);
		shakingv = 0;
		shakeValue = 0;
		onShaking = false;
	}
	
	public void SetNewTarget(Transform p){
		target = p;
	}

	public void SetLockHeight(Transform pos){
		//StopCoroutine("ReleaseLock");
		lockHeight = true;
		transform.position = new Vector3 (transform.position.x , pos.position.y , transform.position.z); 
	}
	public void CancelLock(){
		lockHeight = false;
	}
	public void SetHeigtLimit(Transform min , Transform max){
		yLimitMin = min.position.y;
		yLimitMax = max.position.y;
		onLimit = true;
	}
	public void SetSizeLimit(Transform min , Transform max){
		zLimitMin = min.position.z;
		zLimitMax = max.position.z;
		onSizeLimit = true;
	}
	
}
