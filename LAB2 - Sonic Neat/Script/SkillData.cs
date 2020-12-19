using UnityEngine;
using System.Collections;

public class SkillData : MonoBehaviour {
	
	[System.Serializable]
	public class Skil {
		public string skillName = "";
		public Texture2D icon;
		public Transform skillPrefab;
		public string description = "";
		public AnimationClip skillAnimation;
		public AnimationClip jumpSkillAnimation;
		public AnimationClip onWallSkillAnimation;
		public float castTime = 0.3f;
		public float skillDelay = 0.3f;
		public int manaCost = 10;
	}
	
	public Skil[] skill = new Skil[3];
}