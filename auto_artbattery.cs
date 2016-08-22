using UnityEngine;
using System.Collections;

public class auto_artbattery : MonoBehaviour {
	public Transform tower;
	public Transform gunwheel;
	public Transform guns;
	public ParticleSystem fire_ps;
	public float rot_speed=70;
	public int range=1000;
	public float damage=500;
	public bool ready=true;
	public float reload_time=12;



	// Update is called once per frame
	void Update () {
		if (Global.pause||!Global.playable) return;
		Vector3 rt=Quaternion.LookRotation(Global.player.transform.position-transform.position).eulerAngles;
		tower.rotation=Quaternion.RotateTowards(tower.rotation,Quaternion.Euler(new Vector3(0,rt.y,0)),rot_speed*Time.deltaTime);
		rt.z=0;rt.y=tower.rotation.eulerAngles.y;
		gunwheel.rotation=Quaternion.RotateTowards(gunwheel.rotation,Quaternion.Euler(rt),rot_speed*Time.deltaTime);
		if (ready) {
			RaycastHit rh;
			if (Physics.Raycast(guns.position+guns.forward,gunwheel.forward,out rh,range)&&rh.collider.transform.root.GetComponent<catmech_physics>()) {
				ready=false;
				StartCoroutine(Fire());
			}
		}
	}

	IEnumerator Fire() {
		//if (Global.sm&&Global.sound) Global.sm.MyArtShot();
		ready=false;
		yield return new WaitForSeconds(0.2f);
		RaycastHit rh;
		Vector3 endpos=Vector3.zero;
		fire_ps.Emit(25);
		if (Physics.Raycast(guns.position,gunwheel.forward,out rh,range)) {
			endpos=rh.point;
			Collider[] cds=Physics.OverlapSphere(rh.point,30);
			foreach (Collider cd in cds) {
				cd.transform.root.SendMessage("ApplyDamage",new Vector4(rh.point.x,rh.point.y,rh.point.z,damage),SendMessageOptions.DontRequireReceiver);
			}
		}
		else endpos=guns.position+guns.forward*range;
		Global.SmallExplosionRequest(endpos);
		yield return new WaitForSeconds(reload_time);
		ready=true;
	}


}
