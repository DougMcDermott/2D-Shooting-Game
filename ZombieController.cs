using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour {
	/*
	 * ********************************************************************************
	 * Very basic AI where the zombie moves in the direction of the player
	 * If the zombie reaches an obstacle it cannot pass it will attempt to jump over it
	 * ********************************************************************************
	 */

	private Rigidbody2D rb2d;
	public Transform groundCheck;

	bool grounded = false;
	public float jumpForce = 12f;
	float zombieSpeed;

	float playerXPos;
	float currentZombieXPos;
	float lastZombieXPos;

	[HideInInspector] public int zombieHealth;
	[HideInInspector] public bool zombieHit;
	int flashTime = 3;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D> ();
		rb2d.freezeRotation = true;
		currentZombieXPos = transform.position.x;
		lastZombieXPos = currentZombieXPos;
		zombieSpeed = Random.Range (2f, 5f);
		zombieHealth = 3;
		zombieHit = false;
	}

	void FixedUpdate () {
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		playerXPos = GameObject.FindGameObjectWithTag ("Player").transform.position.x;
		currentZombieXPos = transform.position.x;

		//the zombie move in the direction of the player
		if (playerXPos > currentZombieXPos) {
			rb2d.velocity = new Vector2 (zombieSpeed, rb2d.velocity.y);
		} else {
			rb2d.velocity = new Vector2 (-zombieSpeed, rb2d.velocity.y);
		}

		//if the zombies x position is not changing, the zombie will jump to try and pass an obstacle
		if (currentZombieXPos == lastZombieXPos) {
			if (grounded) {
				rb2d.velocity = new Vector2 (rb2d.velocity.x, jumpForce);
			}
		}

		//update the position of the zombie to determine if it has moved
		lastZombieXPos = currentZombieXPos;

		//Zombie flashes white when hit by the player
		if (zombieHit) {
			flashTime--;
			gameObject.GetComponent<Renderer> ().material.color = new Color (100, 100, 100);
		} 
		if (flashTime <= 0) {
			flashTime = 3;
			zombieHit = false;
			gameObject.GetComponent<Renderer> ().material.color = new Color (0, 0, 0);
		}

		if (zombieHealth <= 0) {
			Destroy(gameObject);
		}
	}
}
