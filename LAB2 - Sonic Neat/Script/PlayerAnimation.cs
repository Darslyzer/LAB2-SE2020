using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AttackTrigger))]

public class PlayerAnimation : MonoBehaviour {
	
	public float runMaxAnimationSpeed = 1.0f;
	public float sprintAnimationSpeed = 1.5f;
	
	private GameObject player;
	public GameObject mainModel;
	
	//string idle = "idle";
	public AnimationClip idle;
	public AnimationClip run;
	public AnimationClip jump;
	public AnimationClip hurt;
	public AnimationClip dash;
	public AnimationClip wallSlide;
	public AnimationClip wallKick;
	public AnimationClip jumpUp;

	private float moveHorizontal = 0.0f;
	
	void Start(){
		if(!player){
			player = this.gameObject;
		}
		mainModel = GetComponent<Status>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
		//mainModel.animation[jump.name].wrapMode  = WrapMode.ClampForever;
		if(jumpUp)
			mainModel.GetComponent<Animation>()[jumpUp.name].layer  = 3;
		//mainModel.animation[wallSlide.name].layer  = 3;
		//mainModel.animation[wallKick.name].layer  = 3;
		
		AnimLayer();
	}

	public void AnimLayer(){
		if(dash){
			mainModel.GetComponent<Animation>()[dash.name].layer = 5;
		}
		if(wallKick){
			mainModel.GetComponent<Animation>()[wallKick.name].layer = 4;
		}
		/*if(wallSlide){
			mainModel.animation[wallSlide.name].layer = 4;
		}*/
		if(hurt){
			mainModel.GetComponent<Animation>()[hurt.name].layer = 5;
		}
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
	}
	
	void Update(){
		/*if(GetComponent<Status>().freeze){
			mainModel.GetComponent<Animation>().CrossFade(idle.name);
			return;
		}*/
		CharacterController controller = player.GetComponent<CharacterController>();
		moveHorizontal = GetComponent<PlayerInputController>().moveHorizontal;
		if(GetComponent<PlayerInputController>().onWallSlide){
			mainModel.GetComponent<Animation>().Play(wallSlide.name);
			return;
		}
		if ((controller.collisionFlags & CollisionFlags.Below) != 0){
			//if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0){
			if (moveHorizontal != 0 && !player.GetComponent<Status>().freeze){
				//Play Move Animation
 	    		mainModel.GetComponent<Animation>().CrossFade(run.name , 0.15f);
		  	}else{
				//Play Idle Animation
				mainModel.GetComponent<Animation>().CrossFade(idle.name , 0.15f);
	      	}
			if(Input.GetButtonDown("Jump") && jumpUp && !GetComponent<Status>().freeze && Time.timeScale != 0.0f){
				mainModel.GetComponent<Animation>()[jumpUp.name].layer  = 3;
				//mainModel.animation.Play(jumpUp.name);
				mainModel.GetComponent<Animation>().PlayQueued(jumpUp.name, QueueMode.PlayNow);
			}
		}else{
				//Play Jump Animation
				mainModel.GetComponent<Animation>().CrossFade(jump.name);
		}
	}
	
	public void AnimationSpeedSet(){
		mainModel = GetComponent<AttackTrigger>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
	}
}

