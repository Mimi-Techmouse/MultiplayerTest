using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

//This script is observable, so that we always sync health and such
public class mt_basicstats : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public int KillCount = 0;

    public bool PlayerOwned = false;

    protected int parentViewID = 0;

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

    protected virtual void Awake() {
        if (gameObject.GetComponent<PhotonView>() != null) {
            gameObject.name = "Player: "+gameObject.GetComponent<PhotonView>().ViewID;
            parentViewID = gameObject.GetComponent<PhotonView>().ViewID;
        }
    }

    protected virtual void Update() {
		if (!PlayerOwned) {
			PlayerPlane.Health.Set(currentHealth); //run it through standard computations because otherwise we won't get death
		}
    }

    /// <summary>
    /// Return how much health is currently available
    /// </summary>
    protected virtual int OnValue_Health {
    	get {
    		return currentHealth;
    	} set {
            int originalHealth = currentHealth;
    		currentHealth = value;

            //Debug.Log(gameObject.name+" health updated to: "+currentHealth);

    		if (currentHealth <= 0) {
    			currentHealth = 0;
    			PlayerPlane.Dead.Start();
    		}

            if (originalHealth != currentHealth)
                PlayerPlane.HealthChanged.Send();
    	}
    }

    /// <summary>
    /// Get the Local Player
    /// </summary>
    protected virtual bool OnValue_isLocalPlayer {
    	get {
    		return PlayerOwned;
    	}
    }

    /// <summary>
    /// Apply Damage to current health
    /// </summary>
    protected virtual void OnMessage_DamageMe(int damageAmount) {

    	Debug.Log(gameObject.name+" message recieved: "+damageAmount);

    	int myHealth = PlayerPlane.Health.Get();
    	PlayerPlane.Health.Set(myHealth-damageAmount);
    }

    //For hiding all renderers on death!
    public void HideMe() {

        if (transform.GetComponent<Renderer>() != null) {
            transform.GetComponent<Renderer>().enabled = false;
        }

        foreach (Renderer ren in transform.GetComponentsInChildren<Renderer>()) {
            ren.enabled = false;
        }

    }


    /// <summary>
    /// For when you die
    /// </summary>
    protected virtual void OnStart_Dead() {

        PlayerPlane.gameObject.SendMessage ("HideMe", true, SendMessageOptions.DontRequireReceiver);

    	GameObject myExplosion = PhotonNetwork.Instantiate("VFX/Explosion", transform.position+transform.forward, Quaternion.identity);
    	
        vp_Timer.In(2.0f, () => { 
            vp_Utility.Destroy(myExplosion); 
            if (!PlayerPlane.isLocalPlayer.Get()) {
                Debug.Log("should destroy: "+this.name);
                //vp_Utility.Destroy(transform.gameObject); 
            }
        });


        if (PlayerPlane.isLocalPlayer.Get()) {
            Debug.Log("should disconnect: "+this.name);

            PhotonNetwork.LeaveRoom();

            vp_Timer.In(Time.deltaTime, () => { PhotonNetwork.LoadLevel("Menu_Lobby"); });
        }
    }


    /// <summary>
    /// Handles the kill counter
    /// </summary>
    public virtual void KilledSomething() {
    	Debug.Log(transform.name+" has killed something!");
    	KillCount++;
    }

    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    public override void OnEnable()
    {
    	base.OnEnable();

        if (PlayerPlane != null)
            PlayerPlane.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    public override void OnDisable()
    {
    	base.OnDisable();

        if (PlayerPlane != null)
            PlayerPlane.Unregister(this);

    }

    /// <summary>
    /// Networking section!
    /// </summary>
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    	if (stream.IsWriting) {
		    // We own this player: send the others our data
            stream.SendNext(parentViewID);
		    stream.SendNext(currentHealth);
		    stream.SendNext(KillCount);
		} else {
		    // Network player, receive data
            int sentViewID = (int)stream.ReceiveNext();
            if (sentViewID != this.parentViewID)
                return;

		    this.currentHealth = (int)stream.ReceiveNext();
		    this.KillCount = (int)stream.ReceiveNext();
		}
    }
    #endregion
}
