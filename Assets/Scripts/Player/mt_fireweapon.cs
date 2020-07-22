﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_fireweapon : MonoBehaviour {

	public string bulletName = "";
	public Transform firingPoint = null;
    public GameObject fireParticle = null;
	protected float lastFired = 0.0f;
	protected mt_weaponhandler Handler = null;

	protected mt_bulletfly lastBullet;

    protected int checkedForPlayer = 0;
    public bool isTurret = false;

    public mt_EventHandler m_PlayerPlane = null;
    public mt_EventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null) {
                if (Handler != null)
                    m_PlayerPlane = Handler.GetComponent<mt_EventHandler>();
                else
                    m_PlayerPlane = transform.parent.parent.GetComponent<mt_EventHandler>();
            }
            return m_PlayerPlane;
        }
    }

    //Removing extra instantiations
    public void Update() {

        if (isTurret)
            return;

        if (PlayerPlane == null)
            checkedForPlayer++;
        else
            return;

        if (checkedForPlayer > 5)
            vp_Utility.Destroy(gameObject);
    }

    protected AudioSource m_audio = null;
    public AudioSource Audio
    {
        get
        {
            if (m_audio == null)
                m_audio = GetComponent<AudioSource>();
            return m_audio;
        }
    } 

    protected Animator m_anim = null;
    public Animator AnimationController
    {
        get
        {
            if (m_anim == null)
                m_anim = GetComponent<Animator>();
            return m_anim;
        }
    } 

    public void SetHandler(mt_weaponhandler h) {
    	Handler = h;
    }

    public Vector3 GetTargetLocation() {

        if (Handler != null)
    	   return (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 500)));
        else
           return PlayerPlane.transform.position+(PlayerPlane.transform.forward*500);

    }

    public void SpawnProjectile() {

        if (Handler != null && !PlayerPlane.isLocalPlayer.Get())
            return;
        if (PlayerPlane == null || PlayerPlane.Dead.Active)
            return;

        fireParticle.SetActive(true);
        //fireParticle.GetComponent<ParticleSystem>().Play();
        //fireParticle.GetComponent<ParticleSystem>().Simulate(GetComponent<ParticleSystem>().duration);

        vp_Timer.In(0.7f, () => { 
            //fireParticle.GetComponent<ParticleSystem>().Clear();
            fireParticle.SetActive(false); 
        });
        //Debug.Log("spawning projectile");

    	GameObject oBullet = PhotonNetwork.Instantiate("Prefabs/"+bulletName, firingPoint.position, Quaternion.identity);
    	lastBullet = oBullet.GetComponent<mt_bulletfly>();
        lastBullet.SetFirer(PlayerPlane);

        Vector3 targetLoc = GetTargetLocation();

        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = targetLoc;
        cube.transform.localScale = new Vector3(2, 2, 2);*/

        //Debug.Log("setting target: "+targetLoc);
        lastBullet.SetTarget(targetLoc);

    }
}
