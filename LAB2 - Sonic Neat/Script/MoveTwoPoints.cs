using UnityEngine;
using System.Collections;

public class MoveTwoPoints : MonoBehaviour {

    public Transform target1;
	public Transform target2;
	private Transform target;
    public float speed = 4.0f;
	public bool lookAtTarget = false;
	
	void Start(){
		//Release Target Object from Parent
		target1.transform.parent = null;
		target2.transform.parent = null;
		//Set Target
		target = target1;
	}
	
    void FixedUpdate() {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
		if(transform.position == target.transform.position){
			//Set New Target
			if(target == target1){
				target = target2;
			}else{
				target = target1;	
			}
			if(lookAtTarget){
				transform.LookAt(target);
			}
		}
    }
	
}
