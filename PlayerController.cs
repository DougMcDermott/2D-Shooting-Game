using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody2D rb2d;
	//private Animator anim;

	public Transform groundCheck;
	public Transform rightWallCheck;
	public Transform leftWallCheck;

	//for jumping
	bool grounded = false;
	bool doubleJumpAvailable = false;
	bool wallJumpAvailable = false;
	bool touchingRightWall = false;
	bool touchingLeftWall = false;
	bool jump = false;

	public float firstJumpForce = 20f;
	public float secondJumpForce = 15f;
	public float wallJumpForce = 20f;
	float jumpForce;
	float waitTime = 0.5f;
	float timeAfterWallJump = 0.25f;

	//for jetpack
	public float jetpackDrag = 50f;
	int jetpackFuel;
	public int jetpackFuelMax = 100;
	public Text infoText;

	//for horizontal movement
	public float moveSpeed = 7f;

	//for shooting
	public Transform shootPoint;
	public GameObject bullet;
	float fireRate = 0.2f;
	float timeBetweenShots = 0.5f;
	int currentAmmo = 16;
	int maxAmmo = 16;
	float reloadTime = 2f;
	float timeOfNextReload = 0f;
	bool isReloading = false;

	//Monster Interaction
	[HideInInspector] public int playerHealth;
	bool invulnerable = false;
	int flashCounter = 0;
	int knockBackDirection;
	float knockbackTime = 0.15f;
	float timeAfterKnockback = 0f;
	float knockBackVelocity = 20f;

	bool facingRight = true;

	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		gameObject.GetComponent<Renderer> ().material.color = new Color(0, 100, 0);
		//anim = GetComponent<Animator> ();
		rb2d.freezeRotation = true;
		infoText.text = "Jetpack Fuel: " + jetpackFuel.ToString () + "\n" + "Ammunition: " + currentAmmo.ToString () + "\n" + "Health: " + playerHealth.ToString();
		playerHealth = 10;
	}

	//operations to assist with player movement
	void Update () {
		//Line cast and see if we are touching the ground/wall layer
		grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		touchingRightWall = Physics2D.Linecast (transform.position, rightWallCheck.position, 1 << LayerMask.NameToLayer ("Wall"));
		touchingLeftWall = Physics2D.Linecast (transform.position, leftWallCheck.position, 1 << LayerMask.NameToLayer ("Wall"));

		//reset the fuel of the jetpack on landing, and controls the jump animation
		if (grounded) {
			if (jetpackFuel != jetpackFuelMax)
				jetpackFuel++;
			//anim.SetBool ("Grounded", true);
		} else {
			//anim.SetBool ("Grounded", false);
		}

		//Controlls the users jump
		//from user input and other input, determines if the player is performing a regular jump, a double jump, or a wall jump
		if (Input.GetKeyDown (KeyCode.W)) {
			if (grounded) {
				jump = true;
				doubleJumpAvailable = true;
				wallJumpAvailable = true;
				jumpForce = firstJumpForce;
			} else if (!grounded && !touchingRightWall && !touchingLeftWall && doubleJumpAvailable) {
				jump = true;
				doubleJumpAvailable = false;
				jumpForce = secondJumpForce;
			} else if (!grounded && (touchingRightWall || touchingLeftWall) && wallJumpAvailable) {
				jump = true;
				wallJumpAvailable = false;
				jumpForce = wallJumpForce;
			}
		}

		//display fuel of jetpack
		infoText.text = "Jetpack Fuel: " + jetpackFuel.ToString () + "\n" + "Ammunition: " + currentAmmo.ToString () + "\n" + "Health: " + playerHealth.ToString();

		//controlls the weapon fire
		if (Time.time > timeOfNextReload) {
			//fire the weapon with user input if there is ammo, other wise reload when empty or when user wants to reload
			if (Input.GetKeyDown (KeyCode.G) && currentAmmo != 0) {
				currentAmmo--;
				if (currentAmmo == 0) {
					timeOfNextReload = Time.time + reloadTime;
				}
				fireProjectile ();
			} else if (currentAmmo == 0 || isReloading) {
				currentAmmo = maxAmmo;
				isReloading = false;
			} else if (Input.GetKeyDown (KeyCode.H) && currentAmmo != maxAmmo) {
				timeOfNextReload = Time.time + reloadTime;
				isReloading = true;
			}
		}

		//if player is invulnerable, make them flash to notify the player
		if (invulnerable) {
			if (flashCounter < 10) {
				gameObject.GetComponent<Renderer> ().material.color = new Color (100, 100, 100);
			} else if (flashCounter < 20) {
				gameObject.GetComponent<Renderer> ().material.color = new Color (0, 100, 0);
			} else {
				flashCounter = 0;
			}
			flashCounter++;
		}

		//destroy the player if they lose all of their health
		/*
		 * 
		 * MUST MODIFY IN THE FUTURE SO THAT THE GAME AUTOMATICALLY RESETS UPON DEATH 
		 * 
		 */
		if (playerHealth <= 0) {
			print ("GAME OVER!!!");
		}
	}

	//control players horizontal and vertical movement
	void FixedUpdate () {
		//get player command for horizontal movement
		float moveHorizontal = Input.GetAxis ("Horizontal");

		//anim.SetFloat ("Speed", Mathf.Abs (moveHorizontal));

		//player controlls horizontal movement, except after a wall jump or after the player is hit by a monster
		if (Time.time > timeAfterKnockback) {
			if (grounded) {
				rb2d.velocity = new Vector2 (moveHorizontal * moveSpeed, rb2d.velocity.y);
			} else {
				if (Time.time > timeAfterWallJump) {
					//move at walking speed in the air
					rb2d.velocity = new Vector2 (moveHorizontal * moveSpeed, rb2d.velocity.y);
				}
			}
		} else {
			rb2d.velocity = new Vector2 (knockBackDirection * knockBackVelocity, rb2d.velocity.y);
		}

		//player jumps and resets the boolean to false
		if (jump) {
			rb2d.velocity = new Vector2 (rb2d.velocity.x, jumpForce);
			//Applies a force to the player in the opposite direction of the side of the wall (ie if the wall is on the right, pushes the player left)
			if (!wallJumpAvailable) {
				wallJumpAvailable = true;
				timeAfterWallJump = Time.time + waitTime;
				//determine which direction the player will move after the wall jump
				float direction = 1;
				if ((facingRight && touchingRightWall) || (!facingRight && touchingLeftWall)) {
					direction = -1;
				} else if ((facingRight && touchingLeftWall) || (!facingRight && touchingRightWall)) {
					direction = 1;
				}
				rb2d.velocity = new Vector2 (direction * moveSpeed, rb2d.velocity.y);
			}
			jump = false;
		}

		//Changes the orientation of the sprite based on what direction the player is moving
		if (moveHorizontal > 0 && !facingRight) {
			Flip ();
		} else if (moveHorizontal < 0 && facingRight) {
			Flip ();
		}

		//after double jump if user holds up the player will fall slower
		if (Input.GetKey (KeyCode.W) && !doubleJumpAvailable && rb2d.velocity.y < 0 && jetpackFuel > 0) {
			rb2d.drag = jetpackDrag;
			jetpackFuel--;
		} else {
			rb2d.drag = 0;
		}
	}

	//Controls the orientation of the sprite
	void Flip() {
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	//instantiate a new bullet
	void fireProjectile () {
		if (Time.time > timeBetweenShots) {
			timeBetweenShots = Time.time + fireRate;
			if (facingRight) {
				Instantiate (bullet, shootPoint.position, Quaternion.Euler (new Vector3 (0,0,0f)));
			} else {
				Instantiate (bullet, shootPoint.position, Quaternion.Euler (new Vector3 (0,0,180f)));
			}
		}
	}

	//detect collision with monster
	void OnTriggerStay2D (Collider2D other) {
		//if not invulnerable, reduce health, set paramerters for player knock back, and set player to invulnerable for set time
		if(!invulnerable) {
			if (other.gameObject.CompareTag ("Zombie")) {
				ZombieController zombie = other.GetComponent<ZombieController> ();
				//determines the direction of the player knockback
				if (zombie.transform.position.x > transform.position.x) {
					knockBackDirection = -1;
				} else {
					knockBackDirection = 1;
				}
				timeAfterKnockback = Time.time + knockbackTime;
				playerHealth--;
				invulnerable = true;
				Invoke ("resetVulnerability", 2f);
			} else if (other.gameObject.CompareTag ("Bat")) {
				BatController bat = other.GetComponent<BatController> ();
				if (bat.transform.position.x > transform.position.x) {
					knockBackDirection = -1;
				} else {
					knockBackDirection = 1;
				}
				timeAfterKnockback = Time.time + knockbackTime;
				playerHealth--;
				invulnerable = true;
				Invoke ("resetVulnerability", 2f);
			}
		}
	}

	//player becomes vulnerable again
	void resetVulnerability () {
		invulnerable = false;
		gameObject.GetComponent<Renderer> ().material.color = new Color(0, 100, 0);
		flashCounter = 0;
	}
}
