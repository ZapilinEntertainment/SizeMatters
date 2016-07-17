using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour {
	public Transform pauldron;
	public Transform gun_trunk;
	public Transform gun_point;
	public LineRenderer lr;
	public Texture screen_flare;
	public ParticleSystem near_ps;
	public ParticleSystem far_ps;
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
	Vector3 prevpos;

	void Start () {
		lr.SetWidth(8,3);
		if (gun_renderer) normal_light_map=gun_renderer.material.GetTexture("_EmissionMap");
	}

	void Update() {
		if (Global.pause) return;
		Quaternion rotateTo=Quaternion.LookRotation(Global.aim-pauldron.transform.position);
		Vector3 ag=rotateTo.eulerAngles;
		ag.y=0;
		ag.z=0;
		rotateTo=Quaternion.Euler(ag.x,0,0);
		Vector3 tpos=Global.aim;
		float a=Vector3.Angle(pauldron.forward,tpos-pauldron.position);
		if (a>45) tpos=prevpos;
		else prevpos=tpos;
		gun_trunk.transform.forward=tpos-gun_trunk.transform.position;
		if (Input.GetMouseButton(0)&&!right||Input.GetMouseButton(1)&&right) 
			{
			if (!firing) {
				lr.enabled=true;
				firing=true;
				if (gun_renderer) gun_renderer.material.SetTexture("_EmissionMap",firelight_map);
				near_ps.Play();
				if (far_sprite) far_sprite.SetActive(true);
			}
			else {
				RaycastHit prh;
				if (Physics.SphereCast(gun_point.position,ray_thick,gun_trunk.forward,out prh,10000)) {
					if (!endfire) {
						endfire=true;
						far_ps.Play();
						last_fired_point=prh.point;
						if (projector) Instantiate(projector,prh.point+Vector3.up,Quaternion.Euler(90,Random.value*360,0));
					}
					if (prh.collider.transform.root.gameObject.layer!=8) prh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(prh.point.x,prh.point.y,prh.point.z,1500),SendMessageOptions.DontRequireReceiver);
					else {
						if (Vector3.Distance(last_fired_point,prh.point)>=fp_dist) {
							last_fired_point=prh.point;
							if (projector) Instantiate(projector,prh.point+Vector3.up,Quaternion.Euler(90,Random.value*360,0));
						}
					}
				}
				else {endfire=false;far_ps.Stop();}
				if (endfire) {far_ps.transform.position=prh.point;if (far_sprite) far_sprite.transform.position=prh.point;}
				else {if (far_sprite) far_sprite.transform.position=gun_point.position+gun_trunk.forward*10000;}
			}
			lr.SetPosition(0,gun_point.position);
			lr.SetPosition(1,gun_point.position+gun_trunk.forward*10000);
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
				if (far_sprite) far_sprite.SetActive(false);
			}
		}
	}
}
