using UnityEngine;
using System.Collections;

public class shield_gen_module : MonoBehaviour {
	void Awake () {
		if (Global.nongamescene) Destroy(this);
	}
	// Use this for initialization
	void Start () {
		transform.root.GetComponent<catmech_physics>().shield_reg_speed*=1.1f;
		transform.root.GetComponent<catmech_physics>().maxshield*=1.1f;
	}

}
