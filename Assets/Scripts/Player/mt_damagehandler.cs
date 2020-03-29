using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_damagehandler : MonoBehaviour {

	//Wants generic state handler so that it can apply to everything
	protected mt_EventHandler m_Handler = null;
	public mt_EventHandler Handler
	{
		get
		{
			if (m_Handler == null)
				m_Handler = transform.GetComponent<mt_EventHandler>();
			if (m_Handler == null)
				m_Handler = transform.GetComponentInParent <mt_EventHandler> ();
			return m_Handler;
		}
	}

	/// *********************************************************************************** /// 
	/// Damage Applicator Functions
	/// Creates a standardized way to apply damage to anything
	/// Calls the native DamageMe message once resistances are applied
	/// Damage may be passed in as float, int, or damage class
	/// *********************************************************************************** /// 
	public void ApplyDamage(mt_Constants.Damage dam) {

        Debug.Log(transform.gameObject.name + " says: Ouch");

		//Blocks may need proper handlers
		if ((Handler.Health.Get () - dam.DamageAmount) <= 0) {
			if (dam.Damager != null)
				dam.Damager.SendMessage ("KilledSomething", m_Handler, SendMessageOptions.DontRequireReceiver);
		}

		Handler.DamageMe.Send (dam.DamageAmount);
        
	}


    public void OnTriggerEnter(Collider other) {
    	Debug.Log("something hit my trigger!");
    }

    public void OnCollisionEnter(Collision collision) {
    	Debug.Log("something collided with meeee!");
    }
}
