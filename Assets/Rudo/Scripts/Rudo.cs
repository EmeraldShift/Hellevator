using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudo : MonoBehaviour {

	public float speed;

	private Rigidbody rb;
	private Vector3 moveVelocity;

	void Start() {
		rb = GetComponent<Rigidbody>();
    }
	
    void Update() {
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		moveVelocity = moveInput.normalized * speed;
	}

	void FixedUpdate() {
		rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
	}
}
