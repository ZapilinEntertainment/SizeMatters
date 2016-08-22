using UnityEngine;
using System.Collections;

public class building : MonoBehaviour {
	public float maxhp=100;
	public float hp;
	public bool use_dust=false;
	public int points=100;
	public int fires=0;
	float fd=0;
	float cd=0;
	public int tall=10;
	Vector3 null_point=Vector3.zero;


	void Start() {
		hp=maxhp;
		if (fires>0) {
			fd=maxhp/fires;
			cd=maxhp;
		}
		RaycastHit rh;
		var layerMask=1<<8;
		if (Physics.Raycast(transform.position+Vector3.up*100,Vector3.down,out rh,1000,layerMask)) null_point=rh.point;
		else {null_point=transform.position-Vector3.down*tall/2;}
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
					Vector3 p=null_point;
					while (tall>0) {
						p+=transform.TransformDirection(Vector3.up*10);
						Global.BigDustRequest(p);
						tall-=10;
					}
				}

				Destroy(gameObject);
			}

		}
		if (hp<cd) {
			GameObject x=Instantiate(Global.r_fire,new Vector3(mg.x,mg.y,mg.z),Quaternion.LookRotation(new Vector3(mg.x,mg.y,mg.z)-transform.position)) as GameObject;
			x.transform.parent=transform;
			if (cd>0) cd-=fd;
		}
	}

	void OnCollisionStay (Collision c) {
		if (c.collider.transform.root.GetComponent<catmech_physics>()) {
			print ("contact");
			hp-=10000*Time.deltaTime;
			if (Global.sound&&!Global.sm.outside_off)Global.sm.TankCrunch();
			if (hp<=0) {
				Global.score+=points;
				if (use_dust) {
					Vector3 p=null_point;
					while (tall>0) {
						p+=transform.TransformDirection(Vector3.up*10);
						Global.BigDustRequest(p);
						tall-=10;
					}
				}
				Destroy(gameObject);
			}
		}
	}

}
