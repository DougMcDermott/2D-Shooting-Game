using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour {

	public GameObject player;
	public float buttonPressDis;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			float disToPlayer = Vector2.Distance (transform.position, player.transform.position);
			if (disToPlayer < buttonPressDis) {
				print (disToPlayer);
			}
		}
	}
}
