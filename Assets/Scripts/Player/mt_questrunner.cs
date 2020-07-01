using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_questrunner : MonoBehaviour {

    public Transform McGuffin = null;
    public GameObject McGuffinHook = null;

    protected mt_PlayerEventHandler m_PlayerPlane = null;
    public mt_PlayerEventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.GetComponent<mt_PlayerEventHandler>();
            return m_PlayerPlane;
        }
    }

    /// <summary>
    /// For recieving the McGuffin Message
    /// </summary>
    public void GrabMcGuffin(GameObject mc) {
    	PlayerPlane.GrabMcGuffin.Send(mc);
    }

    /// <summary>
    /// For recieving the Loose McGuffin message
    /// </summary>
    public void LooseMcGuffin() {
    	PlayerPlane.LooseMcGuffin.Send();
    }

    /// <summary>
    /// We delivered it!
    /// </summary>
    public void McGuffinDelivered(int viewID) {

    	if (viewID == PlayerPlane.GetPlayerView.Get()) {
    		if (PlayerPlane.HasMcGuffin.Get()) {
    			Debug.Log("you win!");
                PlayerPlane.ShowVictoryPanel.Send();
    		}
    	} else {
            //Debug.Log("you delivered to the wrong place!");
            //Debug.Log("your id: "+PlayerPlane.GetPlayerView.Get());
            //Debug.Log("their id: "+viewID);
        }

    }

    /// <summary>
    /// Get the McGuffin
    /// </summary>
    protected virtual Transform OnValue_McGuffin {
        get {
            return McGuffin;
        }
        set {
            McGuffin = value;

            McGuffin.position = McGuffinHook.transform.position;
            McGuffin.parent = McGuffinHook.transform;

            McGuffin.GetComponent<mt_goal>().isGrabbed = true;
        }
    }

    /// <summary>
    /// Do we have the McGuffin?
    /// </summary>
    protected virtual bool OnValue_HasMcGuffin {
    	get {
    		return (McGuffin != null);
    	}
    }

    /// <summary>
    /// Grab that McGuffin!
    /// </summary>
	public virtual void OnMessage_GrabMcGuffin(GameObject mc) {
		Debug.Log("grab the mcguffin: "+mc.name);

		PlayerPlane.McGuffin.Set(mc.transform);
	}

    /// <summary>
    /// Loose that McGuffin!
    /// </summary>
	public virtual void OnMessage_LooseMcGuffin() {

		if (McGuffin == null)
			return;

		Debug.Log("loose that mcguffin: "+McGuffin.name);

		McGuffin.GetComponent<mt_goal>().isGrabbed = false;
		McGuffin.parent = null;
		McGuffin = null;

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
