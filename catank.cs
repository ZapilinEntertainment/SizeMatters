using UnityEngine;
using System.Collections;

public class catank : MonoBehaviour {
	public Transform tower;
	public Transform gun;
	public Renderer[] legs;
	public Renderer[] towerhead;
	public Renderer[] ears;
	public Renderer body;
	Transform target;
	LineRenderer lr;
	public float damage=200;
	public float reload_time=5;
	public float maxspeed=30;
	float speed=0;
	public float ang_speed=4;
	public float gun_rot_speed=60;
	public int range=700;
	bool ready=true;
	float maxhp=2000;
	float hp;
	public int points=-1000;
	RaycastHit rm;
	LayerMask lm;
	catanks_base cb;
	bool dead=false;

	void Awake () {
		GameObject x=GameObject.Find("catanks_base");
		if (x!=null) {
			cb=x.GetComponent<catanks_base>();
		}
		else {
			x=new GameObject("catanks_base");
			cb=x.AddComponent<catanks_base>();
		}
	}

	void Start () {
		transform.parent=null;
		hp=maxhp;
		lm=1<<8;
	
		int a=Mathf.RoundToInt(Random.value*4)+1;
		foreach (Renderer r in legs) {
			r.material=cb.materials[a];
		}
		foreach (Renderer r in towerhead) {
			r.material=cb.materials[a];
		}
		foreach (Renderer r in ears) {
			r.material=cb.materials[a];
		}
		body.material=cb.materials[Mathf.RoundToInt(Random.value*4)+1];
		lr=gun.gameObject.AddComponent<LineRenderer>();
		lr.material=cb.materials[0];
		lr.enabled=false;
	}

	void Update() {
		if (lr.enabled) lr.SetPosition(0,gun.position);
		if (Global.pause||cb==null) return;
		if (Physics.Raycast(transform.position,Vector3.down,out rm,10,lm)) {
			transform.position=new Vector3(transform.position.x,rm.point.y+8,transform.position.z);
		}
		else {
			transform.Translate(Vector3.down*10*Time.deltaTime);
		}
		if (target!=null) {
				tower.transform.rotation=Quaternion.RotateTowards(tower.transform.rotation,Quaternion.LookRotation(target.position-transform.position,Vector3.up),gun_rot_speed*Time.deltaTime);
			if (ready) {
				RaycastHit rh;
				if (Physics.Raycast(gun.transform.position,target.position-gun.transform.position,out rh, range)&&rh.collider.tag=="unit") {
					StartCoroutine(Fire());
					ready=false;
				}
			}
		if (Vector3.Distance(target.position,transform.position)>range/2) {
			Quaternion rotateTo=Quaternion.LookRotation(new Vector3(target.position.x,transform.position.y,target.position.z)-transform.position,Vector3.up);
			transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,ang_speed*Time.deltaTime);
			if (Quaternion.Angle(transform.rotation,rotateTo)<15) speed=maxspeed;
		}
		else {
			speed=0;
		}
		transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
		}
		else {
			target=cb.GiveMeATarget(transform.position);
		}

		if (transform.position.y<-200) Destroy(gameObject);
	}

	IEnumerator Fire() {
		ready=false;
		yield return new WaitForSeconds(0.5f);
		RaycastHit rh;
		Vector3 expoint=tower.forward*range+transform.position;
		if (Physics.Raycast(gun.transform.position,target.position-gun.transform.position,out rh, range)) {
			expoint=rh.point;
			rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
			if (rh.collider.transform.root!=null) {
				rh.collider.transform.root.SendMessage("Provocate",transform,SendMessageOptions.DontRequireReceiver);
			}
		}
		lr.SetPosition(1,expoint);
		lr.enabled=true;
		yield return new WaitForSeconds(1);
		lr.enabled=false;
		yield return new WaitForSeconds(reload_time);
		ready=true;
	}

	public void Flatten(Vector3 pos) {
		if (!Global.sm.outside_off) {Global.sm.TankCrunch();}
		Global.BigDustRequest(transform.position);
		Global.SmallExplosionRequest(transform.position);
		Destroy(gameObject);
	}

	public void ApplyDamage(Vector4 mg) {
		if (dead) return;
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		if (mg.w>maxhp) {
			dead=true;
			Global.menu_script.AddFireplace(transform.position);
			Global.SmallExplosionRequest(transform.position);
			Global.score+=points;
			Destroy(gameObject);
		}
		else {
			hp-=mg.w;
			if (hp<0) {
				dead=true;
				tree dcr=gameObject.AddComponent<tree>();
				dcr.stand=false;
				dcr.rotateTo=Quaternion.LookRotation(transform.forward,Random.onUnitSphere);
				Rigidbody r=gameObject.AddComponent<Rigidbody>();
				r.mass=50;
				Destroy(this);
			}
		}
	}
}
