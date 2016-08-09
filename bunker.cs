using UnityEngine;
using System.Collections;

public class bunker : MonoBehaviour {
	public int range=3200;
	public float lift_speed=15;
	public float reload_time=5;
	public Transform point_r;
	public Transform point_l;
	GameObject right_rocket;
	GameObject left_rocket;
	public float height=5;
	public float maxhp=1000;
	float hp;
	float pos=0;
	bool lift=false;
	bool ready=true;
	public int points=2000;

	void Start () {
		hp=maxhp;
	}
	// Update is called once per frame
	void Update () {
		if (Global.pause) return;
		if (!Global.player||!ready) return;
		if (Vector3.Distance(transform.position,Global.player.transform.position)<range&&transform.InverseTransformPoint(Global.player.transform.position).z>0)
		{
			if (!lift){
				right_rocket=Instantiate(Global.r_homing_missile,point_r.position,transform.rotation) as GameObject;
			right_rocket.transform.parent=transform;
				left_rocket=Instantiate(Global.r_homing_missile,point_l.position,transform.rotation) as GameObject;
			left_rocket.transform.parent=transform;
				pos=0;
				lift=true;
			}
		}
		else {
			lift=false;
		}
		if (lift) {
			if (pos<height) {
				pos+=lift_speed*Time.deltaTime;
				right_rocket.transform.Translate(new Vector3(0,lift_speed*Time.deltaTime,0));
				left_rocket.transform.Translate(new Vector3(0,lift_speed*Time.deltaTime,0));
			}
			else {lift=false;Fire();}
		}
		else {
			if (right_rocket) {
				if (pos>0) {
					pos-=lift_speed*Time.deltaTime;
					right_rocket.transform.Translate(new Vector3(0,-lift_speed*Time.deltaTime,0));
					left_rocket.transform.Translate(new Vector3(0,-lift_speed*Time.deltaTime,0));
				}
				else {
					Destroy(right_rocket);
					Destroy(left_rocket);
				}
			}
		}

	}

	IEnumerator FireCycle() {
		yield return new WaitForSeconds(reload_time);
		ready=true;
	}

	void Fire() {
		homingMissile hmc;
		hmc=right_rocket.GetComponent<homingMissile>();
		hmc.target=Global.player.transform;
		hmc.enabled=true;
		right_rocket.transform.parent=null;
		right_rocket=null;
		hmc=left_rocket.GetComponent<homingMissile>();
		hmc.target=Global.player.transform;
		hmc.enabled=true;
		left_rocket.transform.parent=null;
		left_rocket=null;
		ready=false;
		StartCoroutine(FireCycle());
	}

	public void Flatten(Vector3 pos) {
		GameObject x=Instantiate(Global.r_flatten_tank0,transform.position,Quaternion.identity) as GameObject;
		x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
		x.transform.rotation=Quaternion.Euler(90,transform.rotation.eulerAngles.y,0);
		Global.ConcreteDustRequest(transform.position);
		Global.score+=2*points;
		Destroy(gameObject);
	}

	public void ApplyDamage(Vector4 mg) {
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		if (mg.w>maxhp) {
			Global.menu_script.AddFireplace(transform.position);
			Global.SmallExplosionRequest(transform.position);
			Global.ConcreteDustRequest(transform.position);
			Global.score+=points;
			Destroy(gameObject);
		}
		else {
			hp-=mg.w;
			if (hp<0) {
				GameObject x=Instantiate(Global.r_dead_bunker0) as GameObject;
				x.transform.position=transform.position;
				x.transform.rotation=transform.rotation;
				Global.SmallExplosionRequest(transform.position);
				Global.ConcreteDustRequest(transform.position);
				Global.score+=points;
				Destroy(gameObject);
			}
		}
	}
}
