using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {
	public Sprite Hsprite;
	public Sprite Usprite;
	public bool isActive = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
			((SpriteRenderer)this.GetComponent (typeof(SpriteRenderer))).sprite = Hsprite;
		}
	}
}
