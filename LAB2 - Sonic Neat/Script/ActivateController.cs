//This Script for the Demo to allow Player Freely Change Setting of Player InpuController
using UnityEngine;
using System.Collections;

public class ActivateController : MonoBehaviour {
	public GameObject player;
	public Texture2D helpButton;
	PlayerInputController controller;
	public bool showHelp = true;
	// Use this for initialization
	void Start () {
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
		controller = player.GetComponent<PlayerInputController>();
	}
	
	void Update(){
		if(Input.GetKeyDown("h")){
			if(showHelp){
				showHelp = false;
			}else{
				showHelp = true;
			}
		}
	}
	
	void OnGUI(){
		GUI.Label (new Rect (Screen.width - 300, 50, 100, 30), "Dash");
		GUI.Label (new Rect (Screen.width - 300, 80, 100, 30), "Wall Kick");
		GUI.Label (new Rect (Screen.width - 300, 110, 100, 30), "Air Dash");
		GUI.Label (new Rect (Screen.width - 300, 140, 100, 30), "Double Jump");
		
		if(controller.groundDash){
			if (GUI.Button (new Rect (Screen.width - 200,50,100,30), "On")) {
				controller.groundDash = false;
			}
		}else{
			if (GUI.Button (new Rect (Screen.width - 200,50,100,30), "Off")) {
				controller.groundDash = true;
			}
		}
		//--------------------------------------
		if(controller.wallKick){
			if (GUI.Button (new Rect (Screen.width - 200,80,100,30), "On")) {
				controller.wallKick = false;
			}
		}else{
			if (GUI.Button (new Rect (Screen.width - 200,80,100,30), "Off")) {
				controller.wallKick = true;
			}
		}
		//--------------------------------------
		if(controller.airDash){
			if (GUI.Button (new Rect (Screen.width - 200,110,100,30), "On")) {
				controller.airDash = false;
			}
		}else{
			if (GUI.Button (new Rect (Screen.width - 200,110,100,30), "Off")) {
				controller.airDash = true;
			}
		}
		//--------------------------------------
		if(controller.doubleJump){
			if (GUI.Button (new Rect (Screen.width - 200,140,100,30), "On")) {
				controller.doubleJump = false;
			}
		}else{
			if (GUI.Button (new Rect (Screen.width - 200,140,100,30), "Off")) {
				controller.doubleJump = true;
			}
		}
		//Show Help Button
		if(helpButton && showHelp){
			GUI.DrawTexture(new Rect(5, 184, 200, 286), helpButton);
		}
		
	}
}
