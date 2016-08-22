using UnityEngine;
using System.Collections;

public class ship : MonoBehaviour {
	public int maxhp=7000;
	float hp;
	public int points=3000;
	bool stand=true;
	Quaternion rotateTo;
	public GameObject oil_spot;
	// Use this for initialization
	void Start () {
		hp=maxhp;
	}

	void Update () {
		if (Global.pause) return;
		if (rotateTo!=transform.rotation) transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,60*Time.deltaTime);
		if (transform.position.y<-200) Destroy(gameObject);
	}

	public void Flatten(Vector3 pos) {
		if (stand) {
			stand=false;
			rotateTo=Quaternion.FromToRotation(Vector3.up,transform.position-pos);
		}
	}

	public void ApplyDamage(Vector4 mg) {
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		hp-=mg.w;
		if (hp<100) {
			rotateTo=Quaternion.FromToRotation(Vector3.up,transform.position-point);
			stand=false;
		}
		if (hp<=0) {
			var layerMask=1<<11;
			RaycastHit rh;
			if (Physics.Raycast(transform.position+Vector3.up*10,Vector3.down,out rh,200,layerMask)) {
				Instantiate(oil_spot,rh.point,Quaternion.Euler(0,Random.value*360,0));
			}
			Global.score+=points;
			Destroy(gameObject);
		}
	}
}
