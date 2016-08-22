using UnityEngine;
using System.Collections;

public class oil_platform : MonoBehaviour {
	public int maxhp=10000;
	float hp;
	public int points=10000;
	public GameObject[] buildings;
	public GameObject oil_spot;
	public GameObject explosion;
	bool broken=false;
	// Use this for initialization
	void Start () {
		hp=maxhp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ApplyDamage(Vector4 mg) {
		if (mg.w>maxhp) {
			Global.score+=points;
			Instantiate(explosion,transform.position,transform.rotation);
			Destroy(gameObject);
		}
		else {
			hp-=mg.w;
			if (hp<=0) {
				Instantiate(Resources.Load<GameObject>("bigdust"),transform.position,Quaternion.identity);
				RaycastHit rh;
				var layerMask=1<<11; //waterlayer
					if (Physics.Raycast(transform.position,Vector3.down,out rh,200,layerMask)) {
						Instantiate(oil_spot,rh.point,Quaternion.Euler(0,Random.value*360,0));
				}
				Global.score+=points;
				Destroy(gameObject);
			}
			else {
				if (hp/maxhp<0.25f&&!broken) {
					broken=true;
					GameObject x=Instantiate(Resources.Load<GameObject>("fire1"),transform.position,Quaternion.identity) as GameObject;
					x.transform.parent=transform;
					x=Instantiate(Resources.Load<GameObject>("smoke"),transform.position,Quaternion.identity) as GameObject;
					x.transform.parent=transform;
					foreach (GameObject g in buildings) {
						Destroy(g);
					}
				}
			}
		}
	}
}
