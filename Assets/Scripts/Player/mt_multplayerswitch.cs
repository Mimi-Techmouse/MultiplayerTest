using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

//A script to swap between the cockpit and ship appearance, depending on 
//whether this is the local player or not
public class mt_multplayerswitch : MonoBehaviourPun
{
    private GameObject m_Cockpit = null;
    public GameObject Cockpit {
    	get {
    		if (m_Cockpit == null) {
    			Camera cam = GetComponentInChildren<Camera>();
    			if (cam != null)
    				m_Cockpit = cam.gameObject;
    		}
    		return m_Cockpit;
    	}
    }

    private GameObject m_ShipBody = null;
    public GameObject ShipBody {
    	get {
    		if (m_ShipBody == null) {
    			mt_shipbody body = GetComponentInChildren<mt_shipbody>();
    			if (body != null)
    				m_ShipBody = body.gameObject;
    		}
    		return m_ShipBody;
    	}
    }

    public void Awake() {

		if (photonView.IsMine) {
			Cockpit.SetActive(true);
			ShipBody.SetActive(false);
            GetComponent<mt_basicstats>().PlayerOwned = true;
		} else {
			Cockpit.SetActive(false);
			ShipBody.SetActive(true);
		}
    }
}
