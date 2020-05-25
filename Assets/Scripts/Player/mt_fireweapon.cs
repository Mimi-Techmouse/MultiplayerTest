using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_fireweapon : MonoBehaviour {

	public string bulletName = "";
	public Transform firingPoint = null;
	protected float lastFired = 0.0f;
	protected mt_weaponhandler Handler = null;

	protected mt_bulletfly lastBullet;

    public mt_EventHandler m_PlayerPlane = null;
    public mt_EventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null) {
                if (Handler != null)
                    m_PlayerPlane = Handler.GetComponent<mt_EventHandler>();
                else
                    m_PlayerPlane = transform.root.GetComponent<mt_EventHandler>();
            }
            return m_PlayerPlane;
        }
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
    	   return (Handler.Crosshair.transform.position+(Handler.Crosshair.transform.forward*100));
        else
           return (PlayerPlane.transform.position+PlayerPlane.transform.forward*100);

    }

    public void SpawnProjectile() {

    	//Debug.Log("spawning projectile");

    	GameObject oBullet = PhotonNetwork.Instantiate("Prefabs/"+bulletName, firingPoint.position, Quaternion.identity);
    	lastBullet = oBullet.GetComponent<mt_bulletfly>();
        lastBullet.SetFirer(PlayerPlane);
        lastBullet.SetTarget(GetTargetLocation());

    }
}
