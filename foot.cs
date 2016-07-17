using UnityEngine;
using System.Collections;

public class foot : MonoBehaviour {

	void OnTriggerEnter (Collider c) {
		if (c.transform.root.tag=="unit"&&c.transform.root.position.y<transform.position.y||c.transform.root.tag=="decoration") {			
			c.transform.root.SendMessage("Flatten",transform.position,SendMessageOptions.DontRequireReceiver);}
	}
}
