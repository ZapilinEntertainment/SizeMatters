using UnityEngine;
using System.Collections;

public class chaingun : MonoBehaviour {
	Transform pauldron;
	public Transform[] gun_points;

	public ParticleSystem near_ps;
	public ParticleSystem bullets_fall;
	public ParticleSystem dust_emitter_pref;
	ParticleSystem[] far_ps;
	public LineRenderer line_renderer_pref;
	LineRenderer[] lrs;
	public bool right; 
	bool firing=false;
	public float range=5000;
	public float angle_border=60;
	Quaternion rotateTo;
	public float damage=1500;
	public float damage_radius=5;
	public AudioClip sound;
	int k=16;
	float temperature;
	public float max_temperature=400;
	public float temp_speed=20;
	public float max_ammo=10000;
	public float ammo_consumption=1000;
	float ammo;
	public GameObject ammo_box;

	void Start () {
		if (Global.nongamescene) {Destroy(this);return;}
		if (transform.root.InverseTransformPoint(transform.position).x>0) {
		   right=true;
			bullets_fall.transform.Rotate(0,180,0);
		} else right=false;
		ammo=max_ammo;
		temperature=0;
		k=Screen.height/27;
		if (sound) {
			if (right) {Global.sm.rightgun_sound=sound;Global.sm.r_gun_dominating=true;}
			else {Global.sm.leftgun_sound=sound;Global.sm.l_gun_dominating=true;}
		}
		far_ps=new ParticleSystem[gun_points.Length];
		lrs=new LineRenderer[gun_points.Length];
		if (dust_emitter_pref) {
			for (byte i=0;i<gun_points.Length;i++) {
				far_ps[i]=Instantiate(dust_emitter_pref) as ParticleSystem;
			}
		}
		if (line_renderer_pref) {
			for (byte i=0;i<gun_points.Length;i++) {
				lrs[i]=Instantiate(line_renderer_pref) as LineRenderer;
			}
		}
		pauldron=transform.parent;
	}

	void Update() {
		if (Global.pause||Global.nongamescene||!Global.playable) return;
		float an=Vector3.Angle(Global.aim-transform.position,pauldron.transform.forward);
		if (an<60&&pauldron.transform.InverseTransformPoint(Global.aim).z>0) {
			rotateTo=Quaternion.LookRotation(Global.aim-transform.position);
		}			
		if (rotateTo!=transform.rotation) {transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,45*Time.deltaTime);}

		if (Input.GetMouseButton(0)&&!right||Input.GetMouseButton(1)&&right) 
		{
			if (!firing) {
				if (temperature<max_temperature*0.9f&&ammo>0) {
				firing=true;
				near_ps.Play();
				bullets_fall.Play();
				foreach (LineRenderer lr in lrs) {lr.enabled=true;}
				Global.sm.GunSound(right,true,true);
				}
			}
			else {
				temperature+=temp_speed*Time.deltaTime/Global.bonus;
				ammo-=ammo_consumption*Time.deltaTime;
				if (ammo<0) ammo=0;
				if (ammo==0) {
					firing=false;
					near_ps.Stop();
					bullets_fall.Stop();
					foreach (LineRenderer lr in lrs) {lr.enabled=false;}
					Global.sm.GunSound(right,true,false);
					ammo_box.transform.parent=null;
					ammo_box.AddComponent<BoxCollider>();
					Rigidbody ar=ammo_box.AddComponent<Rigidbody>();
					ar.useGravity=true;
					ar.mass=50;
					ammo_box.AddComponent<foot>();
					ammo_box.AddComponent<timer>();
				}
				if (temperature>=max_temperature) {
					temperature=max_temperature;
					firing=false;
					near_ps.Stop();
					bullets_fall.Stop();
					foreach (LineRenderer lr in lrs) {lr.enabled=false;}
					Global.sm.GunSound(right,true,false);
				}
			}
		}
		else {
			if (firing) {
				firing=false;
				near_ps.Stop();
				bullets_fall.Stop();
				foreach (LineRenderer lr in lrs) {lr.enabled=false;}
				Global.sm.GunSound(right,true,false);
			}
		}

		if (firing) {
			RaycastHit rh;
			for (byte i=0;i<gun_points.Length;i++) {
				lrs[i].SetPosition(0,gun_points[i].position);
				if (Physics.Raycast(gun_points[i].position,transform.forward,out rh,range)) {
					Collider[] cs=Physics.OverlapSphere(rh.point,damage_radius);
					foreach (Collider c in cs) {if (c.transform.root.gameObject.layer!=8) c.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);}
					if (rh.collider.transform.root.gameObject.layer!=11) {
					far_ps[i].transform.position=rh.point;
					far_ps[i].Emit(5);
					}
					else {
						Global.WaterExpl(rh.point);
					}
					lrs[i].SetPosition(1,rh.point);
				}
				else {
					lrs[i].SetPosition(1,gun_points[i].position+transform.forward*range);
				}
			}
		}
		else {
			if (temperature>0) {
				temperature-=temp_speed*0.75f*Time.deltaTime;
				if (temperature<0) temperature=0;
			}
		}
	}

	public void Death() {
		Destroy(near_ps);
		Destroy(bullets_fall);
		foreach (LineRenderer lr in lrs) {Destroy(lr);}
		Global.sm.GunSound(right,true,false);
	}

	void OnGUI () {
		if (Global.pause||!Global.playable) return;
		Vector2 mpos=Input.mousePosition;
		mpos.y=Screen.height-mpos.y;
		float t=temperature/max_temperature;
		if (right) {
			if (t>=0.9f) {
				GUI.contentColor=Color.red;
				GUI.Label(new Rect(mpos.x+2*k,mpos.y+2*k,4*k,k),"OVERHEAT");
			}
			else {
			GUI.DrawTexture(new Rect(mpos.x,mpos.y-k,4*k,2*k),Global.aim_tx_right,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x+3*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			GUI.contentColor=Color.black;
			GUI.Label(new Rect(mpos.x+2*k,mpos.y+2*k,4*k,k),((int)(ammo)).ToString());
			}
		}
		else {
			if (t>=0.9f) {
				GUI.contentColor=Color.red;
				GUI.Label(new Rect(mpos.x-4*k,mpos.y+2*k,4*k,k),"OVERHEAT");
			}
			else {
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y-k,4*k,2*k),Global.aim_tx_left,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			GUI.contentColor=Color.black;
			GUI.Label(new Rect(mpos.x-4*k,mpos.y+2*k,4*k,k),((int)(ammo)).ToString());
			}
		}
	}
}
