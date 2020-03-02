using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_fireweapon : MonoBehaviour {

	public float firingDelay = 1.0f; //defaults to firing 1/second
	public string bulletName = "";
	protected float lastFired = 0.0f;

    protected vp_FPPlayerEventHandler m_PlayerPlane = null;
    public vp_FPPlayerEventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.root.GetComponent<vp_FPPlayerEventHandler>();
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

    // Update is called once per frame
    void Update() {

    	if (PlayerPlane.Attack.Active) {
    		Debug.Log("attacking!");

    		if (Time.time-lastFired > firingDelay) {
    			SpawnProjectile();
    		}
    	}
        
    }

    public void SpawnProjectile() {

    	Debug.Log("spawning projectile");

    	lastFired = Time.time;

    	PhotonNetwork.Instantiate("Prefabs/"+bulletName, transform.position, Quaternion.identity);

    }

    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    protected virtual void OnEnable()
    {

        if (PlayerPlane != null)
            PlayerPlane.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    protected virtual void OnDisable()
    {

        if (PlayerPlane != null)
            PlayerPlane.Unregister(this);

    }
}
