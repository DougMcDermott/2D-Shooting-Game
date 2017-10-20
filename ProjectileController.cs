using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	private Rigidbody2D rb2d;
	GameObject enemy;

	public float rocketSpeed;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D> ();

		//apply instantaneous force to projectile when it is spawned and shoot in the correct direction
		float direction;
		if (transform.localRotation.z != 0) {
			direction = -1;
		} else {
			direction = 1;
		}
		rb2d.AddForce (new Vector2 (1f * direction, 0f) * rocketSpeed, ForceMode2D.Impulse);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.CompareTag ("Terrain")) {
			Destroy (gameObject);
		}

		if (other.gameObject.CompareTag ("Zombie")) {
			ZombieController zombie = other.GetComponent<ZombieController> ();
			zombie.zombieHealth--;
			zombie.zombieHit = true;
			Destroy (gameObject);
		}

		if (other.gameObject.CompareTag ("Bat")) {
			BatController bat = other.GetComponent<BatController> ();
			bat.batHealth--;
			bat.batHit = true;
			Destroy (gameObject);
		}
	}

}
