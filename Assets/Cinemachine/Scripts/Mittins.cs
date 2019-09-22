using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Mittins : MonoBehaviour
{
	public GameManager gm;
	
	public GameObject laserPivot;
	public GameObject laser;
	public GameObject circleLaser;
	public Transform leftEye;
	public Transform center;
	public Transform rightEye;
	public Transform[] fireBallVolley;
	public GameObject rudo;

	public float patternSecs0;
	public float patternSecs1;
	public float patternSecs2;
	public float patternSecs3;

	public float laserRotationFlat0; //Rotation for first pattern
	private float laserRotationSpeed0;

	public float laserTime1; //Time laser stays alive second pattern
	public float bulletCount1;
	public float bulletSpeed1;
	private float curLaserTime1; //Time laser stays alive second pattern
	private bool genBullets1;

	public float timeBetweenCircle2; //Time between next circle spawn
	public float circleSpeed2;
	private float circleTime2;

	public Color laserColor;
	public float distFromEyes;
	public float distFromCenter;

	private float curLaserWarningTime;
	private float laserWarning1 = 0.5f;
	private float laserWarning2 = 1f;

	private int pattern; //Boss attack pattern
	private int lastPattern; //Last boss attack pattern, do not want to repeat
	private int phase; //Phases of the boss
	private System.Random PRNG;
	private float restTime;
	private float timeLeftInPattern;
	private float timeLeftUntilNextPhase;
	private bool initPattern;

	private List<GameObject> circleLasers;
	private List<GameObject> laserPointers;
	private List<GameObject> lasers;
	private List<GameObject> shots; 

	void Start()
    {
		PRNG = new System.Random();
		initPattern = true;
		pattern = 1;
		lastPattern = 3;
		shots = new List<GameObject>();
		lasers = new List<GameObject>();
		laserPointers = new List<GameObject>();
		circleLasers = new List<GameObject>();
		restTime = 0;
		timeLeftInPattern = 0;
		timeLeftUntilNextPhase = 60;
	}

	void removeCircles() {
		if (circleLasers.Count <= 0) return;
		foreach (GameObject toDel in circleLasers) {
			Destroy(toDel);
		}
		circleLasers.Clear();
	}

	void removeLasers() {
		if (laserPointers.Count > 0) {
			foreach (GameObject toDel in laserPointers) {
				Destroy(toDel);
			}
			laserPointers.Clear();
		}
		if (lasers.Count > 0) {
			foreach (GameObject toDel in lasers) {
				Destroy(toDel);
			}
			lasers.Clear();
		}
	}

	void endPatternRecognition(float timeLeft) {
		lastPattern = pattern;
		pattern = PRNG.Next(0, 3);
		timeLeftInPattern = timeLeft;
		initPattern = false;
	}

	void endPattern(float rest) {
		restTime = rest;
		initPattern = true;

		removeLasers();
		gm.DestroyAllBullets();
		removeCircles();
	}

	void resetLaserColor() {
		laserColor.r = 1;
		laserColor.g = 1;
		laserColor.b = 1;
		laserColor.a = 1;
	}

	void updateLaserColor(float curTime, float totalTime) {
		laserColor.g = (curTime / totalTime);
		laserColor.b = (curTime / totalTime);
		laserColor.a = 1 - (curTime / totalTime);
		foreach(GameObject oneLaser in lasers) {
			oneLaser.GetComponent<Renderer>().material.color = laserColor;
		}
	}

	void Update()
    {
		if (pattern == lastPattern) pattern = PRNG.Next(0 , 3);
		if (restTime <= 0 && initPattern) {
			while (pattern == lastPattern) pattern = PRNG.Next(0, 3);
			if (pattern == 0) {
				resetLaserColor();

				laserPointers.Add(Instantiate(laserPivot, leftEye.position, leftEye.rotation));
				laserPointers.Add(Instantiate(laserPivot, rightEye.position, rightEye.rotation));
				lasers.Add(Instantiate(laser, laserPointers[0].transform));
				lasers.Add(Instantiate(laser, laserPointers[1].transform));
				lasers[0].transform.position += lasers[0].transform.forward * distFromEyes;
				lasers[1].transform.position += lasers[1].transform.forward * distFromEyes;

				laserRotationSpeed0 = laserRotationFlat0;
				curLaserWarningTime = laserWarning1;

				endPatternRecognition(patternSecs0);
			} else if (pattern == 1) {
				resetLaserColor();

				genBullets1 = false;
				curLaserWarningTime = laserWarning2;
				curLaserTime1 = 0;

				endPatternRecognition(patternSecs1);
			} else if (pattern == 2) {
				resetLaserColor();

				laserPointers.Add(Instantiate(laserPivot, leftEye.position, leftEye.rotation));
				laserPointers.Add(Instantiate(laserPivot, rightEye.position, rightEye.rotation));
				lasers.Add(Instantiate(laser, laserPointers[0].transform));
				lasers.Add(Instantiate(laser, laserPointers[1].transform));
				lasers[0].transform.position += lasers[0].transform.forward * distFromEyes;
				lasers[1].transform.position += lasers[1].transform.forward * distFromEyes;

				laserPointers[0].transform.Rotate(Vector3.up * 27.5f);
				laserPointers[1].transform.Rotate(Vector3.down * 27.5f);

				curLaserWarningTime = laserWarning1;
				circleTime2 = 0.5f;

				endPatternRecognition(patternSecs2);
			} else if (pattern == 3) {


				endPatternRecognition(patternSecs3);
			}
		}

		if (!initPattern) {
			if (timeLeftInPattern > 0) {
				if (lastPattern == 0) {
					if(curLaserWarningTime <= 0) {
						laserPointers[0].transform.Rotate(Vector3.up * (laserRotationSpeed0 * Time.deltaTime));
						laserPointers[1].transform.Rotate(Vector3.down * (laserRotationSpeed0 * Time.deltaTime));
						//laserRotationSpeed0 += (float)(Time.deltaTime * (laserRotationSpeed0 - laserRotationFlat0 + 15)); //Accelerate
					}
					curLaserWarningTime -= Time.deltaTime;
					updateLaserColor(curLaserWarningTime, laserWarning1);
				} else if (lastPattern == 1) {
					if(curLaserTime1 > 0) {
						if (curLaserWarningTime <= 0 && !genBullets1) {
							for(int i = 0; i < bulletCount1; i++)
							{
								Vector3 nextSpawnVector = laserPointers[0].transform.position + laserPointers[0].transform.forward * 0.075f * i;
								float nextSpawnAngle = Vector3.Angle(laserPointers[0].transform.position, laserPointers[0].transform.forward);
								gm.SpawnBullet(nextSpawnVector, nextSpawnAngle + 90f, 0, bulletSpeed1);
								gm.SpawnBullet(nextSpawnVector, nextSpawnAngle - 90f, 0, bulletSpeed1);
							}
							genBullets1 = true;
						}
						curLaserWarningTime -= Time.deltaTime;
						updateLaserColor(curLaserWarningTime, laserWarning2);
					} else {
						removeLasers();

						laserPointers.Add(Instantiate(laserPivot, center.position, center.rotation));
						lasers.Add(Instantiate(laser, laserPointers[0].transform));
						lasers[0].transform.position += lasers[0].transform.forward * distFromCenter;
						lasers[0].transform.localScale = new Vector3(0.01f, 0.01f, 1.125f);

						laserPointers[0].transform.LookAt(rudo.transform);

						curLaserTime1 = laserTime1;
						curLaserWarningTime = laserWarning2;
						genBullets1 = false;
					}
					curLaserTime1 -= Time.deltaTime;
				} else if (lastPattern == 2) {
					if(circleTime2 < 0) {
						removeCircles();

						circleLasers.Add(Instantiate(circleLaser, center.position, center.rotation));
						circleLasers[0].transform.localScale = Vector3.zero;
						
						circleTime2 = timeBetweenCircle2;
					} else {
						if (circleLasers.Count > 0) {
							circleLasers[0].transform.localScale += (Vector3.one * circleSpeed2);
						}
					}
					circleTime2 -= Time.deltaTime;
					curLaserWarningTime -= Time.deltaTime;
					updateLaserColor(curLaserWarningTime, laserWarning1);
				} else if (lastPattern == 3) {

				}
			} else endPattern(0.25f);
		}
		restTime -= Time.deltaTime;
		timeLeftInPattern -= Time.deltaTime;
	}
}
