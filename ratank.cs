using UnityEngine;
using System.Collections;

public class ratank : MonoBehaviour {
	public Transform gunwheel;
	public Transform tower;
	public Transform gun;
	public Transform launcher_a;
	public Transform launcher_b;
	public float damage=50;
	public float reload_time=15;
	public float maxspeed=15;
	float speed=0;
	public float ang_speed=4;
	public float gun_rot_speed=30;
	public int range=600;
	bool ready=true;
	Transform target;
	float maxhp=92000;
	float hp;
	public int points=2000;
	RaycastHit rm;
	LayerMask lm;
	bool optimized=false;
	public GameObject main_mesh;
	public GameObject lod_sprite;

	void Start () {
		transform.parent=null;
		hp=maxhp;
		lm=1<<8;
		lod_sprite=Instantiate(Resources.Load<GameObject>("ratank_lod_sprite"),transform.position+Vector3.up*4.5f,transform.rotation) as GameObject;
		lod_sprite.transform.parent=transform;
		lod_sprite.SetActive(false);
	}

	void Update() {
		if (Global.pause) return;
		if (Physics.Raycast(transform.position+Vector3.up*5,Vector3.down,out rm,10,lm)) {
			float dt=rm.point.y-transform.position.y+6;
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
		float d=Vector3.Distance(Global.cam.transform.position,transform.position);
		if (d>800) {
			if (!optimized) {
				main_mesh.SetActive(false);
				tower.gameObject.SetActive(false);
				lod_sprite.SetActive(true);
				optimized=true;
			}
		}
		else {
			if (optimized) {
				main_mesh.SetActive(true);
				tower.gameObject.SetActive(true);
				lod_sprite.SetActive(false);
				optimized=false;
			}
		}
		tower.transform.rotation=Quaternion.RotateTowards(tower.transform.rotation,Quaternion.LookRotation(new Vector3(target.position.x,tower.transform.position.y,target.position.z)-tower.transform.position,Vector3.up),gun_rot_speed*Time.deltaTime);
		gunwheel.transform.forward=target.position-gunwheel.transform.position;
			if (ready) {
				RaycastHit rh;
			if (Physics.Raycast(gunwheel.transform.position,target.position-gunwheel.transform.position,out rh, range)) {
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
		if (transform.position.y<-200) Destroy(gameObject);
	}

	IEnumerator Fire() {
		ready=false;
		yield return new WaitForSeconds(0.5f);
		Global.ShotEffectRequest(gun.transform.position);
		if (Global.sm&&Global.sm.use_sound&&!Global.sm.outside_off) {
			if (Global.cam.transform.InverseTransformPoint(transform.position).z>0) {
				Global.sm.TankShot(1);
			}
			else {Global.sm.TankShot(2);}
		}
		yield return new WaitForSeconds(0.1f);
		RaycastHit rh;
		Vector3 expoint=gunwheel.forward*range;
		if (Physics.Raycast(gun.transform.position,target.position-gunwheel.transform.position+Random.onUnitSphere*5,out rh, range)) {
			expoint=rh.point;
			rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
		}
		Global.ShotExplEffectRequest(expoint);
		Global.DustEffectRequest(launcher_a.position);
		Global.DustEffectRequest(launcher_b.position);
		GameObject x=null;
		for (byte i=0;i<10;i++) {
			yield return new WaitForSeconds(0.1f);
			x=Instantiate(Global.r_simple_rocket,launcher_a.position+gun.TransformDirection(Random.Range(-2,2),Random.Range(-2,2),2),launcher_a.rotation) as GameObject;
			x.transform.forward=target.position-x.transform.position;
			x=Instantiate(Global.r_simple_rocket,launcher_b.position+gun.TransformDirection(Random.Range(-2,2),Random.Range(-2,2),2),launcher_b.rotation) as GameObject;
			x.transform.forward=target.position-x.transform.position;
		}
		yield return new WaitForSeconds(reload_time);
		ready=true;
	}

	public void Flatten(Vector3 pos) {
		if (!Global.sm.outside_off) {Global.sm.TankCrunch();}
		Global.BigDustRequest(transform.position);
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
