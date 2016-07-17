using UnityEngine;
using System.Collections;

public class tree : MonoBehaviour {
	public Transform mesh;
	public float maxsize=5;
	public float minsize=0.8f;
	bool stand=true;
	Quaternion rotateTo;
	// Use this for initialization
	void Start () {
		mesh.transform.localScale=Vector3.one*Random.Range(minsize,maxsize);;
		transform.rotation=Quaternion.Euler(0,Random.value*360,0);
		transform.parent=null;
	}

	void Update () {
		if (rotateTo!=transform.rotation) transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,60*Time.deltaTime);
	}

	public void Flatten(Vector3 pos) {
		if (!stand) {
		GameObject x=Instantiate(Global.r_flatten_tree,transform.position,Quaternion.identity) as GameObject;
			x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
			x.transform.rotation=Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
		x.transform.localScale=mesh.transform.localScale;
		Destroy(gameObject);
		}
		else {
			stand=false;
			rotateTo=Quaternion.FromToRotation(Vector3.up,transform.position-pos);
		}
	}

	public void ApplyDamage(Vector4 mg) {
		Vector3 point=new Vector3(mg.x,mg.y,mg.z);
		if (mg.w<100) {
			rotateTo=Quaternion.FromToRotation(Vector3.up,transform.position-point);
			stand=false;
		}
		else {
			GameObject x=Instantiate(Global.r_fired_place,transform.position,Quaternion.identity) as GameObject;
			x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
			x.transform.rotation=Quaternion.Euler(90,Random.value*360,0);
			Destroy(gameObject);
		}
	}
}
