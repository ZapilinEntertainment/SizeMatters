using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mission1 : MonoBehaviour {
	public Transform right_pauldron;
	public Transform left_pauldron;
	public Vector3 endpoint=new Vector3(0,0,2200);
	bool mission_complete=false;
	public int far_border=1700;
	public int cam_border=2060;
	int k=16;
	int sw;
	int sh;
	int realscore=0;
	public byte next_m=2;
	public float cutscene_time=0;

	public string mechcode="";
	GameObject[] modules;
	// Use this for initialization
	void Awake() {
		Global.nongamescene=false;
		Global.pause=false;
		Global.mission_end=false;
		if (cutscene_time>0) {
			Global.playable=false;
			StartCoroutine(Cutscene());
		}
		else {
			Global.playable=true;
		}
	}

	void Start () {
		k=Screen.height/9;
		sw=Screen.width;
			sh=Screen.height;
		if (PlayerPrefs.HasKey("mechcode")) mechcode=PlayerPrefs.GetString("mechcode");
		else mechcode="0102";
		modules=new GameObject[mechcode.Length/2];
		for (byte i=0;i<mechcode.Length/2;i++) {
			SpawnModule(mechcode.Substring(i*2,2),i);
		}
		Global.endpoint=endpoint;
	}

	IEnumerator Cutscene() {
		yield return new WaitForSeconds(cutscene_time);
		Global.playable=true;
	}

	// Update is called once per frame
	void Update () {
		if (Global.player==null) return;
		if (Global.player.transform.position.z>far_border) {
			Global.cam.transform.parent=null;
			Global.cam.transform.position=new Vector3(1000,10,2000);
				if (!mission_complete) {
					mission_complete=true;
					PlayerPrefs.SetInt("lastmission",2);
				realscore=Global.score/1000;
				Global.mission_end=true;
				Destroy(Global.player.transform.root.GetComponent<catmech_physics>().incam);
				}
			Global.cam.transform.LookAt(Global.player.transform.position);
			}
				
		}


	void SpawnModule (string code, byte index) {
		if (code=="00") return;
		GameObject x=modules[index];
		if (x!=null) Destroy(x);
		Vector3 add_correction_vector=Vector3.zero;
		bool right=true;
		switch (code) {
		case "01":
			x=Instantiate(Resources.Load<GameObject>("plasmagun")) as GameObject;
			if (index!=0) {
				x.transform.rotation=Quaternion.Euler(x.GetComponent<moduleInfo>().rot_lf);
				add_correction_vector=new Vector3(1.5f,-1,0);
				right=false;
			}
			break;
		case "02":
			x=Instantiate(Resources.Load<GameObject>("chaingun")) as GameObject;
			break;
		case "03":
			x=Instantiate(Resources.Load<GameObject>("rocket_launcher")) as GameObject;
			break;	
		case "05":
		x=Instantiate(Resources.Load<GameObject>("pvogun")) as GameObject;
		break;
		case "06":
		x=Instantiate(Resources.Load<GameObject>("artbattery")) as GameObject;
		break;
		case "08":
		x=Instantiate(Resources.Load<GameObject>("shlauncher")) as GameObject;
		if (index%2==0) right=true; else right=false;
		break;
		}
		if (index%2==0) right=true; else right=false;
		Vector3 pos=Vector3.zero;
		Vector3 slot_correction=Vector3.zero;
		switch (index) {
		case 2: slot_correction=new Vector3(-2,3,3.5f); break;
		case 3: slot_correction=new Vector3(2,3,3.5f); break;
		case 4: slot_correction=new Vector3(1.5f,3,3.5f); break;
		case 5: slot_correction=new Vector3(-1.5f,3,3.5f); break;
		case 6: slot_correction=new Vector3(-2,3,-3.5f); break;
		case 7: slot_correction=new Vector3(-1.5f,3,-3.5f); break;
		case 8: slot_correction=new Vector3(1.5f,3,-3.5f); break;
		case 9: slot_correction=new Vector3(2,3,-3.5f); break;
		}
		if (right) x.transform.parent=right_pauldron;
		else x.transform.parent=left_pauldron;
		x.transform.position=pos+add_correction_vector;
		modules[index]=x;
		x.transform.localPosition=slot_correction+x.GetComponent<moduleInfo>().correction_vector;
		x.BroadcastMessage("SlotNumber",index,SendMessageOptions.DontRequireReceiver);
	}

	void OnGUI () {
		if (mission_complete) {
			GUI.Label(new Rect(sw/2-k,Screen.height/2,4*k,k),"mission complete!");
			if (GUI.Button(new Rect(sw/2-2*k,Screen.height/2+k,2*k,k),"Onward!")) {SceneManager.LoadScene(next_m);}
			if (GUI.Button(new Rect(Screen.width/2,Screen.height/2+k,2*k,k),"Leave")) {SceneManager.LoadScene(0);}

		}
	}
}
