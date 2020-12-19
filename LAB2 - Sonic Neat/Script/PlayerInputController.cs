using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]

public class PlayerInputController : MonoBehaviour {
	public GameObject damageTaker;
	private CharacterMotor motor;
	private PlayerAnimation anim;
	public float walkSpeed = 6.0f;
	public float dashSpeed = 12.0f;
	public float dashDuration = 0.5f;
	public GameObject mainModel;
	public bool groundDash = true;
	public bool airDash = false;
	public bool doubleJump = false;
	public bool wallKick = false;
	public GameObject dashEffect;
	public GameObject wallKickEffect;
	public GameObject wallSlideEffect;
	
	[HideInInspector]
	public bool onJumping = false;
	[HideInInspector]
	public bool dashing = false;
	[HideInInspector]
	public bool jumpingDash = false;
	private bool jumpingDown = false;
	[HideInInspector]
	public bool onWallSlide = false;
	private bool airJump = false;
	[HideInInspector]
	public bool airMove = false;
	[HideInInspector]
	public bool airMove2 = false;
	[HideInInspector]
	public bool onWallKick = false;
	[HideInInspector]
	public float originalX = 0.0f;
	private float gravity = 20.0f;
	
	public bool underWater = false;
	
	[HideInInspector]
	public float moveHorizontal = 0.0f;
	
	//----------Sounds-------------
	[System.Serializable]
	public class MovementSound {
		public AudioClip jumpVoice;
		public AudioClip walkingSound;
		public AudioClip dashVoice;
	}
	public MovementSound sound;
	private bool onMobile = false;
	private Vector3 wk;
	private bool jumpingDashHold = false;
	
	// Use this for initialization
	void Start (){
		motor = GetComponent<CharacterMotor>();
		anim = GetComponent<PlayerAnimation>();
		mainModel = GetComponent<Status>().mainModel;
		gravity = motor.movement.gravity;
		originalX = transform.position.x;
		if(wallSlideEffect){
			wallSlideEffect.SetActive(false);
		}
		if(dashEffect){
			dashEffect.SetActive(false);
		}
		CheckPlatform ();
	}
	
