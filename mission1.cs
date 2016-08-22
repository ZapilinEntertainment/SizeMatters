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
	public bool standart_victory=true;
	bool settings=false;
	bool help=false;
	public bool water_on_level=false;
	bool gui_set=false;
	string gamedata_string="001";
	byte quality_lvl=5;
	public string mechcode="";
	GameObject[] modules;
	Texture win_tx;

	// Use this for initialization
	void Awake() {
		Global.nongamescene=false;
		Global.pause=false;
		Global.mission_end=false;
		Global.water_on_level=water_on_level;
		Global.chosen_frame_tx=Resources.Load<Texture>("chosen_frame");
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
		Global.pause=false;
		Time.timeScale=1;
		Global.bonus=1;

		win_tx=Resources.Load<Texture>("win_tx");
	}

	IEnumerator Cutscene() {
		yield return new WaitForSeconds(cutscene_time);
		Global.playable=true;
	}

	// Update is called once per frame
	void Update () {
		if (Global.player==null) return;
		if (standart_victory) {
		if (Global.player.transform.position.z>far_border) {
			Global.cam.transform.parent=null;
			Global.cam.transform.position=new Vector3(1000,10,2000);
				if (!mission_complete) {
					mission_complete=true;
				realscore=Global.score/1000;
				Global.mission_end=true;
					if (Global.menu_script.last_mission<next_m) {
						string tws=next_m.ToString();
						if (tws.Length<2) tws='0'+tws;
						PlayerPrefs.SetString("walkthrough",tws);
					}
				Destroy(Global.player.transform.root.GetComponent<catmech_physics>().incam);
				}
			Global.cam.transform.LookAt(Global.player.transform.position);
			}
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
		case "07":
			x=Instantiate(Resources.Load<GameObject>("pvo_launcher")) as GameObject;
			break;
		case "08":
		x=Instantiate(Resources.Load<GameObject>("shlauncher")) as GameObject;
		break;
		case "09":
			x=Instantiate(Resources.Load<GameObject>("shield_gen")) as GameObject;
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
		if (index>5) x.transform.localRotation=Quaternion.Euler(0,180,0);
	}

	void OnGUI () {
		if (!gui_set) {
			GUI.skin.GetStyle("Button").fontSize=k/5;
			GUI.skin.GetStyle("Label").fontSize=k/5;
			GUI.skin.label.clipping=TextClipping.Overflow;
			GUI.skin.button.clipping=TextClipping.Overflow;
			gui_set=true;
		}
		if (mission_complete) {
			Global.mission_end=true;
			GUI.DrawTexture(new Rect(sw/2-3*k,Screen.height/2-3*k,6*k,6*k),win_tx,ScaleMode.StretchToFill);
			if (GUI.Button(new Rect(sw/2-k,Screen.height/2-k,2*k,k),Global.txtr.dont_stop_button)) {SceneManager.LoadScene(next_m);}
			if (GUI.Button(new Rect(sw/2-k,Screen.height/2,2*k,k),Global.txtr.to_hangar_button)) {
				Global.to_hangar=true;
				Global.lastmission=(byte)SceneManager.GetActiveScene().buildIndex;
				SceneManager.LoadScene(0);
			}
		}
		if (Global.pause) {
			if (GUI.Button(new Rect(sw-3*k,0,3*k,k),Global.txtr.continue_button)) {Global.pause=false;Time.timeScale=1;settings=false;}
			if (GUI.Button(new Rect(sw-3*k,k,3*k,k),Global.txtr.settings_button))  {
				if (!settings) {if (PlayerPrefs.HasKey("gdata")) gamedata_string=PlayerPrefs.GetString("gdata");settings=true;help=false;}
				else {PlayerPrefs.SetString("gdata",gamedata_string);settings=false;}
			}
			if (GUI.Button(new Rect(sw-3*k,2*k,3*k,k),"help me!"))  { help=!help;settings=false;}
			if (GUI.Button(new Rect(sw-3*k,3*k,3*k,k),Global.txtr.to_menu_button))  {SceneManager.LoadScene(0);}

			if (settings) {
				GUI.DrawTexture(new Rect(sw-3*k,k,3*k,k),Global.chosen_frame_tx,ScaleMode.StretchToFill);
				Rect r2=new Rect(sw-5*k,0,2*k,k/2);
				GUI.Label(r2,Global.txtr.sound_button);
				if (Global.sound) {
					if (GUI.Button(new Rect(r2.x+1.5f*k,r2.y,k/2,k/2),Global.ind_green_tx)) {
						Global.sound=false;
						if (PlayerPrefs.HasKey("gdata")) {
							if (gamedata_string.Length>3) gamedata_string=gamedata_string.Substring(0,2)+'0'+gamedata_string.Substring(3,gamedata_string.Length-3);
							else {
								if (gamedata_string.Length<2) gamedata_string="000";
								else gamedata_string=gamedata_string.Substring(0,2)+'0';
							}
						}
						else {
							gamedata_string="000";
						}
						PlayerPrefs.SetString("gdata",gamedata_string);
					}
				}
				else {
					if (GUI.Button(new Rect(r2.x+1.5f*k,r2.y,k/2,k/2),Global.ind_red_tx)) {
						Global.sound=true;
						if (PlayerPrefs.HasKey("gdata")) {
							if (gamedata_string.Length>3) gamedata_string=gamedata_string.Substring(0,2)+'1'+gamedata_string.Substring(3,gamedata_string.Length-3);
							else {
								if (gamedata_string.Length<2) gamedata_string="001";
								else gamedata_string=gamedata_string.Substring(0,2)+'1';
							}
						}
						else {
							gamedata_string="001";
						}
						PlayerPrefs.SetString("gdata",gamedata_string);
					}
				}

				GUI.Label(new Rect(r2.x,r2.y+1.5f*k,2*k,k/2),Global.txtr.graphics_quality_button);
				if (GUI.Button(new Rect(r2.x,r2.y+2*k,2*k,k/2),"Fastest")) {QualitySettings.SetQualityLevel(0);quality_lvl=0;}
				if (GUI.Button(new Rect(r2.x,r2.y+2.5f*k,2*k,k/2),"Fast")) {QualitySettings.SetQualityLevel(1);quality_lvl=1;}
				if (GUI.Button(new Rect(r2.x,r2.y+3*k,2*k,k/2),"Simple")) {QualitySettings.SetQualityLevel(2);quality_lvl=2;}
				if (GUI.Button(new Rect(r2.x,r2.y+3.5f*k,2*k,k/2),"Good")) {QualitySettings.SetQualityLevel(3);quality_lvl=3;}
				if (GUI.Button(new Rect(r2.x,r2.y+4*k,2*k,k/2),"Beautiful")) {QualitySettings.SetQualityLevel(4);quality_lvl=4;}
				if (GUI.Button(new Rect(r2.x,r2.y+4.5f*k,2*k,k/2),"Fantastic")) {QualitySettings.SetQualityLevel(5);quality_lvl=5;}
				Rect cr=new Rect(r2.x,r2.y+2*k,2*k,k/2);
				switch (quality_lvl) {
				case 1:cr.y+=k/2;break; 
				case 2:cr.y+=k;break;
				case 3: cr.y+=1.5f*k;break;
				case 4: cr.y+=2*k;break;
				case 5: cr.y+=2.5f*k;break;
				}
				GUI.DrawTexture(cr,Global.chosen_frame_tx,ScaleMode.StretchToFill);
				GUI.DrawTexture(r2,Global.chosen_frame_tx,ScaleMode.StretchToFill);
			}

			if (help) {
				GUI.DrawTexture(new Rect (0,0,sh/2,sh),Global.txtr.help_tx);
				GUI.DrawTexture(new Rect(sw-3*k,2*k,3*k,k),Global.chosen_frame_tx,ScaleMode.StretchToFill);
			}
		}
		else {
			if (GUI.Button(new Rect(sw-k/2,0,k/2,k/2),"Esc")) {Global.pause=true;Time.timeScale=0;}
		}
	}
}
