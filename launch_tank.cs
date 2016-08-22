using UnityEngine;
using System.Collections;

public class launch_tank : MonoBehaviour {
	public Transform gun;
	public float reload_time=25;
	public float maxspeed=20;
	float speed=0;
	public float ang_speed=4;
	public int range=700;
	bool ready=true;
	bool firing=false;
	Transform target;
	float maxhp=500;
	float hp;
	public int points=300;
	RaycastHit rm;
	LayerMask lm;

	void Start () {
		transform.parent=null;
		hp=maxhp;
		lm=1<<8;
	}

	void Update() {
		if (Global.pause) return;
		if (Physics.Raycast(transform.position+Vector3.up*5,Vector3.down,out rm,10,lm)) {
			float dt=rm.point.y-transform.position.y;
			if (Mathf.Abs(dt)<1) transform.position=new Vector3(transform.position.x,rm.point.y,transform.position.z);
			else {
				if (dt>0) transform.Translate(Vector3.up*Time.deltaTime);
				else transform.Translate(Vector3.down*9*Time.deltaTime);
			}
		}
		else {
			transform.Translate(Vector3.down*10*Time.deltaTime);
		}
		if (target==null) {
			if (Global.player!=null) target=Global.player.transform;
			else return;
		}
		float d=Vector3.Distance(target.position,transform.position);
		Quaternion rotateTo=transform.rotation;
		if (d<=range) {
			if (d<100) {
				rotateTo=Quaternion.LookRotation(transform.position-new Vector3(target.position.x,transform.position.y,target.position.z),Vector3.up);
				transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,ang_speed*Time.deltaTime);
				speed=maxspeed;
			}
			else {
				RaycastHit rh;
				if (ready&&!firing&&Quaternion.Angle(transform.rotation,rotateTo)<5&&!Physics.Raycast(gun.position,target.position-gun.position,out rh,d+1,lm)) {
					StartCoroutine(Fire());
				}
			speed=0;
			}
		}
		transform.Translate(new Vector3(0,0,speed*Time.deltaTime));

		if (transform.position.y<-200) Destroy(gameObject);
	}

	IEnumerator Fire() {
		ready=false;
		firing=true;
		GameObject x=null;
		yield return new WaitForSeconds(0.5f);
		Global.DustEffectRequest(gun.position);
		for (byte i=0;i<10;i++) {
		yield return new WaitForSeconds(0.1f);
			x=Instantiate(Global.r_simple_rocket,gun.position+gun.TransformDirection(Random.Range(-2,2),Random.Range(-2,2),2),gun.rotation) as GameObject;
			x.transform.forward=target.position-x.transform.position;
		}
		firing=false;
		yield return new WaitForSeconds(reload_time);
		ready=true;
	}

	public void Flatten(Vector3 pos) {
		GameObject x=Instantiate(Global.r_flatten_tank0,transform.position,Quaternion.identity) as GameObject;
		x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
		x.transform.rotation=Quaternion.Euler(90,transform.rotation.eulerAngles.y,0);
		if (!Global.sm.outside_off) {Global.sm.TankCrunch();}
		Global.ConcreteDustRequest(transform.position);
		Global.score+=2*points;
		Destroy(gameObject);
	}

	public void ApplyDamage(Vector4 mg) {
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		if (mg.w>maxhp) {
			Global.menu_script.AddFireplace(transform.position);
			Global.SmallExplosionRequest(transform.position);
			Global.score+=points;
			Destroy(gameObject);
		}
		else {
			hp-=mg.w;
			if (hp<0) {
				GameObject x=Instantiate(Global.r_dead_tank0) as GameObject;
				x.transform.position=transform.position;
				x.transform.rotation=transform.rotation;
				Global.SmallExplosionRequest(transform.position);
				Global.score+=points;
				Destroy(gameObject);
			}
		}
	}

	public void Provocate (Transform t) {
		target=t;
	}
}
