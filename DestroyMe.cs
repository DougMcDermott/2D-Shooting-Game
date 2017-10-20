using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour {

	public float timeAlive;

	//destroy object after timeAlive seconds
	void Awake () {
		Destroy (gameObject, timeAlive);
	}
}
