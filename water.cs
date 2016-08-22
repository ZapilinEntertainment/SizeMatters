using UnityEngine;
using System.Collections;

public class water : MonoBehaviour {
	public ParticleSystem water_ps;
	public int water_level=30;

	void Start () {
		water_ps=Instantiate(water_ps,Vector3.zero,Quaternion.Euler(-90,0,0)) as ParticleSystem;
	}



	public void OnTriggerEnter (Collider c) {
		water_ps.transform.position=c.transform.position;
		water_ps.Emit(50);
		if (c.transform.root.GetComponent<catmech_physics>()) {
			c.transform.root.BroadcastMessage("WaterTracing",this,SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnTriggerExit(Collider c) {
		if (c.transform.root.GetComponent<catmech_physics>()) {
			c.transform.root.BroadcastMessage("StopWaterTracing",this,SendMessageOptions.DontRequireReceiver);
		}
	}



	public void WaterSplash (Vector3 pos) {
		pos.y=water_level;
		water_ps.transform.position=pos;
		water_ps.Emit(50);
	}
}
