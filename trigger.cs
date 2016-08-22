using UnityEngine;
using System.Collections;

public class trigger : MonoBehaviour {
	public Vector3[] points;
	public GameObject prefab;
	public int tall=10;
	bool reacted=false;


	void Update() {
		if (Global.pause||!Global.playable||!Global.player||reacted) return;
		if (Vector3.Distance(transform.position,Global.player.transform.position)<900) {
			RaycastHit rh;
			var layerMask=1<<8;
			foreach (Vector3 p in points) {
				Vector3 rp=transform.TransformPoint(p.x,500,p.z);
				if (Physics.Raycast(rp,Vector3.down,out rh,1000,layerMask)) {
					Instantiate(prefab,rh.point+Vector3.up*tall,Quaternion.LookRotation(Global.player.transform.position-p));
				}
				else {
					Instantiate(prefab,new Vector3(p.x,0,p.z),Quaternion.LookRotation(Global.player.transform.position-p));
				}
			}
		}
	}
	

}
