using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_weaponhandler : MonoBehaviour {

	public GameObject Crosshair;
	public string GunName;
	public Transform RightGunHook;
	public Transform LeftGunHook;

	public GameObject[] InstantiatedWeapons;

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


    void Start() {

    	InstantiatedWeapons = new GameObject[2];

    	InstantiatedWeapons[0] = PhotonNetwork.Instantiate("Prefabs/"+GunName, RightGunHook.position, RightGunHook.rotation);
    	InstantiatedWeapons[0].transform.parent = RightGunHook;
    	InstantiatedWeapons[1] = PhotonNetwork.Instantiate("Prefabs/"+GunName, LeftGunHook.position, LeftGunHook.rotation);
    	InstantiatedWeapons[1].transform.parent = LeftGunHook;
        
    }


    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnStart_Attack() {

    	foreach (GameObject weapon in InstantiatedWeapons) {

    		mt_fireweapon w = weapon.GetComponent<mt_fireweapon>();
    		w.SetHandler(this);
    		w.AnimationController.SetTrigger("Fire");

    	}

    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnStop_Attack() {

    	foreach (GameObject weapon in InstantiatedWeapons) {

    		mt_fireweapon w = weapon.GetComponent<mt_fireweapon>();
    		w.AnimationController.SetTrigger("Stop_Fire");

    	}

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
