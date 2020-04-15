using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro.Examples
{
public class mt_ui_healthbar : MonoBehaviour {

	public TMP_Text HealthReadout = null;

    public mt_EventHandler m_PlayerPlane = null;
    public mt_EventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.root.transform.GetComponent<mt_EventHandler>();
            return m_PlayerPlane;
        }
    }

    protected virtual void Awake() {
    	UpdateHealthBar();
    }

    /// <summary>
    /// Apply Damage to current health
    /// </summary>
    protected virtual void OnMessage_HealthChanged() {

    	Debug.Log("health bar should update!");
    	UpdateHealthBar();
    }

    /// <summary>
    /// Update Health Bar
    /// </summary>
    public void UpdateHealthBar() {
    	HealthReadout.text = "Health: "+PlayerPlane.Health.Get()+" / 100";
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
}
