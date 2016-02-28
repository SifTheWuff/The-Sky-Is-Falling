using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class MAIN1 : MonoBehaviour {
	public Animator anim;
	/*
	0 - strait
	1 - redRight
	2 - blueRight
	3 - metor basic
	4 - satalite
	5 - metor 4 way
	*/
	public GameObject[] ObsArray;
	public Sprite[] HSpriteArray;
	public Sprite[] USpriteArray;

	//public GameObject straightPlanet;
	public GameObject stop;
	public GameObject playerChar;
	public GameObject cloud;
	public GameObject cloud2;
	public GameObject buttonPrompt;
	public IList obs;
	public const float offScreen = 6.5f; 
	public float startingPlayerSpeed;
	private float playerSpeed;
	public int rotateInvl = 6;
	public GameObject[] comets;
	public bool isRunning = true; 
	public AudioClip BGMusic;
	private int obsNum = 3;

	public float difficultyTime = 15;
	private float lastDifficultyChange = 0;

	private int difficulty = 0;

	public Sprite[] Hilite;

	public GameObject expl;
	private AudioSource BGSource;
	private GameObject tempObj = null;
	private Animator explode;
	private bool onMenu = true;

	private int[] Weight = { 1, 0, 0, 1, 0, 0 };
	private bool pressCheck = false;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		obs = new ArrayList ();

		reset ();
		//startGame ();

		BGSource = GetComponent<AudioSource> ();
		explode = expl.GetComponent<Animator> ();
		explode.Stop ();
	}
	void FixedUpdate () {
		playerChar.transform.Translate (new Vector2 (0, playerSpeed));
		//print (playerSpeed + " " + playerChar.transform.position.y);

		cloud.transform.Translate (new Vector2 (0, playerSpeed * 0.75f));
		cloud2.transform.Translate (new Vector2 (0, playerSpeed * 0.75f));

		if (playerChar.transform.position.y - cloud.transform.position.y > 6.7f) {
			//float aboveScreen = GameObject.Find ("Main Camera").transform.position.y + offScreen;
			cloud.transform.Translate (new Vector2 (0, 20));
		}

		if (playerChar.transform.position.y - cloud2.transform.position.y > 6.7f) {
			//float aboveScreen = GameObject.Find ("Main Camera").transform.position.y + offScreen;
			cloud2.transform.Translate (new Vector2 (0, 20));
		}

		print ("playerSpeed: " + playerSpeed + " difficulty: " + difficulty);

		if (onMenu) {
			if (Input.anyKeyDown) {
				startGame ();
			}
		} else {
			//lastDifficultyChange += Time.fixedTime;
			if (Time.fixedTime > lastDifficultyChange + difficultyTime + 2 * difficulty && isRunning) {
				//print ("Difficulty: " + difficulty);
				increaseDifficulty ();
				lastDifficultyChange = Time.fixedTime;
			}

			if (getActive () != tempObj) {
				if ((tempObj != null) && (tempObj.CompareTag ("Comet"))) {
					tempObj.GetComponent<CometBehaviour> ().spinBool = false;
				}
				if ((getActive () != null) && (getActive ().CompareTag ("Comet"))) {
					getActive ().GetComponent<CometBehaviour> ().isActive = true;
				}
				if ((tempObj != null) && (tempObj.CompareTag ("Comet"))) {
					tempObj.GetComponent<CometBehaviour> ().isActive = false;
				}
				if ((getActive () != null) && (getActive ().CompareTag ("Sphere"))) {
					getActive ().GetComponent<Highlight> ().isActive = true;
				}
				if ((tempObj != null) && (tempObj.CompareTag ("Sphere"))) {
					tempObj.GetComponent<Highlight> ().isActive = false;
				}
				tempObj = getActive ();

			//((SpriteRenderer)getActive.GetComponent (SpriteRenderer)).sprite = HSpriteArray[0];
			}

			if (((GameObject)obs [0]).transform.position.y < GameObject.Find ("Main Camera").transform.position.y - offScreen) {
				GameObject.Destroy ((GameObject)obs [0]);
				obs.RemoveAt (0);
				if (obs.Count < obsNum) {
					spawnObject ();
				}
			}

			if ((Input.anyKey) && isRunning) {
				if ((getActive ().CompareTag ("Sphere")) && pressCheck) {
					pressSphere (getActive ());
				} 
				if (getActive ().CompareTag ("Comet")) {
					pressComet (getActive ());
				}
				pressCheck = false;
			}
			if ((!Input.anyKey) && isRunning) {
				if (getActive ().CompareTag ("Comet")) {
					getActive ().GetComponent<CometBehaviour> ().spinBool = true;
				}
				pressCheck = true;
			}

			if (Input.anyKeyDown && !isRunning) {
				reset ();
			}
		}
	}

	void startGame () {
		buttonPrompt.SetActive (false);
		increaseDifficulty ();
		lastDifficultyChange = Time.fixedTime;
		onMenu = false;

		for (int i = 0; i < obsNum; i++) {
			spawnObject ();
		}
	}

	void reset () {
		buttonPrompt.SetActive (true);
		playerSpeed = startingPlayerSpeed;
		onMenu = true;
	}

	void OnCollisionEnter2D(Collision2D test){
		anim.SetTrigger ("Die");
		//explode.Play ("ExplodeAnim",0);
		playerSpeed = 0f;
		CometBehaviour.stopMoving = true;
		isRunning = false;
		((Rigidbody2D)playerChar.GetComponent (typeof(Rigidbody2D))).Sleep ();
	}

	void increaseDifficulty () {
		difficulty++;
		int minHeight = 10;
		/*
			0 - strait
			1 - redRight
			2 - blueRight
			3 - metor basic
			4 - satalite
			5 - metor 4 way
		*/

		Weight [0] = 3 - (int)(difficulty / 2);
		Weight [3] = difficulty > 1 ? 1 : 0;
		Weight [2] = Math.Max (difficulty - 2, 0);
		Weight [1] = Math.Max (difficulty - 4, 0);
		Weight [4] = Math.Max (difficulty - 5, 0);

		playerSpeed += 0.01f;

		switch (difficulty) {
		case 1:
			spawnObject (0, minHeight);
			break;
		case 2:
			spawnObject (3, minHeight);
			break;
		case 3:
			spawnObject (2, minHeight);
			break;
		case 5:
			spawnObject (1, minHeight);
			break;
		case 6:
			spawnObject (4, minHeight);
			break;
		}
	}

	GameObject getActive(){
		
		for(int i = 0; i<obs.Count;i++){
			float obsPos;

			obsPos = ((GameObject)obs [i]).transform.position.y;//= ((Collider2D)((GameObject)obs [i]).GetComponent (typeof(Collider2D))).bounds.max.y;
			
			float playerPos = playerChar.transform.position.y;
			if (obsPos + 0.7 > playerPos) {
				return ((GameObject)obs [i]);
			}
		}
		//print(obsPos);
		return null;
	}

	void spawnObject (int type = -1, int minHeight = 0) {

		GameObject obsType;
		if (type < 0) {
			obsType = generateObsType ();
		} else {
			obsType = ObsArray [type];
		}

		int rotation = UnityEngine.Random.Range (0, 3) * 90;

		float aboveScreen = GameObject.Find ("Main Camera").transform.position.y + offScreen;
		float displacement = UnityEngine.Random.Range (8, 16) / 2;
		float aboveTopObject = obs.Count > 0 ? ((GameObject)obs [obs.Count - 1]).transform.position.y + displacement : 0;

		float newYVal = Mathf.Max (aboveScreen, aboveTopObject, minHeight);

		GameObject newObs = (GameObject)Instantiate (obsType, new Vector2 (0, newYVal), new Quaternion (0, 0, 0, 0));
		newObs.transform.Rotate (new Vector3 (0, 0, rotation));
		obs.Add (newObs);
	}
		
	GameObject generateObsType() {
		
		int sumWeight = Weight.Sum ();

		int selection = UnityEngine.Random.Range (0, sumWeight);
		if (selection < Weight [0]) {
			// Return sphere object
			return ObsArray [0];
		} else if (selection < Weight [0] + Weight [1]) {
			// return toggle object
			return ObsArray [1];
		} else if (selection < Weight [0] + Weight [1] + Weight [2]) {
			// return toggle object
			return ObsArray [2];
		} else if (selection < Weight [0] + Weight [1] + Weight [2] + Weight [3]) {
			// return toggle object
			return ObsArray [3];
		} else if (selection < Weight [0] + Weight [1] + Weight [2] + Weight [3] + Weight [4]) {
			// return toggle object
			return ObsArray [4];
		} else {
			// return toggle object
			return ObsArray [5];
		}
	}

	void pressSphere(GameObject CurObs){
		CurObs.transform.Rotate (new Vector3(0,0,-90));
	}

	void pressComet(GameObject ob){
		ob.GetComponent<CometBehaviour> ().spinBool = false;
	}
}