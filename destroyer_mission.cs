using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class destroyer_mission : MonoBehaviour {
	public GameObject[] targets;
	public bool use_exitzone=true;
	public GameObject red_marker_pref;
	public GameObject green_marker_pref;
	bool received=false;
	bool mission_complete=false;
	int k=16;
	int realscore=0;
	public byte next_m=5;
	GameObject gmarker=null;
	Texture enemy_sign;
	Texture win_tx;
	public bool mark_targets=true;
	public byte condition=0;
	// Use this for initialization
	void Start () {
		k=Screen.height/9;
		RaycastHit rh;
		GameObject m=null;
		if (mark_targets) {
		foreach (GameObject x in targets) {
			if (Physics.Raycast(x.transform.position+Vector3.up*500,Vector3.down,out rh,1000)) {
				m=Instantiate(red_marker_pref,rh.point+Vector3.up*30,Quaternion.identity) as GameObject;
			}
			else {
				m=Instantiate(red_marker_pref,x.transform.position+Vector3.up*30,Quaternion.identity) as GameObject;
			}
			m.transform.parent=x.transform;
		}
		if (use_exitzone) {
			var layerMask=1<<8;
			RaycastHit rm;
			if (Physics.Raycast(transform.position+Vector3.up*100,Vector3.down,out rm ,200,layerMask)) {
				gmarker=Instantiate(green_marker_pref,rm.point,Quaternion.identity) as GameObject; 
			}
			else {
				gmarker=Instantiate(green_marker_pref,transform.position,Quaternion.identity) as GameObject; 
			}
			gmarker.SetActive(false);
		}
		enemy_sign=Resources.Load<Texture>("enemy_sign");
		}
		win_tx=Resources.Load<Texture>("win_tx");
		if (condition!=0) StartCoroutine(Checker());
	}
	
	IEnumerator Checker() {
		yield return new WaitForSeconds(2);
		if (condition==1) {
			int x=GameObject.FindGameObjectsWithTag("unit").Length;
			if (x==0) {
				if (use_exitzone) received=true;
				else {
					mission_complete=true;
					realscore=Global.score/1000;
					Global.mission_end=true;
					if (Global.menu_script.last_mission<next_m) {
						string tws=next_m.ToString();
						if (tws.Length<2) tws='0'+tws;
						PlayerPrefs.SetString("walkthrough",tws);
				}
			}
		}
		}
		StartCoroutine(Checker());
	}

	void OnTriggerEnter (Collider c) {
		if (c.transform.root.GetComponent<catmech_physics>()&&received) {
			mission_complete=true;
			realscore=Global.score/1000;
			Global.mission_end=true;
			if (Global.menu_script.last_mission<next_m) {
				string tws=next_m.ToString();
				if (tws.Length<2) tws='0'+tws;
				PlayerPrefs.SetString("walkthrough",tws);
			}
		}
	}


	void OnGUI () {
		if (mission_complete) {
			Global.mission_end=true;
			GUI.DrawTexture(new Rect(Screen.width/2-3*k,Screen.height/2-3*k,6*k,6*k),win_tx,ScaleMode.StretchToFill);
			if (SceneManager.GetActiveScene().buildIndex!=8) {
			if (GUI.Button(new Rect(Screen.width/2-2*k,Screen.height/2-k,4*k,k),Global.txtr.dont_stop_button)) {
				SceneManager.LoadScene(next_m);
				}}
			if (GUI.Button(new Rect(Screen.width/2-2*k,Screen.height/2,4*k,k),Global.txtr.to_hangar_button)) {
				Global.to_hangar=true;
				Global.lastmission=(byte)SceneManager.GetActiveScene().buildIndex;
				SceneManager.LoadScene(0);
			}
		}
		else {
			if (Global.pause||!Global.playable||received||!mark_targets) return;
			int a=0;
			foreach (GameObject x in targets) {
				if (x!=null) a++;
			}
			if (a==0) {
				received=true;
				if (use_exitzone) {	gmarker.SetActive(true);}
				else {
					mission_complete=true;
					if (PlayerPrefs.GetInt("lastmission")<next_m-1) PlayerPrefs.SetInt("lastmission",next_m-1);
					realscore=Global.score/1000;
					Global.mission_end=true;
					if (Global.menu_script.last_mission<next_m) {
						string tws=next_m.ToString();
						if (tws.Length<2) tws='0'+tws;
						PlayerPrefs.SetString("walkthrough",tws);
					}}
			}
			else {
				float f=(Screen.width/2-k/2)/targets.Length;
				if (f>k/2) f=k/2;
				for (int ia=0;ia<a;ia++) {
					GUI.DrawTexture(new Rect(Screen.width-k-ia*k/2,0,k/2,k/2),enemy_sign,ScaleMode.StretchToFill);
				}
			}
		}
	}
}
