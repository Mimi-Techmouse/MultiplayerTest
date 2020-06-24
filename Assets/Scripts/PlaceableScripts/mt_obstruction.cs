using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is for large objects which will significantly damage the player
//if you run into them
public class mt_obstruction : MonoBehaviour {

	public int DamageToDo = 50;
	public AudioClip screech = null;

	protected mt_EventHandler m_EventHandler = null;
    public mt_EventHandler EventHandler
    {
        get
        {
            if (m_EventHandler == null)
                m_EventHandler = transform.parent.GetComponent<mt_EventHandler>();
            return m_EventHandler;
        }
    }

    protected AudioSource m_AudioSource = null;
    public AudioSource SoundSource
    {
        get
        {
            if (m_AudioSource == null)
                m_AudioSource = gameObject.AddComponent<AudioSource>();
            return m_AudioSource;
        }
    }

    public void Awake() {
    	screech = Resources.Load<AudioClip>("Sounds/ImpactScreech");
    }

	/// *********************************************************************************** /// 
	/// Damage Applicator Function
	/// Calls the parent damagehandler function
	/// *********************************************************************************** /// 
	public void ApplyDamage(mt_Constants.Damage dam) {
		mt_damagehandler handler = EventHandler.gameObject.GetComponent<mt_damagehandler>();
		Debug.Log("forwarding damage up the chain");
		handler.ApplyDamage(dam);
	}

	/// *********************************************************************************** /// 
	/// On Trigger enter
	/// *********************************************************************************** /// 
	public void OnTriggerEnter(Collider other) {

        Debug.Log(transform.name+": "+other.transform.parent.name+" trigger entered!");

        float force = 3.0f;

        Vector3 dir = -other.transform.forward.normalized;
        GameObject target = GetDamageHandlerObject(other.gameObject);
        if (target == null)
            return;

        target.SendMessage ("AddImpact", dir, SendMessageOptions.DontRequireReceiver);
        CollideWithOther(target);

    }

	/// *********************************************************************************** /// 
	/// On collision enter
	/// *********************************************************************************** /// 
    public void OnCollisionEnter(Collision other) {

        Debug.Log(transform.name+": "+other.gameObject.name+" collider entered!");

    	float force = 3.0f;

    	Vector3 dir = (other.contacts[0].point - transform.position).normalized;
        GameObject target = GetDamageHandlerObject(other.gameObject);
        if (target == null)
            return;

        target.SendMessage ("AddImpact", dir, SendMessageOptions.DontRequireReceiver);
        CollideWithOther(target);

    }

	/// *********************************************************************************** /// 
	/// Collision handling 
	/// *********************************************************************************** /// 
    protected void CollideWithOther(GameObject other) {

        Debug.Log("doing the collision");

    	SoundSource.clip = screech;
    	SoundSource.Play();

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(DamageToDo, other.transform.position, gameObject);
        other.SendMessage ("ApplyDamage", damageToSend, SendMessageOptions.DontRequireReceiver);
    }

    /// *********************************************************************************** /// 
    /// Get that damage handler!
    /// *********************************************************************************** /// 
    public GameObject GetDamageHandlerObject(GameObject other) {

        if (other.GetComponent<mt_damagehandler>() != null) {
            return other;
        }
        if (other.transform.parent.gameObject.GetComponent<mt_damagehandler>() != null) {
            return other.transform.parent.gameObject;
        }

        return null;
    }
    
}
