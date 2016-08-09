using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour {
	public int active_distance=3000;
	public byte count=5;
	public float reload_time=60;
	public float delay=0.3f;
	public float maxhp=1000;
	float hp;
	public Transform point;
	public GameObject prefab;
	public GameObject flatten;
	bool spawning=false;
	public int points=4000;

	void Start () {hp=maxhp;}
	// Update is called once per frame
	void Update () {
		if (!Global.player||Global.pause) return;
		if (Vector3.Distance(transform.position,Global.player.transform.position)<=active_distance) {
			if (spawning==false) {spawning=true;StartCoroutine(StartSpawn());}
		}
		else {
			if (spawning) {
				spawning=false;
				StopCoroutine(StartSpawn());
			}
		}
	}

	IEnumerator StartSpawn() {
		yield return new WaitForSeconds(reload_time);
		for (byte i=0;i<count;i++) {
		Instantiate(prefab,point.position,transform.rotation);
			yield return new WaitForSeconds(delay);
		}
		if (spawning) {
			StartCoroutine(StartSpawn());
		}
	}

	public void Flatten(Vector3 pos) {
		RaycastHit rh;
		Vector3 point=transform.position;
		if (Physics.Raycast(transform.position+Vector3.up*100,Vector3.down,out rh,200)) {
			point=rh.point;
		}
		GameObject x=Instantiate(flatten,point,Quaternion.identity) as GameObject;
		x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
		Global.ConcreteDustRequest(transform.position);
		Global.score+=points*2;
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
				x.transform.position=transform.position+Vector3.down*9;
				x.transform.rotation=transform.rotation;
				Global.SmallExplosionRequest(transform.position);
				Global.ConcreteDustRequest(transform.position);
				Global.score+=points;
				Destroy(gameObject);
			}
		}
	}
}
