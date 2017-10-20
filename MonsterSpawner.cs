using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {

	public float minSpawnX;
	public float maxSpawnX;

	float spawnTime;
	bool nextSpawnChosen = true;

	public GameObject zombie;
	public GameObject bat;

	void Start () {
		spawnTime = 3f;
	}

	void Update () {

		//Allow another monster to spawn after the allowed time
		if (Time.time >= spawnTime) {
			nextSpawnChosen = false;
		}

		//spawn the monster and select the next random spawn time
		if (!nextSpawnChosen) {
			float randTime = Random.Range (3, 8);
			spawnTime += randTime;
			nextSpawnChosen = true;
			if (randTime <= 6) {
				spawnZombie ();
			} else {
				spawnBat ();
			}
		}
	}

	//spawns zombie at random X location
	void spawnZombie () {
		Vector2 temp = new Vector2 (Random.Range(minSpawnX, maxSpawnX), 10f);
		Instantiate (zombie, temp, Quaternion.Euler (new Vector3 (0,0,0f)));
	}

	//spawns bat at random X location
	void spawnBat () {
		Vector2 temp = new Vector2 (Random.Range(minSpawnX, maxSpawnX), 4f);
		Instantiate (bat, temp, Quaternion.Euler (new Vector3 (0,0,0f)));
	}
}
