using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_questrunner : MonoBehaviour {

    public Transform McGuffin = null;
    public GameObject McGuffinHook = null;

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

    /// <summary>
    /// For reciving the McGuffin Message
    /// </summary>
    public void GrabMcGuffin(GameObject mc) {
    	PlayerPlane.GrabMcGuffin.Send(mc);
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
        }
    }


    /// <summary>
    /// Grab that McGuffin!
    /// </summary>
	public virtual void OnMessage_GrabMcGuffin(GameObject mc) {
		Debug.Log("grab the mcguffin: "+mc.name);
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
