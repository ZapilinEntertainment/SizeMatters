using UnityEngine;
using System.Collections;

public class step_tracer : MonoBehaviour {
	bool left_leg=true;
	public GameObject left_feet;
	public GameObject right_feet;
	public GameObject dust_effect;
	public GameObject step_projector;
	GameObject[] projectors;
	public uint steps_count=50;
	uint step_index=0;
	public bool make_traces=true;
	water waterscript;

	void Start () {
		projectors=new GameObject[steps_count];
		if (PlayerPrefs.HasKey("gdata")) {
			string s=PlayerPrefs.GetString("gdata");
			if (s.Length>1) {
				if (s[1]=='1') make_traces=true;
				else make_traces=false;
			}
			else make_traces=false;
		}
		else make_traces=false;
		if (make_traces) {if (Global.quality>=4) make_traces=true; else make_traces=false;}
	}

	public void MakeStepTrace() {
		RaycastHit hit;
		Vector3 pos;
		if (left_leg) {
			pos=left_feet.transform.position;
		}
		else {
			pos=right_feet.transform.position;
		}

		if (dust_effect!=null) Instantiate(dust_effect,pos,transform.root.rotation);

		var layerMask=1<<8;
		if (make_traces&&Physics.Raycast(pos,Vector3.down,out hit,pos.y+20,layerMask)) {
		Quaternion projectorRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
			GameObject obj=projectors[step_index];
			if (obj==null) {
				obj= Instantiate(step_projector, hit.point + hit.normal * 0.25f, projectorRotation) as GameObject;
				projectors[step_index]=obj;
			}
			else {obj.transform.position=hit.point + hit.normal * 0.25f;obj.SetActive(true);}
		obj.transform.parent = hit.transform;
		obj.transform.rotation= Quaternion.Euler(obj.transform.eulerAngles.x, transform.root.eulerAngles.y, obj.transform.eulerAngles.y);
			step_index++;
			if (step_index>=steps_count) step_index=0;
		}
		if (Global.sound) {
			StartCoroutine(StepDelay(pos,left_leg));
		}
		left_leg=!left_leg;
		if (waterscript!=null) waterscript.WaterSplash(pos);
	}

	IEnumerator StepDelay(Vector3 pos,bool left) {
		yield return new WaitForSeconds(0.5f);
		Global.sm.StepSound(pos,left);
	}

	public void WaterTracing( water w) {
		waterscript=w;
	}
	public void StopWaterTracing (water w) {
		if (waterscript==w) waterscript=null;
	}
}
