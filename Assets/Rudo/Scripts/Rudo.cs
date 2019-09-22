using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudo : MonoBehaviour
{
	public GameManager gm;
	
	public float speed;
	public GameObject gamePlane;
	public Camera camera;
	public AudioSource audioPlayer;
	public int maxHp;
	public float iSeconds; // invincibility seconds when dashing
	public float yoyoRadius;
	public float yoyoSpeed;
	public float yoyoDestroyDistance;
	public float boomerangSpeed;
	public float boomerangDestroyDistance;
	public GameObject yoyoPrefab;
	public GameObject boomerangPrefab;
	[HideInInspector] public int souls;

	
	private int hp; // current hp
	private bool yoyo; // yoyo out or boomerang out
	private GameObject yoyoGo;
	private GameObject boomerangGo;
	private Renderer _renderer;
	private Vector3 lastMoveInput;
	private Vector3 moveVelocity;
	private Vector3 yoyoMoveVelocity;
	private float dashCooldown;
	
	private bool isDashing;
	private bool isYoyoOut;
	private bool isBoomerangOut;
	private bool isInvincible;
	private bool isDead;
	private bool shouldDestroyYoyo;
	private bool shouldDestroyBoomerang;

	private Rigidbody rb;
	private Rigidbody boomerangRb;
	private Animator animator;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		_renderer = GetComponent<Renderer>();
		animator = GetComponent<Animator>();
		hp = maxHp;
		yoyo = false;
		isDashing = false;
		isYoyoOut = false;
		isBoomerangOut = false;
		isInvincible = false;
		isDead = false;
		souls = 0;
	}

	void Update()
	{
		float dx = Input.GetAxisRaw("Horizontal");
		float dz = Input.GetAxisRaw("Vertical");
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveInput = moveInput.normalized;
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (dashCooldown <= 0 && moveVelocity.magnitude >= Mathf.Epsilon)
			{
				isDashing = true;
				dashCooldown = 1;
				StartCoroutine(nameof(DashEffect));
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			// Should not destroy the yoyo while left click is being held
			shouldDestroyYoyo = false;
			
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayInfo = new RaycastHit();
			if (!gamePlane.GetComponent<Collider>().Raycast(ray, out rayInfo, 1000.0f))
			{
				Debug.Log("Player left clicked, but couldn't locate the point on the game plane");
				return;
			}

			Vector3 hitPoint = rayInfo.point;
			Vector3 relDiff = hitPoint - transform.position;

			if (yoyo)
			{
				// Yoyo logic
				if (!isYoyoOut)
				{
					yoyoGo = Instantiate(yoyoPrefab, transform.position, Quaternion.identity);
				}
				else
				{
					// Move in direction of mouse
					if(relDiff.magnitude > yoyoRadius)
						relDiff = yoyoRadius * relDiff.normalized;
					else if (relDiff.magnitude <= yoyoDestroyDistance)
						relDiff = 2 * yoyoDestroyDistance * relDiff.normalized; //can't be too close to destroy radius
					
					Vector3 targetPos = transform.position + relDiff;
					Vector3 yoyoPos = yoyoGo.transform.position;
					float adjustedSpeed = yoyoSpeed * relDiff.magnitude;
					yoyoMoveVelocity = adjustedSpeed * (targetPos - yoyoPos);
				}
			}
			else
			{
				// boomerang logic
				if (!isBoomerangOut)
				{
					boomerangGo = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
					boomerangRb = boomerangGo.GetComponent<Rigidbody>();
					boomerangRb.velocity = relDiff.normalized * boomerangSpeed;
				}
				else
				{
					boomerangRb.velocity += boomerangSpeed * (transform.position - boomerangGo.transform.position);
				}
			}
		}
		else
		{
			// Mouse is not being pressed down, so retract yoyo
			if (isYoyoOut)
			{
				// Retract Yoyo
				Vector3 yoyoDisplacement = yoyoGo.transform.position - transform.position;
				float yoyoMoveSpeed = yoyoSpeed * yoyoDisplacement.magnitude;
				yoyoMoveVelocity = -yoyoMoveSpeed * yoyoDisplacement;
			}
			shouldDestroyYoyo = true;
		}

		// Switch weapons
		if (Input.GetKeyDown(KeyCode.F))
			yoyo = !yoyo;
		
		moveVelocity = moveInput * speed;
	}

	private void LateUpdate()
	{
		if (isYoyoOut && shouldDestroyYoyo && (yoyoGo.transform.position - transform.position).magnitude <= yoyoDestroyDistance)
		{
			Destroy(yoyoGo); //todo don't destroy?
			isYoyoOut = false;
		}

		if (isBoomerangOut && shouldDestroyBoomerang && (boomerangGo.transform.position - transform.position).magnitude <= boomerangDestroyDistance)
		{
			Destroy(boomerangGo);
			isBoomerangOut = false;
		}
	}

	IEnumerator TurnOffInvisibility()
	{
		yield return new WaitForSeconds(iSeconds);
		setInvincible(false);
	}

	IEnumerator DashEffect()
	{
		Material mat = _renderer.material;
		mat.color = Color.blue;
		yield return new WaitForSeconds(0.1f);
		mat.color = Color.white;
	}

	IEnumerator HitEffect()
	{
		Material mat = _renderer.material;
		mat.color = Color.red;
		yield return new WaitForSeconds(0.1f);
		mat.color = Color.white;
	}

	void FixedUpdate()
	{
		if (isDashing)
		{
			moveVelocity *= 10;
			setInvincible(true);
			isDashing = false;
			StartCoroutine(nameof(TurnOffInvisibility));
		}

		dashCooldown -= Time.deltaTime;
		rb.velocity = moveVelocity;
	}

	private void OnCollisionEnter(Collision other)
	{
		GameObject go = other.gameObject;
		if (go.CompareTag("Bullet"))
		{
			if (isInvincible)
				return;
			
			hp -= 1;
			gm.DestroyBullet(other.gameObject);
			StartCoroutine(nameof(HitEffect));
			Debug.Log("oof");
			checkDeath();
			setInvincible(true);
			StartCoroutine(nameof(TurnOffInvisibility));
		}
	}

	void setInvincible(bool value)
	{
		isInvincible = value;
		if(value)
			gm.OnBecomeInvincible();
	}

	private void checkDeath()
	{
		if (hp <= 0)
		{
			//todo death
		}
	}
}