	void CheckPlatform(){
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
			onMobile = true;
		}else{
			onMobile = false;
		}
		
	}
	
	// Update is called once per frame
	void Update (){
		Status stat = GetComponent<Status>();
		if(!onMobile){
			//moveHorizontal = Input.GetAxis("Horizontal");
			moveHorizontal = Input.GetAxisRaw("Horizontal");
		}
		if(jumpingDashHold){
			if(Input.GetButtonUp("Jump")){
				jumpingDashHold = false;
			}
		}
		if(transform.position.x != originalX){
			transform.position = new Vector3(originalX , transform.position.y , transform.position.z);
		}

		if(stat.freeze || stat.flinch){
			motor.inputMoveDirection = new Vector3(0,0,0);
			if(dashing){
				CancelDash();
			}
			return;
		}
		if(Time.timeScale == 0.0f){
			return;
		}
		CharacterController controller = GetComponent<CharacterController>();
		
		if(moveHorizontal > 0.1){
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 0, transform.eulerAngles.z);
		}else if(moveHorizontal < -0.1){
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 180, transform.eulerAngles.z);
		}
		
		if(jumpingDash){
			//Jump while dashing
			if(Input.GetButtonUp("Jump")){
				CancelDashJump();
			}
			//Vector3 jd = transform.TransformDirection(new Vector3(0 , 2 , Mathf.Abs(moveHorizontal) +1));
			Vector3 jd = transform.TransformDirection(new Vector3(0 , 2 , Mathf.Abs(Input.GetAxis("Horizontal")) +1));
			//jd.z = Input.GetAxis("Horizontal") * 5;
			
			//print (jd);
			
			controller.Move(jd * 5.9f * Time.deltaTime);
			return;
		}
		
		if(jumpingDown){
			//Vector3 jdown = transform.TransformDirection(new Vector3(0 , 0 , Mathf.Abs(moveHorizontal) +1));
			Vector3 jdown = transform.TransformDirection(new Vector3(0 , 0 , Mathf.Abs(Input.GetAxis("Horizontal")) +1));
			//jdown.z = Input.GetAxis("Horizontal") * 4;
			controller.Move(jdown * 6.3f * Time.deltaTime);
			return;
		}
		if(onWallKick){
			//Wall Kick
			motor.inputMoveDirection = new Vector3(0 , 0, moveHorizontal);
			controller.Move(wk * 6 * Time.deltaTime);
			return;
		}
		if(airJump){
			//Double Jump
			Vector3 aj = transform.TransformDirection(new Vector3(0 , 2.5f , Mathf.Abs(moveHorizontal)));
			controller.Move(aj * 4 * Time.deltaTime);
			return;
		}
		
		if(dashing){
			//Dash
			if(Input.GetKeyUp("l") || Input.GetKeyUp(KeyCode.LeftShift) || !motor.canControl){
				CancelDash();
			}
			if(Input.GetButtonDown("Jump") && (controller.collisionFlags & CollisionFlags.Below) != 0){
				StartCoroutine("DashJump");
			}
			Vector3 fwd = transform.TransformDirection(Vector3.forward);
			controller.Move(fwd * dashSpeed * Time.deltaTime);
			//motor.inputMoveDirection = fwd;
			return;
		}
		
		// Get the input vector from kayboard or analog stick
		//Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector3 directionVector = new Vector3(0 , 0, moveHorizontal);
		//Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal") , 0, 0);
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		/*if(moveHorizontal > 0.1){
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 0, transform.eulerAngles.z);
		}else if(moveHorizontal < -0.1){
			transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 180, transform.eulerAngles.z);
		}*/
		//motor.inputMoveDirection = transform.rotation * Vector3.forward;
		motor.inputMoveDirection = new Vector3(0 , 0, moveHorizontal);
		
		//Double Jump
		if(Input.GetButtonDown("Jump") && !motor.grounded && doubleJump && !onWallSlide && !onWallKick){
			StartCoroutine("DoubleJumping");
		}
		
		if (controller.collisionFlags == CollisionFlags.None && onWallSlide){
			CancelWallSlide();
		}
		
		if(Input.GetButtonDown("Jump") && sound.jumpVoice && motor.grounded){
			//audio.clip = sound.jumpVoice;
			//audio.Play();
			GetComponent<AudioSource>().PlayOneShot(sound.jumpVoice);
		}
		if(moveHorizontal != 0 && sound.walkingSound && !GetComponent<AudioSource>().isPlaying){
			GetComponent<AudioSource>().clip = sound.walkingSound;
			GetComponent<AudioSource>().Play();
		}
		
		//Activate Sprint
		if(Input.GetKeyDown("l") && !dashing && groundDash || Input.GetKeyDown(KeyCode.LeftShift)  && !dashing && groundDash){
			//if(Input.GetKeyDown("l") && !dashing && groundDash || Input.GetKeyDown(KeyCode.LeftShift)  && !dashing && groundDash){
			//Dash
			if((controller.collisionFlags & CollisionFlags.Below) != 0){
				//StartCoroutine(Dash());
				if(sound.dashVoice){
					//audio.clip = sound.dashVoice;
					//audio.Play();
					GetComponent<AudioSource>().PlayOneShot(sound.dashVoice);
				}
				StartCoroutine("Dash");
			}else if(airDash && !onWallSlide){
				StartCoroutine("AirDash");
			}
		}
		
		if(Input.GetButton ("Jump")) {
			motor.inputJump = Input.GetButton ("Jump");
		}else{
			motor.inputJump = onJumping;
		}
		//motor.inputJump = Input.GetKeyDown("c");
	}
	
	public IEnumerator Dash(){
		if(!dashing){
			if(dashEffect){
				dashEffect.SetActive(true);
				//dashEffect.GetComponent<ParticleSystem>().Clear();
			}
			dashing = true;
			if(damageTaker){
				damageTaker.GetComponent<CapsuleCollider>().height = 1.1f;
				damageTaker.GetComponent<CapsuleCollider>().center = new Vector3(0 , -0.35f , 0);
			}
			transform.position = new Vector3(transform.position.x , transform.position.y + 0.02f , transform.position.z);
			mainModel.GetComponent<Animation>().Play(anim.dash.name);
			yield return new WaitForSeconds(dashDuration);
			CancelDash();
		}
	}
	
	public void CancelDash(){
		StopCoroutine("Dash");
		if(dashEffect){
			dashEffect.SetActive(false);
		}
		mainModel.GetComponent<Animation>().Stop(anim.dash.name);
		motor.freezeGravity = false;
		dashing = false;
		if(damageTaker){
			damageTaker.GetComponent<CapsuleCollider>().height = 2;
			damageTaker.GetComponent<CapsuleCollider>().center = Vector3.zero;
		}
		transform.position = new Vector3(transform.position.x , transform.position.y + 0.02f , transform.position.z);
	}
	
	public IEnumerator AirDash(){
		if(!dashing && !airMove){
			if(dashEffect){
				dashEffect.SetActive(true);
			}
			if(sound.dashVoice){
				//audio.clip = sound.dashVoice;
				//audio.Play();
				GetComponent<AudioSource>().PlayOneShot(sound.dashVoice);
			}
			airMove = true;
			dashing = true;
			if(damageTaker){
				damageTaker.GetComponent<CapsuleCollider>().height = 1.1f;
				damageTaker.GetComponent<CapsuleCollider>().center = new Vector3(0 , -0.35f , 0);
			}
			motor.freezeGravity = true;
			mainModel.GetComponent<Animation>().Play(anim.dash.name);
			yield return new WaitForSeconds(dashDuration);
			motor.freezeGravity = false;
			CancelDash();
		}
	}
	
	public IEnumerator DoubleJumping(){
		if(!airMove2){
			mainModel.GetComponent<Animation>().Play(anim.jumpUp.name);
			airMove2 = true;
			airJump = true;
			motor.freezeGravity = true;
			yield return new WaitForSeconds(0.2f);
			motor.freezeGravity = false;
			airJump = false;
		}
	}
	
	public IEnumerator DashJump(){
		CancelDash();
		mainModel.GetComponent<Animation>().Play(anim.jumpUp.name);
		motor.inputMoveDirection = Vector3.zero;
		motor.freezeGravity = true;
		jumpingDash = true;
		jumpingDashHold = true;
		yield return new WaitForSeconds(0.25f);
		
		CancelDashJump();
	}
	
	public void CancelDashJump(){
		if(jumpingDash){
			jumpingDown = true;
		}
		jumpingDash = false;
		motor.freezeGravity = false;
		StopCoroutine("DashJump");
	}
	//void OnCollisionStay(Collision col) {
	void OnControllerColliderHit(ControllerColliderHit col){
		/*if(onMobile){
			MobileColliderHit(col);
			return;
		}*/
		CharacterController controller = GetComponent<CharacterController>();
		if(jumpingDown){
			jumpingDown = false;
		}
		if(airMove && wallKick && motor.grounded || airMove2 && wallKick && motor.grounded){
			airMove = false;
			airMove2 = false;
			motor.freezeGravity = false;
		}else if(airMove && wallKick && controller.collisionFlags == CollisionFlags.Sides && col.gameObject.tag == "Wall" || airMove2 && wallKick && controller.collisionFlags == CollisionFlags.Sides && col.gameObject.tag == "Wall"){
			airMove = false;
			airMove2 = false;
			motor.freezeGravity = false;
		}else if(airMove && (controller.collisionFlags & CollisionFlags.Below) != 0 || airMove2 && (controller.collisionFlags & CollisionFlags.Below) != 0){
			airMove = false;
			airMove2 = false;
			motor.freezeGravity = false;
		}
		
		if(col.gameObject.tag == "Wall"){
			if(Input.GetAxisRaw("Horizontal") != 0 && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && Input.GetButton("Jump") && !motor.jumping.holdingJumpButton && wallKick && !jumpingDashHold){
				//if(Input.GetButton("Horizontal") && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && Input.GetButton("Jump") && !motor.jumping.holdingJumpButton && wallKick){
				StartCoroutine(WallJump());
				//}else if(Input.GetButton("Horizontal") && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && motor.movement.velocity.z == 0 && motor.movement.velocity.y <= 0 && wallKick){
			}else if(Input.GetAxisRaw("Horizontal") != 0 && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && motor.movement.velocity.z == 0 && motor.movement.velocity.y <= 0 && wallKick){
				//Wall Slide
				CancelDashJump();
				onWallSlide = true;
				if(wallSlideEffect){
					wallSlideEffect.SetActive(true);
				}
				
				mainModel.GetComponent<Animation>().Stop(anim.jumpUp.name);
				//mainModel.animation.Play(anim.wallSlide.name);
				motor.movement.gravity = gravity / 4;
				motor.movement.maxFallSpeed = 5;
			}else if(onWallSlide){
				CancelWallSlide();
			}
		}else if(onWallSlide){
			CancelWallSlide();
		}
	}
	
	//---------------------------------------------------
	void MobileColliderHit(ControllerColliderHit col){
		CharacterController controller = GetComponent<CharacterController>();
		if(jumpingDown){
			jumpingDown = false;
		}
		if(airMove && wallKick && col.gameObject.tag != "Enemy" && motor.grounded){
			//if(airMove && wallKick && col.gameObject.tag != "Enemy"){
			airMove = false;
			motor.freezeGravity = false;
		}else if(airMove && wallKick && col.gameObject.tag != "Enemy" && controller.collisionFlags == CollisionFlags.Sides && col.gameObject.tag == "Wall"){
			airMove = false;
			motor.freezeGravity = false;
		}else if(airMove && (controller.collisionFlags & CollisionFlags.Below) != 0){
			airMove = false;
			motor.freezeGravity = false;
		}
		
		if(col.gameObject.tag == "Wall"){
			
			if(moveHorizontal != 0.0f && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && onJumping && !motor.jumping.holdingJumpButton && wallKick){
				StartCoroutine(WallJump());
			}else if(moveHorizontal != 0.0f && !motor.grounded && controller.collisionFlags == CollisionFlags.Sides && motor.movement.velocity.z == 0 && motor.movement.velocity.y <= 0 && wallKick){
				//Wall Slide
				CancelDashJump();
				onWallSlide = true;
				if(wallSlideEffect){
					wallSlideEffect.SetActive(true);
				}
				//mainModel.animation.Stop(anim.jumpUp.name);
				//mainModel.animation.Play(anim.wallSlide.name);
				motor.movement.gravity = gravity / 4;
				motor.movement.maxFallSpeed = 5;
			}else if(onWallSlide){
				CancelWallSlide();
			}
		}else if(onWallSlide){
			CancelWallSlide();
		}
	}
	//-----------------------------------------------
	
	public void CancelWallSlide(){
		if(wallSlideEffect){
			wallSlideEffect.SetActive(false);
		}
		onWallSlide = false;
		//mainModel.animation.Stop(anim.wallSlide.name);
		motor.movement.gravity = gravity;
		motor.movement.maxFallSpeed = 20;
		if(underWater){
			motor.movement.gravity = 5;
			motor.movement.maxFallSpeed = 5;
		}
	}
	
	public IEnumerator WallJump(){
		if(onWallKick){
			yield break;
		}
		
		wk = transform.TransformDirection(new Vector3(0 , 2.5f , -0.99f));
		if(Input.GetKey("l")){
			wk.z *= 1.38f;
			wk.y *= 1.5f;
		}
		CancelWallSlide();
		CancelDashJump();
		mainModel.GetComponent<Animation>().Play(anim.wallKick.name);
		mainModel.GetComponent<Animation>().Blend(anim.jump.name, 0.1f, 0.3f);
		
		motor.freezeGravity = true;
		onWallKick = true;
		if(wallKickEffect){
			Instantiate(wallKickEffect , transform.position , wallKickEffect.transform.rotation);
		}
		if(wallSlideEffect){
			wallSlideEffect.SetActive(false);
		}
		yield return new WaitForSeconds(0.15f);
		
		//print (moveHorizontal);
		if(moveHorizontal > 0.1){
			motor.inputMoveDirection = new Vector3(0 , 0 , 1);
		}else if(moveHorizontal < -0.1){
			motor.inputMoveDirection = new Vector3(0 , 0 , -1);
		}else{
			motor.inputMoveDirection = Vector3.zero;
		}
		
		motor.freezeGravity = false;
		onWallKick = false;
		//motor.inputMoveDirection = wk;
		//}
		
	}
	
}
