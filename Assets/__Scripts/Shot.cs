using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] //make this visible in the Inspector
public class Shot { //show does not extend MonoBehavior
	static public List<Shot> shots = new List<Shot>(); //list of all shots
	static public string prefsName = "QuickSnap_Shots";
	public Vector3 position; //pos of camera
	public Quaternion rotation; //Rotation of camera
	public Vector3 target; //Where the Camera is pointed


	//generatres a single-line <shot> entry for an XML doc
	public string ToXML() {
		string ss = "<shot ";
		ss += "x=\""+position.x+"\" ";
		ss += "y=\""+position.y+"\" ";
		ss += "z=\""+position.z+"\" ";
		ss += "qx=\""+rotation.x+"\" ";
		ss += "qy=\""+rotation.y+"\" ";
		ss += "qz=\""+rotation.z+"\" ";
		ss += "qw=\""+rotation.w+"\" ";
		ss += "tx=\""+target.x+"\" ";
		ss += "ty=\""+target.y+"\" ";
		ss += "tz=\""+target.z+"\" ";
		ss += " />";

		return(ss);
	}

	//Takes a PT_XMLHashtable from PT_XMLReader ofa <shot> entry in XML and parses it into a Shot
	static public Shot ParseShotXML( PT_XMLHashtable xHT) {
		Shot sh = new Shot();
		
		sh.position.x = float.Parse (xHT.att ("x"));
		sh.position.y = float.Parse (xHT.att ("y"));
		sh.position.z = float.Parse (xHT.att ("z"));
		sh.rotation.x = float.Parse (xHT.att ("qx"));
		sh.rotation.y = float.Parse (xHT.att ("qy"));
		sh.rotation.z = float.Parse (xHT.att ("qz"));
		sh.rotation.w = float.Parse (xHT.att ("qw"));
		sh.target.x = float.Parse (xHT.att ("tx"));
		sh.target.y = float.Parse (xHT.att ("ty"));
		sh.target.z = float.Parse (xHT.att ("tz"));

		return (sh);
	}

	//Loads all of the Shots from PlayerPrefs
	static public void LoadShots() {
		//Empty the shots List<Shot>
		shots = new List<Shot>();

		if(!PlayerPrefs.HasKey (prefsName)){
			//if there are no shots, reutrn
			return;
		}

		//Get the full XML and parse it
		string shotsXML = PlayerPrefs.GetString (prefsName);
		PT_XMLReader xmlr = new PT_XMLReader();
		xmlr.Parse (shotsXML);

		//Pull the PT_XMLHashlist of all <shot>s
		PT_XMLHashList hl = xmlr.xml["xml"][0]["shot"];
		for (int i=0; i<hl.Count; i++){
			//Parse each <shot> in the PT_XMLHashlist into a Shot
			PT_XMLHashtable ht = hl[i];
			Shot sh = ParseShotXML(ht);
			//add it to the List<shot> shots
			shots.Add(sh);
		}
	}

	//Save List<Shot> shots to playerprefs
	static public void SaveShots() {
		string xs = Shot.XML;

		Utils.tr (xs); //trace all the XML to the Console

		//Set the PlayerPrefs
		PlayerPrefs.SetString (prefsName, xs);

		Utils.tr ("PlayerPrefs."+prefsName+" has been set.");
	}

	//Convert all Shot.shots to XML
	static public string XML {
		get {
			//Start an XML string
			string xs = "<xml>\n";
			//Add each of the Shots as a <shot> in XML
			foreach (Shot sh in shots) {
				xs += sh.ToXML ()+"\n";
			}
			//Add the closing XML tag
			xs += "</xml>";
			return(xs);
		}
	}

	//Delete shots from Shot.shots and PlayerPRefs
	static public void DeleteShots() {
		shots = new List<Shot>();
		if (PlayerPrefs.HasKey (prefsName)) {
			PlayerPrefs.DeleteKey(prefsName);
			Utils.tr("PlayerPrefs."+prefsName+" has been deleted.");
		} else {
			Utils.tr("there was no PlayerPrefs."+prefsName+" to delete.");
		}
	}

	//Replace the shot
	static public void ReplaceShot(int ndx, Shot sh) {
		//Make sure theres a Shot at that index to replace
		if (shots==null || shots.Count <= ndx) return;
		//remove the old shot
		shots.RemoveAt (ndx);
		//List<>.Insert() adds somethign to the list at ta spsecific index
		shots.Insert (ndx,sh);

		Utils.tr ("Replaced shot:", ndx, "with", sh.ToXML ());
	}
}
