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

	
	private int _hp; // current hp
	private bool _yoyo; // yoyo out or boomerang out
	private GameObject _yoyoGo;
	private GameObject _boomerangGo;
	private Renderer _renderer;
	private Vector3 _lastMoveInput;
	private Vector3 _moveVelocity;
	private Vector3 _yoyoMoveVelocity;
	private float _dashCooldown;
	
	private bool _isDashing;
	private bool _isYoyoOut;
	private bool _isBoomerangOut;
	private bool _isInvincible;
	private bool _isDead;
	private bool _shouldDestroyYoyo;
	private bool _shouldDestroyBoomerang;

	private Rigidbody _rb;
	private Rigidbody _boomerangRb;
	private Animator _animator;

	void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_renderer = GetComponent<Renderer>();
		_animator = GetComponent<Animator>();
		_hp = maxHp;
		_yoyo = false;
		_isDashing = false;
		_isYoyoOut = false;
		_isBoomerangOut = false;
		_isInvincible = false;
		_isDead = false;
		souls = 0;
	}

	void Update()
	{
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveInput = moveInput.normalized;
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (_dashCooldown <= 0 && _moveVelocity.magnitude >= Mathf.Epsilon)
			{
				_isDashing = true;
				_dashCooldown = 1;
				StartCoroutine(nameof(DashEffect));
			}
		}

		HandleWeapon();
		
		_moveVelocity = moveInput * speed;
	}

	private void HandleWeapon()
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit rayInfo = new RaycastHit();
		if (!gamePlane.GetComponent<Collider>().Raycast(ray, out rayInfo, 1000.0f))
		{
			Debug.Log("Player left clicked, but couldn't locate the point on the game plane");
			return;
		}

		Vector3 hitPoint = rayInfo.point;
		
		// Proccess weapon tick
		if (_yoyo)
			HandleYoyo(hitPoint);
		else
			HandleBoomerang(hitPoint);
		
		// Switch weapons
		if (Input.GetKeyDown(KeyCode.F))
			_yoyo = !_yoyo;
	}

	private void HandleYoyo(Vector3 hitPoint)
	{
		if (Input.GetMouseButton(0))
		{
			Vector3 relDiff = hitPoint - transform.position;
			
			// Yoyo logic
			if (!_isYoyoOut)
			{
				_yoyoGo = Instantiate(yoyoPrefab, relDiff.normalized * yoyoDestroyDistance + transform.position, Quaternion.identity);
				_yoyoGo.GetComponent<Yoyo>().gm = gm;
				
				// Tell yoyo where to return
				_yoyoGo.SendMessage("SetSender", transform);
				_isYoyoOut = true;
			}

			// Move in direction of mouse
			_yoyoGo.SendMessage("Retract", false);
			Vector3 mousePoint = hitPoint;
			if (relDiff.sqrMagnitude > yoyoRadius * yoyoRadius)
				mousePoint = transform.position + relDiff.normalized * yoyoRadius;
			Vector3 mouseDiff = mousePoint - _yoyoGo.transform.position;
			_yoyoGo.GetComponent<Rigidbody>().velocity = mouseDiff * yoyoSpeed;
		}
		else
		{
			// Mouse is not being pressed down, so retract yoyo
			if (_isYoyoOut)
			{
				// Retract Yoyo
				_yoyoGo.SendMessage("Retract", true);
				Vector3 yoyoDisplacement = _yoyoGo.transform.position - transform.position;
				float yoyoMoveSpeed = yoyoSpeed * yoyoDisplacement.magnitude;
				_yoyoMoveVelocity = -yoyoMoveSpeed * yoyoDisplacement;
			}
			_shouldDestroyYoyo = true;
		}
	}

	private void HandleBoomerang(Vector3 hitPoint)
	{
		if (!Input.GetMouseButton(0))
			return;
		
		Vector3 relDiff = hitPoint - transform.position;
		
		// Boomerang logic
		if (_isBoomerangOut)
			return;
		_isBoomerangOut = true; // set to false when destroyed

		var position = transform.position;
		_boomerangGo = Instantiate(boomerangPrefab, position, Quaternion.identity);
		_boomerangRb = _boomerangGo.GetComponent<Rigidbody>();
		_boomerangRb.velocity = relDiff.normalized * boomerangSpeed;
		Physics.IgnoreCollision(_boomerangGo.GetComponent<Collider>(), GetComponent<Collider>());

		// Tell boomerang where to go, and where to return
		_boomerangGo.SendMessage("SetSender", transform);
	}

	public void YoyoDone()
	{
		_isYoyoOut = false;
	}

	public void BoomerangDone()
	{
		_isBoomerangOut = false;
	}
	
	IEnumerator TurnOffInvisibility()
	{
		yield return new WaitForSeconds(iSeconds);
		SetInvincible(false);
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
		if (_isDashing)
		{
			_moveVelocity *= 10;
			SetInvincible(true);
			_isDashing = false;
			StartCoroutine(nameof(TurnOffInvisibility));
		}

		_dashCooldown -= Time.deltaTime;
		_rb.velocity = _moveVelocity;
	}

	private void OnCollisionEnter(Collision other)
	{
		GameObject go = other.gameObject;
		if (go.CompareTag("Bullet"))
		{
			if (_isInvincible)
				return;
			
			_hp -= 1;
			gm.DestroyBullet(other.gameObject);
			StartCoroutine(nameof(HitEffect));
			Debug.Log("oof");
			CheckDeath();
			SetInvincible(true);
			StartCoroutine(nameof(TurnOffInvisibility));
		}
	}

	void SetInvincible(bool value)
	{
		_isInvincible = value;
		if(value)
			gm.OnBecomeInvincible();
	}

	private void CheckDeath()
	{
		if (_hp <= 0)
		{
			//todo death
		}
	}
}