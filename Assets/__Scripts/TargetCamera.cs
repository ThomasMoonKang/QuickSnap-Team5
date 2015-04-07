using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetCamera : MonoBehaviour {
	public bool editMode = true;
	public GameObject fpCamera; //First person camera

	public bool checkToDeletePlayerPrefs = false;

	public bool _________________;

	public Rect camRectNormal; //Pulle dfrom camera.rect

	public int shotNum;
	public GUIText shotCounter, shotRating;
	public GUITexture checkMark;

	void Start () {
		//Find the GUI coponennts
		GameObject go = GameObject.Find ("ShotCounter");
		shotCounter = go.GetComponent<GUIText>();
		go = GameObject.Find ("ShotRating");
		shotRating = go.GetComponent<GUIText>();
		go = GameObject.Find ("_Check_64");
		checkMark = go.GetComponent<GUITexture>();
		//Hide the checkmark
		checkMark.enabled = false;

		//Load all the shots from PlayerPRefs
		Shot.LoadShots ();
		//If htere were shots stored in PlayerPrefs
		if (Shot.shots.Count>0){
			shotNum = 0;
			ShowShot(Shot.shots[shotNum]);
		}

		//Hide the cursor (Note: this doesnt work in the unity editor
		//Unless the game pane is set to Maximize on Play
		Cursor.visible = false;

		camRectNormal = GetComponent<Camera>().rect;
	}
	
	// Update is called once per frame
	void Update () {
		Shot sh;

		//Mouse input
		//If left or Right mouse button is pressed this frame
		if (Input.GetMouseButtonDown (0)|| Input.GetMouseButtonDown (1)) {//Left mouse button
			sh = new Shot();
			//Grab the pos and rot of fpCamera
			sh.position = fpCamera.transform.position;
			sh.rotation = fpCamera.transform.rotation;

			//Shoot a ray from the amera and shee what it hits
			Ray ray = new Ray(sh.position, fpCamera.transform.forward);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				sh.target = hit.point;
			}

			if(editMode) {
				if(Input.GetMouseButtonDown (0)) {
					//Left button records a new shot
					Shot.shots.Add (sh);
					shotNum = Shot.shots.Count-1;
				} else if (Input.GetMouseButtonDown (1)) {
					//Right button replaces the current shot
					Shot.ReplaceShot (shotNum, sh);
					ShowShot(Shot.shots[shotNum]);
				}
			}

			//Position _TargetCamera with the shot
			//ShowShot(sh);

			Utils.tr (sh.ToXML());

		//Record a new shot
		Shot.shots.Add(sh);
		shotNum = Shot.shots.Count-1;
		}

	//Keyboard Input, use Q and E to cycle shots
	//Note either of these will throw an error if Shot.shots is empty
	if (Input.GetKeyDown(KeyCode.Q)) {
		shotNum--;
		if(shotNum < 0) shotNum = Shot.shots.Count-1;
		ShowShot(Shot.shots[shotNum]);
	}
	if (Input.GetKeyDown(KeyCode.E)) {
		shotNum++;
		if(shotNum >= Shot.shots.Count) shotNum = 0;
		ShowShot(Shot.shots[shotNum]);
	}
	//If in editMode and left shift is held down
	if(editMode && Input.GetKey(KeyCode.LeftShift)) {
		//use Shift-s to Save
		if (Input.GetKeyDown(KeyCode.S)) {
			Shot.SaveShots();
		}
		//Use shit==x to output xml to console
		if (Input.GetKeyDown(KeyCode.X)) {
			Utils.tr(Shot.XML);
		}
	}
		
	//Hold Tab to maximize the Target window
	if(Input.GetKeyDown(KeyCode.Tab)) {
		GetComponent<Camera>().rect = new Rect(0,0,1,1);
	}
	if (Input.GetKeyUp(KeyCode.Tab)) {
		GetComponent<Camera>().rect=camRectNormal;
	}
	//Update the Guitexts 
	shotCounter.text = (shotNum+1).ToString()+" of "+Shot.shots.Count;
	if (Shot.shots.Count == 0) shotCounter.text = "No shots exist";
	//^ Shot.shots.count doesnt require .ToString() because it is assumed
	//when the left side of the + operator is a string
	shotRating.text = ""; //this line will be replaced later
}

	public void ShowShot (Shot sh) {
		//Position _TargetCamera with the Shot
		transform.position = sh.position;
		transform.rotation = sh.rotation;
	}

	//OnDrawGizmos() is called ANY time Gizmos need to be drawn, even when units isn tplaying
	public void OnDrawGizmos() {
		List<Shot> shots = Shot.shots;
		for (int i=0; i<shots.Count; i++) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(shots[i].position, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(shots[i].position, shots[i].target);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(shots[i].target, 0.25f);
		}

		//If check to deleteplayers ifs checked
		if (checkToDeletePlayerPrefs) {
			Shot.DeleteShots (); //Delete all the shots
			//Uncheck checkToDeletePlayerPrefs
			checkToDeletePlayerPrefs = false;
			shotNum = 0; //Set shotNum to 0
		}
	}

}