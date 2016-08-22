using UnityEngine;
using System.Collections;

public class decoration : MonoBehaviour {
	public float maxhp=100;
	public float hp;
	public byte sound=0; //0-nothing, 1- falling tree, 2- metal scrap
	public GameObject flatten_corpse;
	public bool use_dust=false;
	public bool bigdust=false;
	public int points=100;

	void Start() {hp=maxhp;}

	void Update() {
		if (transform.position.y<-200) Destroy(gameObject);
	}

	public void Flatten(Vector3 pos) {
		if (sound!=0) {
			switch (sound) {
			case 1: break;
			case 2: Global.sm.TankCrunch(); break;
			}
		}
		if (flatten_corpse!=null) {
		GameObject x=Instantiate(flatten_corpse,transform.position,Quaternion.identity) as GameObject;
		x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
		x.transform.rotation=Quaternion.Euler(90,transform.rotation.eulerAngles.y,0);
		}
		Global.ConcreteDustRequest(transform.position);
		Global.score+=2*points;
		Destroy(gameObject);
	}

	public void ApplyDamage(Vector4 mg) {

		if (mg.w>maxhp) {
			Global.menu_script.AddFireplace(transform.position);
			Global.score+=points;
			Destroy(gameObject);
		}
		else {
			hp-=mg.w;
			if (hp<=0) {
				if (use_dust) {
					if (!bigdust) Global.ConcreteDustRequest(transform.position);
					else Global.BigDustRequest(transform.position);
				}
				Global.score+=points;
				Destroy(gameObject);
			}
		}
	}
		
}
