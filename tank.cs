using UnityEngine;
using System.Collections;

public class tank : MonoBehaviour {
	public Transform trunk;
	public Transform tower;
	public Transform gun;
	public float damage=50;
	public float reload_time=5;
	public float maxspeed=20;
	float speed=0;
	public float ang_speed=4;
	public float gun_rot_speed=50;
	public int range=500;
	bool ready=true;
	Vector3 target_pos;

	void Start () {
		transform.parent=null;
	}

	void Update() {
		if (!Global.player) return;
		target_pos=Global.player.transform.position;
		if (Vector3.Distance(target_pos,transform.position)>range/2) {
			Quaternion rotateTo=Quaternion.LookRotation(new Vector3(target_pos.x,transform.position.y,target_pos.z)-transform.position,Vector3.up);
			transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,ang_speed*Time.deltaTime);
			if (Quaternion.Angle(transform.rotation,rotateTo)<15) speed=maxspeed;
		}
		else {
			speed=0;
		}
		transform.Translate(new Vector3(0,0,speed*Time.deltaTime));
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

	IEnumerator Fire() {
		ready=false;
		yield return new WaitForSeconds(0.5f);
		Global.ShotEffectRequest(gun.transform.position);
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
		Destroy(gameObject);
	}

	public void ApplyDamage(Vector4 mg) {
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		if (mg.w>500) {
			GameObject x=Instantiate(Global.r_fired_place,transform.position,Quaternion.identity) as GameObject;
			x.transform.position=new Vector3(transform.position.x,0.2f,transform.position.z);
			x.transform.rotation=Quaternion.Euler(90,Random.value*360,0);
			Destroy(gameObject);
		}
	}
}
