using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour {

	private Rigidbody2D rb2d;
	public Transform wallCheck;

	bool touchingWall;
	bool facingRight = true;
	float currentDirection = 1f;

	bool isAttacking = false;
	float distanceToPlayer;
	float playerXPos;
	float playerYPos;
	float attackRadius = 8f;
	float arcTime = 0;
	float maxArcTime = 2f; //2pi
	float yArcAttack;
	bool isCorrecting = false;
	float yCorrection;
	float yCorrectionTime = 0;
	float maxYCorrectionTime = 2f;

	public float batSpeed;
	float yArc = 3f;

	[HideInInspector] public int batHealth = 1;
	[HideInInspector] public bool batHit;
	int flashTime = 3;

	void Awake () {
		rb2d = GetComponent<Rigidbody2D> ();
		rb2d.freezeRotation = true;
		yCorrection = transform.position.y;
		batHealth = 1;
		batHit = false;
	}

	void FixedUpdate () {
		touchingWall = Physics2D.Linecast (transform.position, wallCheck.position, 1 << LayerMask.NameToLayer ("Wall"));
		playerXPos = GameObject.FindGameObjectWithTag ("Player").transform.position.x;
		playerYPos = GameObject.FindGameObjectWithTag ("Player").transform.position.y;
		distanceToPlayer = Mathf.Abs(Vector2.Distance (transform.position, GameObject.FindGameObjectWithTag ("Player").transform.position));
		float yValue;

		//if the bat hits the wall change the direction that it is flying in
		if (touchingWall || (distanceToPlayer < attackRadius && !isAttacking && ((currentDirection == 1 && playerXPos < transform.position.x)
			|| (currentDirection == -1 && playerXPos > transform.position.x)))) {
			if (currentDirection == 1) {
				currentDirection = -1;
				flip ();
			}
			else {
				currentDirection = 1;
				flip ();
			}
		}

		if (distanceToPlayer < attackRadius && !isAttacking) {
			isAttacking = true;
			float batY = transform.position.y;
			float difference = Mathf.Abs(batY - playerYPos);
			yArcAttack = Mathf.Abs (Mathf.Sin (difference/distanceToPlayer)) * 20f;
		}

		if (arcTime >= maxArcTime) {
			arcTime = 0f;
			isCorrecting = true;
			isAttacking = false;
		}

		if (yCorrectionTime >= maxYCorrectionTime || isAttacking) {
			yCorrectionTime = 0f;
			isCorrecting = false;
		}

		if (isAttacking) {
			arcTime += Time.deltaTime;
			float yDir = -1;
			if (arcTime >= maxArcTime/2) {
				yDir = 1;
			}
			yValue = Mathf.Pow((arcTime - (maxArcTime/2)), 2) * yDir * yArcAttack;
		} else {
			float modifier;
			if (isCorrecting) {
				yCorrectionTime += Time.deltaTime;
				if (transform.position.y < yCorrection) {
					modifier = 1.5f;
				} else {
					modifier = -1.5f;
				}
			} else {
				modifier = 0;
			}
			yValue = yArc * Mathf.Cos (yArc * Time.time) + modifier;
		}

		rb2d.velocity = new Vector2 (batSpeed * currentDirection, yValue);

		//Bat flashes white when hit by the player
		if (batHit) {
			flashTime--;
			gameObject.GetComponent<Renderer> ().material.color = new Color (100, 100, 100);
		} 
		if (flashTime <= 0) {
			flashTime = 3;
			batHit = false;
			gameObject.GetComponent<Renderer> ().material.color = new Color (0, 0, 0);
		}

		if (batHealth <= 0) {
			Destroy(gameObject);
		}
	}

	//function flips the orientation of the object
	void flip () {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
