using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_sensor : MonoBehaviour {

	public List<mt_EventHandler> enemies = null;
	private float lastTested = 0.0f;
	public float senseInterval = 1.0f;
	public float senseRadius = 30.0f;

	public mt_EventHandler CurrentTarget;

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

    // Don't check for enemeis too often!
	public void Update() {

		if (Time.time - lastTested < senseInterval)
			return;

		GetSensed ();

	}

	public void GetSensed() {

		float closestEnemey = Mathf.Infinity;

		LayerMask mask = 1 << 30; //only player layer
		lastTested = Time.time;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, senseRadius, mask);
		enemies = new List<mt_EventHandler> ();
		int n = 0;
		while (n < hitColliders.Length) {

			Transform item = hitColliders[n].transform;
			mt_EventHandler itemHandler = item.GetComponent<mt_EventHandler>();
			if (itemHandler != null && !itemHandler.Dead.Active) {
				//Debug.Log("found: "+item.name);
				enemies.Add(itemHandler);

				float thisDistance = Vector3.Distance(transform.position, item.position);
				if (thisDistance < closestEnemey) {
					CurrentTarget = itemHandler;
					closestEnemey = thisDistance;
				}
			}

			n++;

		}

	}

	/// <summary>
    /// Apply Damage to current health
    /// </summary>
    protected mt_EventHandler OnValue_CurrentTarget {
    	get {
    		return CurrentTarget;
    	}
    }

    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    protected virtual void OnEnable() {

        if (PlayerPlane != null)
            PlayerPlane.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    protected virtual void OnDisable() {

        if (PlayerPlane != null)
            PlayerPlane.Unregister(this);

    }
}
