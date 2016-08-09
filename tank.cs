using UnityEngine;
using System.Collections;

public class tank : MonoBehaviour {
	public Transform trunk;
	public Transform tower;
	public Transform gun;
	public GameObject main_mesh;
	GameObject lod_sprite;
	bool optimized=false;
	public float damage=50;
	public float reload_time=5;
	public float maxspeed=20;
	float speed=0;
	public float ang_speed=4;
	public float gun_rot_speed=50;
	public int range=500;
	bool ready=true;
	Vector3 target_pos;
	float maxhp=300;
	float hp;
	public int points=200;
	RaycastHit rm;
	LayerMask lm;

	void Start () {
		transform.parent=null;
		hp=maxhp;
		lod_sprite=Instantiate(Global.r_tank0_lod_sprite,transform.position,transform.rotation) as GameObject;
		lod_sprite.transform.parent=transform;
		lod_sprite.SetActive(false);
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
		if (!Global.player) return;
		float d=Vector3.Distance(Global.cam.transform.position,transform.position);
		if (d>500) {
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
			tower.transform.rotation=Quaternion.RotateTowards(tower.transform.rotation,Quaternion.LookRotation(new Vector3(target_pos.x,tower.transform.position.y,target_pos.z)-tower.transform.position,Vector3.up),gun_rot_speed*Time.deltaTime);
			trunk.transform.forward=target_pos-trunk.transform.position;
			if (ready) {
				RaycastHit rh;
				if (Physics.Raycast(gun.transform.position,target_pos-gun.transform.position,out rh, range)) {
					StartCoroutine(Fire());
					ready=false;
				}
			}
		}
		if (Vector3.Distance(target_pos,transform.position)>range/2) {
			Quaternion rotateTo=Quaternion.LookRotation(new Vector3(target_pos.x,transform.position.y,target_pos.z)-transform.position,Vector3.up);
			transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,ang_speed*Time.deltaTime);
			if (Quaternion.Angle(transform.rotation,rotateTo)<15) speed=maxspeed;
		}
		else {
			speed=0;
		}
		target_pos=Global.player.transform.position;
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
		Vector3 expoint=trunk.forward*range;
		if (Physics.Raycast(gun.transform.position,target_pos-trunk.transform.position+Random.onUnitSphere*5,out rh, range)) {
			expoint=rh.point;
			rh.collider.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
		}
		Global.ShotExplEffectRequest(expoint);
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
}
