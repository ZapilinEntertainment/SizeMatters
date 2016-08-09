using UnityEngine;
using System.Collections;

public class menu : MonoBehaviour {
	public ushort shot_effect_count=30;
	public ushort shot_expl_effect_count=30;
	public ushort dust_effect_count=30;
	GameObject effects;
	Transform fireplaces;
	int lastscore=0;
	float deltascore=0;
	public byte stage=1;
	public int stage_score=5000;
	public float score_fall_speed=20;
	public Texture frame_tx;
	public Texture h_line_tx;

	void Start () {
		Global.pause=false;
		Global.menu_script=this;
		fireplaces=new GameObject("fireplaces").transform;
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
		Global.r_flatten_bunker0=Resources.Load<GameObject>("flatten_bunker0");
		Global.r_flatten_tree=Resources.Load<GameObject>("flatten_tree");
		Global.r_fired_place=Resources.Load<GameObject>("fired_ground");
		Global.r_dead_tank0=Resources.Load<GameObject>("dead_tank0");
		Global.r_dead_bunker0=Resources.Load<GameObject>("dead_bunker0");
		Global.r_dead_flatten_tank0=Resources.Load<GameObject>("dead_flatten_tank0");
		Global.r_rocket=Resources.Load<GameObject>("rocket");
		Global.r_homing_missile=Resources.Load<GameObject>("homing_missile");
		Global.r_tree_oak_sprite=Resources.Load<GameObject>("tree_oak_sprite");
		Global.r_tank0_lod_sprite=Resources.Load<GameObject>("tank0_lod_sprite");
		Global.r_simple_rocket=Resources.Load<GameObject>("simpleRocket");

		Global.small_explosion=Instantiate(Resources.Load<ParticleSystem>("expl0")) as ParticleSystem;
		Global.concrete_dust=Instantiate(Resources.Load<ParticleSystem>("concrete_dust")) as ParticleSystem;
		Global.artillery_strike=Instantiate(Resources.Load<ParticleSystem>("artillery_strike")) as ParticleSystem;

		Global.aim_tx_right=Resources.Load<Texture>("aim_right");
		Global.aim_tx_left=Resources.Load<Texture>("aim_left");
		Global.aim_slider_tx=Resources.Load<Texture>("aim_slider");
		Global.ind_red_tx=Resources.Load<Texture>("ind_red");
		Global.ind_green_tx=Resources.Load<Texture>("ind_green");
		Global.quality=QualitySettings.GetQualityLevel();
	}
	// Update is called once per frame

	public void AddFireplace(Vector3 pos) {
		float d=0;
		Transform t;
		if (fireplaces.childCount>0) {
		for (int i=0;i<fireplaces.childCount;i++) {
			t=fireplaces.GetChild(i);
			d=Vector3.Distance(t.position,pos);
			if (d<=5) {				
					return;
			}
			}}
		RaycastHit rh;
		var layerMask = 1 << 8;
		if (Physics.Raycast(new Vector3(pos.x,pos.y+5,pos.z),Vector3.down,out rh,6,layerMask)) {
			GameObject fp=Instantiate(Global.r_fired_place,rh.point+Vector3.up*0.1f,Quaternion.Euler(0,Random.value*360,0)) as GameObject;
			fp.transform.parent=fireplaces;
		}
	}

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
		float ud=Global.score-lastscore;
		if (ud==0) {
			if (stage>0) {
				deltascore-=Time.deltaTime;
				if (deltascore<=0) {
					stage--;
					if (stage!=0) deltascore=stage_score*stage;
					else deltascore=0;
				}
			}
		}
		else //ud>0
		{
			deltascore+=ud;
			if (deltascore>=stage*stage_score) {
				stage++;
				deltascore-=stage*stage_score;
			}
		}
        
			lastscore=Global.score;
		Global.gui_piece=Screen.height/24;
	}

	void OnGUI () {
		if (!Global.playable) return;
		GUILayout.Label(deltascore.ToString());
		GUILayout.Label(stage.ToString());
	}
}
