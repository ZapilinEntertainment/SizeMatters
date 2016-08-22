using UnityEngine;
using System.Collections;

public class m6_triggers : MonoBehaviour {
	public Transform shield_center;
	public Collider shield;
	public GameObject[] sh_gens;
	public GameObject aps;
	public GameObject atomic_explosion;
	Vector3 aps_pos;
	bool boom=false;
	bool shield_off=false;
	public GameObject plane_pref;
	float ae_r=0;
	float ae_speed=400;
	public int ae_damage=1000;
	bool prevsound=false;

	void Start () {
		StartCoroutine(PlaneSpawn());
		aps_pos=aps.transform.position;
	}
	// Update is called once per frame
	void Update () {
		if (Global.pause||Global.nongamescene) return;
		if (!shield_off) {
		bool a=true;
		foreach (GameObject x in sh_gens) {
			if (x!=null) {
				a=false;
				break;
			}
		}
			if (a==true)  {Destroy(shield.gameObject);shield_off=true;}
			else {
				if (Vector3.Distance(Global.player.transform.position,shield_center.position)<1000) {
					shield.enabled=false;
				}
				else shield.enabled=true;
			}
			}
		if (!boom) {
			if (aps==null) {
				Instantiate(atomic_explosion,aps_pos,Quaternion.identity);
				boom=true;
				prevsound=Global.sound;
				if (prevsound) Global.sound=false;
			}
		}
		else {
			if (ae_r<2000) {				
				ae_r+=ae_speed*Time.deltaTime;
				Collider[] cds=Physics.OverlapSphere(aps_pos,ae_r);
				foreach (Collider cd in cds) {
					cd.transform.root.SendMessage("ApplyDamage",new Vector4(cd.transform.position.x,cd.transform.position.y,cd.transform.position.z,(1-Vector3.Distance(cd.transform.root.position,shield_center.position)/2000)*ae_damage*Time.deltaTime),SendMessageOptions.DontRequireReceiver);
				}
			}
			else {
				if (prevsound) {Global.sound=true;prevsound=false;}
			}
		}
	}

	IEnumerator PlaneSpawn() {
		yield return new WaitForSeconds(120);
		if (aps!=null) {
		Vector3 p=Vector3.zero;
		if (Random.value>0.5f) p=new Vector3(Random.value*4000,400+Random.value*400,6000);
		else p=new Vector3(-6000,400+Random.value*400,Random.value*4000);
		Instantiate(plane_pref,p,Quaternion.LookRotation(Global.player.transform.position-p));
		Instantiate(plane_pref,p-Vector3.forward*40,Quaternion.LookRotation(Global.player.transform.position-p));
		StartCoroutine(PlaneSpawn());
		}
	}		
}
