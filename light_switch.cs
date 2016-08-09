using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class light_switch : MonoBehaviour {
	public Texture start_button_tx;
	public Texture settings_button_tx;
	public Texture help_button_tx;
	public Texture credits_button_tx;
	public Texture exit_button_tx;
	public Texture return_button_tx;
	public Texture weaponry_button_tx;
	public Texture chosen_frame_tx;
	public Texture back_tx;
	public Texture module_empty;
	public Light main_light;
	public Transform cam;
	public Vector3 cam_menu_pos=new Vector3(35.5f,20,34);
	public Vector3 cam_menu_rot=new Vector3(355,246,0);
	public Vector3 cam_custom_fwd_pos=new Vector3(0,42,28);
	public Vector3 cam_custom_fwd_rot=new Vector3(17,180,0);
	public Vector3 cam_custom_back_pos=new Vector3(10,46,-25);
	public Vector3 cam_custom_back_rot=new Vector3(17,0,0);
	public AudioClip mech_activated;
	public AudioClip mech_deactivated;
	public Transform right_pauldron;
	public Transform left_pauldron;
	public string mechcode="0102";
	GameObject[] modules;
	int k=16;
	int sh=600;
	int sw=800;
	public byte chosen_module=255;
	Rect r1,r2,r3,r4,r5;
	int gui_state=0;
	public Renderer[] rrs;
	public float speed=10;
	public bool powered=false;
	bool settings=false;
	bool help=false;
	bool credits=false;
	bool weaponry=false;
	public int lastmission=10;
	byte i=0;
	// Use this for initialization

	void Awake () {
		Global.nongamescene=true;
		Global.cam=cam.gameObject;
		Global.pause=false;
	}

	void Start () {
		//if (PlayerPrefs.HasKey("lastmission")) {
		//	lastmission=PlayerPrefs.GetInt("lastmission");
		//}
		//else {PlayerPrefs.SetInt("lastmission",0);}
		sh=Screen.height;
		sw=Screen.width;
		k=sh/9;
		int sti=sw-2*k;
		r1=new Rect(sti,2*k,2*k,k);
		r2=new Rect(sti,r1.y+k,2*k,k);
		r3=new Rect(sti,r2.y+k,2*k,k);
		r4=new Rect(sti,r3.y+k,2*k,k);
		r5=new Rect(sti,r4.y+k,2*k,k);
		if (!powered) {
			for (int i=0;i<rrs.Length;i++) {
				rrs[i].material.SetColor("_EmissionColor",Color.black);
			}
		}
		cam.position=cam_menu_pos;
		cam.rotation=Quaternion.Euler(cam_menu_rot);
		if (PlayerPrefs.HasKey("mechcode")) {
			mechcode=PlayerPrefs.GetString("mechcode");
			if (mechcode.Length<20) mechcode=mechcode+new string ('0',20-mechcode.Length);
		}
		else mechcode="0102";
		modules=new GameObject[mechcode.Length/2];
		for (byte i=0;i<mechcode.Length/2;i++) {
			SpawnModule(mechcode.Substring(i*2,2),i);
		}
	}

	void SpawnModule (string code, byte index) {
		if (code=="00") return;
		GameObject x=modules[index];
		if (x!=null) Destroy(x);
		bool right=true;
		switch (code) {
		case "00": break;
		case "01":
			x=Instantiate(Resources.Load<GameObject>("plasmagun")) as GameObject;
			if (index!=0) {
				x.transform.rotation=Quaternion.Euler(x.GetComponent<moduleInfo>().rot_lf);
				right=false;
			}
			break;
		case "02":
			x=Instantiate(Resources.Load<GameObject>("chaingun")) as GameObject;
			if (index!=0) right=false;
			break;
		case "03":
			x=Instantiate(Resources.Load<GameObject>("rocket_launcher")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "04":break;
		case "05":
			x=Instantiate(Resources.Load<GameObject>("pvogun")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "06":
			x=Instantiate(Resources.Load<GameObject>("artbattery")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
		case "08":
			x=Instantiate(Resources.Load<GameObject>("shlauncher")) as GameObject;
			if (index%2==0) right=true; else right=false;
			break;
			
		}

		Vector3 pos=Vector3.zero;
		Vector3 slot_correction=Vector3.zero;
		if (right) {pos=right_pauldron.position;x.transform.parent=right_pauldron;}
		else {pos=left_pauldron.position;x.transform.parent=left_pauldron;}
		if (index>1) {
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
		}
		modules[index]=x;
		x.transform.position=pos;
		x.transform.localPosition+=slot_correction+x.GetComponent<moduleInfo>().correction_vector;
		x.transform.localRotation=Quaternion.Euler(0,0,0);
		if (index!=modules.Length-1) mechcode=mechcode.Substring(0,index*2)+x.GetComponent<moduleInfo>().code.Substring(0,2)+mechcode.Substring((index+1)*2,(modules.Length-index-1)*2);
		else {mechcode=mechcode.Substring(0,index*2)+x.GetComponent<moduleInfo>().code.Substring(0,2);}
	}

	public void LightsOn() {
		foreach (Renderer r in rrs) {
			r.material.SetColor("_EmissionColor",Color.white);
		}
		powered=true;
		AudioSource.PlayClipAtPoint(mech_activated,cam.position);
	}
	public void LightsOff() {
		foreach (Renderer r in rrs) {
			r.material.SetColor("_EmissionColor",Color.black);
		}
		powered=false;
		AudioSource.PlayClipAtPoint(mech_deactivated,cam.position);
	}

	void OnGUI () {
		switch (gui_state) {
		case 0: 
			if (GUI.Button(r1,start_button_tx)) {
				gui_state=1;
				r1=new Rect(sw-2*k,2*k,2*k,k/2);
				r5=new Rect(sw-2*k,0,2*k,k);
				r3=new Rect(sw-2*k,k,2*k,k);
				LightsOn();
				settings=false;
				credits=false;
				help=false;
				chosen_module=255;
		}
			if (GUI.Button(r2,settings_button_tx)) {
				if (!settings) {
					settings=true;credits=false;help=false;
				}
				else settings=false;
			}
			if (GUI.Button(r3,help_button_tx)) {
				if (!help) {help=true;settings=false;credits=false;}
				else help=false;
			}
			if (GUI.Button(r4,credits_button_tx)) {
				if (!credits) {help=false;settings=false;credits=true;}
				else credits=false;
			}
			if (GUI.Button(r5,exit_button_tx)) {
				Application.Quit();
			}
			break;
		case 1:
			int py=(int)r1.y;
			for (i=1;i<=lastmission;i++) {
				if (GUI.Button(new Rect(r1.x,py,r1.width,r1.height),"mission"+i.ToString())) {PlayerPrefs.SetString("mechcode",mechcode);SceneManager.LoadScene(i);}
				py+=(int)(k/2);
				}
				int lm=(int)(1.5f*k);
			moduleInfo mi=null;
				if (modules[1]!=null) {
				mi=modules[1].GetComponent<moduleInfo>();
				if 	(GUI.Button(new Rect(r1.x-3*k,r1.y+k,k,k),mi.picture)) chosen_module=1;
				}
				if (modules[0]!=null) {
					mi=modules[0].GetComponent<moduleInfo>();
				if 	(GUI.Button(new Rect(r1.x-k,r1.y+k,k,k),mi.picture)) chosen_module=0;
				}
			Texture module_picture=module_empty;
			mi=null;
			if (modules[2]!=null) {mi=modules[2].GetComponent<moduleInfo>();	}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-k,r1.y,k/2,k/2),module_picture)) chosen_module=2;

			mi=null;
			if (modules[3]!=null) {mi=modules[3].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-2.5f*k,r1.y,k/2,k/2),module_picture)) chosen_module=3;

			mi=null;
			if (modules[4]!=null) {mi=modules[4].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-0.5f*k,r1.y,k/2,k/2),module_picture)) chosen_module=4;

			mi=null;
			if (modules[5]!=null) {mi=modules[5].GetComponent<moduleInfo>();}
			if (mi!=null) module_picture=mi.picture; else module_picture=module_empty;
			if 	(GUI.Button(new Rect(r1.x-3*k,r1.y,k/2,k/2),module_picture)) chosen_module=5;

				Rect ch_rect=new Rect(-k,-k,k,k);
		

			if (chosen_module!=255) {
				if (chosen_module<2) {
					if (GUI.Button(new Rect(r1.x-5*k,r1.y,2*k,k/2),"Heavy Plasmagun")) SpawnModule("01",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+k/2,2*k,k/2),"Heavy Chaingun")) SpawnModule("02",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+k,2*k,k/2),"Heavy Rocket Launcher")) SpawnModule("03",chosen_module);
				}
				else {
					if (GUI.Button(new Rect(r1.x-5*k,r1.y,2*k,k/2),"Air Defence Gun")) SpawnModule("05",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+k/2,2*k,k/2),"Artillery Tower")) SpawnModule("06",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+k,2*k,k/2),"Air Defence Rockets")) SpawnModule("07",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+1.5f*k,2*k,k/2),"Rocket Launchers")) SpawnModule("08",chosen_module);
					if (GUI.Button(new Rect(r1.x-5*k,r1.y+2*k,2*k,k/2),"Shield Generator")) SpawnModule("09",chosen_module);
				}
			
				switch (chosen_module) {
				case 0: ch_rect=new Rect(r1.x-k,r1.y+k,k,k);break;
				case 1: ch_rect=new Rect(r1.x-3*k,r1.y+k,k,k); break;
				case 2: ch_rect=new Rect(r1.x-k,r1.y,k/2,k/2);break;
				case 3: ch_rect=new Rect(r1.x-2.5f*k,r1.y,k/2,k/2); break;
				case 4: ch_rect=new Rect(r1.x-0.5f*k,r1.y,k/2,k/2);break;
				case 5: ch_rect=new Rect(r1.x-3*k,r1.y,k/2,k/2); break;
				}
				GUI.DrawTexture(ch_rect,chosen_frame_tx,ScaleMode.StretchToFill);
			}
			if (GUI.Button(r5,return_button_tx)) {
				gui_state=0;
				r1=new Rect(sw-2*k,2*k,2*k,k);
				r5=new Rect(sw-2*k,6*k,2*k,k);
				r3=new Rect(sw-2*k,4*k,2*k,k);
				if (weaponry)
				{weaponry=false;
				cam.position=cam_menu_pos;
				cam.rotation=Quaternion.Euler(cam_menu_rot);
					PlayerPrefs.SetString("mechcode",mechcode);
				}
				main_light.enabled=false;
				LightsOff();
			}
			break;
		}
	}
}
