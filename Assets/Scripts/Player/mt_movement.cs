using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class mt_movement : MonoBehaviourPun
{
	public float movementSpeed = 4.0f;

	private CharacterController controller = null;
	private Camera myCamera = null;

	private void Start() {
		controller = GetComponent<CharacterController>();
		myCamera = Camera.main;
	}

	void Update() {

		if (photonView.IsMine) {
			TakeInput();
			myCamera.transform.position = new Vector3(transform.position.x, 10, transform.position.z);
		} 

	}

	private void TakeInput() {

		Vector3 movement = new Vector3 {
			x = Input.GetAxisRaw("Horizontal"),
			y = 0f,
			z = Input.GetAxisRaw("Vertical"),
		}.normalized;

		controller.SimpleMove(movement * movementSpeed);

	}
}
