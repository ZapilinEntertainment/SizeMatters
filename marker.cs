using UnityEngine;
using System.Collections;

public class marker : MonoBehaviour {
	public float speed=5;
	public float jump=1;
	float starty=0;
	void Start () {
		starty=transform.position.y;
	}
	// Update is called once per frame
	void Update () {
		transform.Rotate(0,speed*Time.deltaTime,0,Space.Self);

		transform.position=new Vector3(transform.position.x,starty-jump/2+Mathf.PingPong(Time.time,jump),transform.position.z);
	}
}
