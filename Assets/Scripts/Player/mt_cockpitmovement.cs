using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_cockpitmovement : MonoBehaviour
{
	public float speed = 100.0f;

    public Camera m_playerCamera = null;
    public Camera PlayerCamera {
    	get {
    		if (m_playerCamera == null)
    			m_playerCamera = Camera.main;
    		return m_playerCamera;
    	}
    }

    public void Update() {
    	transform.rotation = Quaternion.Lerp(transform.rotation, PlayerCamera.transform.rotation, Time.deltaTime*speed);
    	transform.position = PlayerCamera.transform.position;
    }

    public void LateUpdate() {
    	transform.rotation = Quaternion.Lerp(transform.rotation, PlayerCamera.transform.rotation, Time.deltaTime*speed);
    	transform.position = PlayerCamera.transform.position;
    }
}
