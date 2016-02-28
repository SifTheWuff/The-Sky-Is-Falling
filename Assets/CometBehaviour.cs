using UnityEngine;
using System.Collections;

public class CometBehaviour : MonoBehaviour {
	public float rotateInvl = 6;
	public static bool stopMoving = false;
	public bool spinBool = true;
	public Sprite Hsprite;
	public Sprite Usprite;
	public bool isActive = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isActive) {
			((SpriteRenderer)this.GetComponent (typeof(SpriteRenderer))).sprite = Hsprite;
		}
		if (spinBool && !stopMoving) {
			this.transform.Rotate (new Vector3 (0, 0, rotateInvl));
		}
	}
}
