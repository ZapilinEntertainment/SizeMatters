using UnityEngine;
using System.Collections;

public class menu : MonoBehaviour {
	public ushort shot_effect_count=30;
	public ushort shot_expl_effect_count=30;
	public ushort dust_effect_count=30;
	GameObject effects;

	void Start () {
		Global.pause=false;
		effects=new GameObject("effects");
		GameObject x=Resources.Load<GameObject>("shot_effect");
		Global.r_shot_effect=new GameObject[shot_effect_count];
		ushort i=0;
		for (i=0;i<shot_effect_count;i++) {
			Global.r_shot_effect[i]=Instantiate(x) as GameObject;
			Global.r_shot_effect[i].SetActive(false);
			Global.r_shot_effect[i].transform.parent=effects.transform;
		}
		x=Resources.Load<GameObject>("shot_expl_effect");
		Global.r_shot_expl_effect=new GameObject[shot_expl_effect_count];
		for (i=0;i<shot_expl_effect_count;i++) {
			Global.r_shot_expl_effect[i]=Instantiate(x) as GameObject;
			Global.r_shot_expl_effect[i].SetActive(false);
			Global.r_shot_expl_effect[i].transform.parent=effects.transform;
		}
		x=Resources.Load<GameObject>("dust_effect");
		Global.r_dust_effect=new GameObject[dust_effect_count];
		for (i=0;i<dust_effect_count;i++) {
			Global.r_dust_effect[i]=Instantiate(x) as GameObject;
			Global.r_dust_effect[i].SetActive(false);
			Global.r_dust_effect[i].transform.parent=effects.transform;
		}
		Global.r_flatten_tank0=Resources.Load<GameObject>("flatten_tank0");
		Global.r_flatten_tree=Resources.Load<GameObject>("flatten_tree");
		Global.r_fired_place=Resources.Load<GameObject>("fired_ground");
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Global.pause) {
				Global.pause=false;
				Time.timeScale=1;
			}
			else {
				Global.pause=true;
			     Time.timeScale=0;
			}
		}
		Global.gui_piece=Screen.height/24;
	}
}
