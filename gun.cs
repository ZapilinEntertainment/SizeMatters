using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour {
	public Transform pauldron;
	public Transform gun_trunk;
	public Transform gun_point;
	public LineRenderer lr;
	public ParticleSystem near_ps;
	ParticleSystem far_ps;
	public GameObject far_sprite;
	public Renderer gun_renderer;
	public Texture firelight_map;
	public GameObject projector;
	public bool right;
	public float ray_thick=2;
	Texture normal_light_map;
	Vector3 last_fired_point=Vector3.zero;
	float fp_dist=1;
	bool firing=false;
	bool endfire=false;
	public float angle_border=60;
	Quaternion rotateTo;
	public float damage=1500;
	public int range=1000;
	public AudioClip sound;
	public float max_temperature=200;
	public float temp_speed=20;
	float temperature=0;
	int k=16;

	void Start () {
		if (Global.nongamescene) {Destroy(this);return;}
		pauldron=transform.parent;
		if (transform.root.InverseTransformPoint(transform.position).x>0) {right=true;} else right=false;
		lr.SetWidth(8,3);
		if (gun_renderer) normal_light_map=gun_renderer.material.GetTexture("_EmissionMap");
		far_sprite=Instantiate(far_sprite) as GameObject;
		far_ps=Instantiate(Resources.Load<ParticleSystem>("plasma_far_pref")) as ParticleSystem;
		if (sound) {
		    if (right) Global.sm.rightgun_sound=sound;
			else Global.sm.leftgun_sound=sound;
		}
		k=Screen.height/27;
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
				if (temperature<max_temperature*0.9f) {
				lr.enabled=true;
				firing=true;
				if (gun_renderer) gun_renderer.material.SetTexture("_EmissionMap",firelight_map);
				near_ps.Play();
				far_sprite.SetActive(true);
				Global.sm.GunSound(right,true,true);
				}
			}
			else {
				if (temperature>=max_temperature) {
					firing=false;
					if (gun_renderer) gun_renderer.material.SetTexture("_EmissionMap",normal_light_map);
					lr.enabled=false;
					near_ps.Stop();
					far_ps.Stop();
					far_sprite.SetActive(false);
					Global.sm.GunSound(right,true,false);
				}
				else {
				RaycastHit prh;
				if (Physics.SphereCast(gun_point.position,30,gun_trunk.forward,out prh,range)) {
					if (prh.collider.transform.root.gameObject.layer!=8) prh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(prh.point.x,prh.point.y,prh.point.z,damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);
				}
				if (Physics.Raycast(gun_point.position,gun_trunk.forward,out prh,range)) {
				if (!endfire) {
					endfire=true;
					far_ps.Play();
					last_fired_point=prh.point;
					if (projector) Instantiate(projector,prh.point+Vector3.up,Quaternion.Euler(90,Random.value*360,0));
				}
				if (prh.collider.transform.root.gameObject.layer!=8) prh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(prh.point.x,prh.point.y,prh.point.z,damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);
				else {
						if (Global.quality>=4&&Vector3.Distance(last_fired_point,prh.point)>=fp_dist) {
						last_fired_point=prh.point;
						if (projector) Instantiate(projector,prh.point+Vector3.up,Quaternion.Euler(90,Random.value*360,0));
					}
					}
						Collider[] cds=Physics.OverlapSphere(prh.point,30);
						foreach (Collider cd in cds) {
							if (cd.transform.root.gameObject.layer!=8) prh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(prh.point.x,prh.point.y,prh.point.z,damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);
						}
					}
				else {endfire=false;far_ps.Stop();}
				if (endfire) {far_ps.transform.position=prh.point+new Vector3(0,ray_thick/2,1);far_sprite.transform.position=prh.point;}
				else {far_sprite.transform.position=gun_point.position+gun_trunk.forward*range;}
					temperature+=temp_speed*Time.deltaTime;
				}}
			lr.SetPosition(0,gun_point.position);
			lr.SetPosition(1,gun_point.position+gun_trunk.forward*range);
			near_ps.transform.position=gun_point.position;
			}
		else {
			endfire=false;
			if (firing) {
			    firing=false;
				if (gun_renderer) gun_renderer.material.SetTexture("_EmissionMap",normal_light_map);
				lr.enabled=false;
				near_ps.Stop();
				far_ps.Stop();
				far_sprite.SetActive(false);
				Global.sm.GunSound(right,true,false);
			}
		}
		if (!firing&&temperature>0) {
			temperature-=temp_speed*0.75f*Time.deltaTime;
			if (temperature<0) temperature=0;
		}
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
				GUI.contentColor=Color.black;
			}
			else {
			GUI.DrawTexture(new Rect(mpos.x,mpos.y-k,4*k,2*k),Global.aim_tx_right,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x+3*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			}
		}
		else {
			if (t>=0.9f) {
				GUI.contentColor=Color.red;
				GUI.Label(new Rect(mpos.x-4*k,mpos.y+2*k,4*k,k),"OVERHEAT");
				GUI.contentColor=Color.black;
			}
			else {
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y-k,4*k,2*k),Global.aim_tx_left,ScaleMode.StretchToFill);
			GUI.DrawTexture(new Rect(mpos.x-4*k,mpos.y+k/2-2*k*t,k,k),Global.aim_slider_tx,ScaleMode.StretchToFill);
			}
		}
	}
}
