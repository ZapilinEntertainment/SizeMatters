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

	void Start () {
		projectors=new GameObject[steps_count];
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

		Instantiate(dust_effect,pos,transform.root.rotation);
		left_leg=!left_leg;


		if (Physics.Raycast(pos,Vector3.down,out hit,pos.y+2)) {
		Quaternion projectorRotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
			GameObject obj=projectors[step_index];
			if (obj==null) {
				obj= Instantiate(step_projector, hit.point + hit.normal * 0.25f, projectorRotation) as GameObject;
			}
			else {obj.transform.position=hit.point + hit.normal * 0.25f;obj.SetActive(true);}
		obj.transform.parent = hit.transform;
		obj.transform.rotation= Quaternion.Euler(obj.transform.eulerAngles.x, transform.root.eulerAngles.y, obj.transform.eulerAngles.y);
			step_index++;
			if (step_index>=steps_count) step_index=0;
		}}
}
