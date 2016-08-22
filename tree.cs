using UnityEngine;
using System.Collections;

public class tree : MonoBehaviour {
	public Transform mesh;
	public GameObject sprite;
	bool optimized=false;
	public float maxsize=5;
	public float minsize=0.8f;
	public bool stand=true;
	bool invisible=false;
	public Quaternion rotateTo;
	public int points = 100;
	// Use this for initialization
	void Start () {
		mesh.transform.localScale=Vector3.one*Random.Range(minsize,maxsize);;
		transform.rotation=Quaternion.Euler(0,Random.value*360,0);
		transform.parent=null;
		sprite=Instantiate(sprite,transform.position,transform.rotation) as GameObject;
		sprite.transform.parent=transform;
		sprite.transform.localScale=mesh.transform.localScale;
		sprite.SetActive(false);
	}

	void Update () {
		if (Global.pause||!Global.cam) return;
		float d=Vector3.Distance(Global.cam.transform.position,transform.position);
		if (d>1000) {
			if (d>4000) {
				if (!invisible) {invisible=true;mesh.gameObject.SetActive(false);sprite.SetActive(false);}
			}
			else {
				if (invisible) {
					invisible=false;mesh.gameObject.SetActive(false);
					sprite.SetActive(true);
					optimized=true;}
			}
			if (!optimized) {
				mesh.gameObject.SetActive(false);
				sprite.SetActive(true);
				optimized=true;
			}
		}
		else {
			if (optimized) {
				mesh.gameObject.SetActive(true);
				sprite.SetActive(false);
				optimized=false;
			}
		}
		if (rotateTo!=transform.rotation) transform.rotation=Quaternion.RotateTowards(transform.rotation,rotateTo,60*Time.deltaTime);
		if (transform.position.y<-200) Destroy(gameObject);
	}

	public void Flatten(Vector3 pos) {
		if (!stand) {
		GameObject x=Instantiate(Global.r_flatten_tree,transform.position,Quaternion.identity) as GameObject;
			x.transform.position=new Vector3(transform.position.x,0.1f,transform.position.z);
			x.transform.rotation=Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
		x.transform.localScale=mesh.transform.localScale;
			Global.score+=points*2;
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
			Global.menu_script.AddFireplace(transform.position);
			Destroy(gameObject);
			Global.score+=points;
		}
	}
}
