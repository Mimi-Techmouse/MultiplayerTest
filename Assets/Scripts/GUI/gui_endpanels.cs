using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

public class gui_endpanels : MonoBehaviour {

	public GameObject StartPanel = null;
	public GameObject VictoryPanel = null;
	public GameObject LossPanel = null;
    public Text CountDown = null;

    protected mt_PlayerEventHandler m_PlayerPlane = null;
    public mt_PlayerEventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.parent.parent.GetComponent<mt_PlayerEventHandler>();
            return m_PlayerPlane;
        }
    }

	void Start() {
	    VictoryPanel.SetActive(false);
	    LossPanel.SetActive(false);
	    StartPanel.SetActive(false);
	    PlayerPlane.ShowStartPanel.Send();
        CountDown.gameObject.SetActive(false);
	}
    
    void Update() {

	    if (vp_Input.GetButtonDown("Menu")) {
	    	HideStartPanel();

	    	if (VictoryPanel.activeSelf || LossPanel.activeSelf) 
	    		LogOff();
	    }

	}

	public void LogOff() {

        Debug.Log("should disconnect: "+this.name);

		PhotonNetwork.LeaveRoom();

        vp_Timer.In(Time.deltaTime, () => { PhotonNetwork.LoadLevel("Menu_Lobby"); });

	}

	public void HideStartPanel() {

		Debug.Log("hiding the start panel!");

		PlayerPlane.HideStartPanel.Send();

    	StartPanel.SetActive(false);

	}

    protected virtual void OnMessage_Countdown(int n) {
        CountDown.gameObject.SetActive(true);
        CountDown.text = ""+n;

        if (n <= 1) {
            vp_Timer.In(0.9f, () => { CountDown.gameObject.SetActive(false); });
        }
    }

    /// <summary>
    /// Show the victory panel!
    /// </summary>
    protected virtual void OnMessage_ShowVictoryPanel() {

    	if (!PlayerPlane.isLocalPlayer.Get())
    		return;

    	VictoryPanel.SetActive(true);
    }

    /// <summary>
    /// Show the looser panel :(
    /// </summary>
    protected virtual void OnMessage_ShowLossPanel() {

    	if (!PlayerPlane.isLocalPlayer.Get())
    		return;

    	LossPanel.SetActive(true);
    }

    /// <summary>
    /// Show the start panel
    /// </summary>
    protected virtual void OnMessage_ShowStartPanel() {

    	if (!PlayerPlane.isLocalPlayer.Get())
    		return;

    	StartPanel.SetActive(true);
    }

    /// <summary>
    /// Show the start panel
    /// </summary>
    protected virtual void OnMessage_HideStartPanel() {

    	StartPanel.SetActive(false);
    }


    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    public virtual void OnEnable()
    { 

        if (PlayerPlane != null)
            PlayerPlane.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    public virtual void OnDisable()
    {

        if (PlayerPlane != null)
            PlayerPlane.Unregister(this);

    }
}
