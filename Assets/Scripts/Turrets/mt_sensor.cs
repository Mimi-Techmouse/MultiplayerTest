using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_sensor : MonoBehaviour {

	public List<mt_EventHandler> enemies = null;
	private float lastTested = 0.0f;
	public float senseInterval = 1.0f;
	public float senseRadius = 30.0f;

    // Don't check for enemeis too often!
	public void Update() {

		if (Time.time - lastTested < senseInterval)
			return;

		GetSensed ();

	}

	public void GetSensed() {

		LayerMask mask = 1 << 30; //only player layer
		lastTested = Time.time;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, senseRadius, mask);
		enemies = new List<mt_EventHandler> ();
		int n = 0;
		while (n < hitColliders.Length) {

			Transform item = hitColliders[n].transform;
			mt_EventHandler itemHandler = item.GetComponent<mt_EventHandler>();
			if (itemHandler != null) {
				Debug.Log("found: "+item.name);
			}

		}

	}
}
