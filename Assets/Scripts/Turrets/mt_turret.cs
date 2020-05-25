﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_turret : MonoBehaviour {

	public GameObject[] InstantiatedWeapons;

    public float MaxRotation = 180.0f; //the amount you are allowed to stray from base angle
    private Quaternion baseRotation;
    private Quaternion targetRotation;

	protected mt_EventHandler m_PlayerPlane = null;
    public mt_EventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.GetComponent<mt_EventHandler>();
            return m_PlayerPlane;
        }
    }

	void Start() {
		baseRotation = transform.rotation;
	}

	void Update() {

		mt_EventHandler target = PlayerPlane.CurrentTarget.Get();
		Debug.Log("target: "+target.name);
		if (target == null)
			return;

		Vector3 look = target.transform.position - transform.position;
         
        Quaternion q = Quaternion.LookRotation(look);
        q.z = 0;
        q.x = 0;

        //We are within bounds! Get 'im!
        Debug.Log("look: "+look);
        Debug.Log("within angle? "+Quaternion.Angle (q, baseRotation));
        if (Quaternion.Angle (q, baseRotation) <= MaxRotation) {
            targetRotation = q;
        	StartFiring();
        } else {
        	StopFiring();
        }
         
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);

	}

	void StartFiring() {

		foreach (GameObject weapon in InstantiatedWeapons) {

    		mt_fireweapon w = weapon.GetComponent<mt_fireweapon>();
    		w.AnimationController.SetTrigger("Fire");

    	}

	}

	void StopFiring() {

    	foreach (GameObject weapon in InstantiatedWeapons) {

    		mt_fireweapon w = weapon.GetComponent<mt_fireweapon>();
    		w.AnimationController.SetTrigger("Stop_Fire");

    	}

	}

}
