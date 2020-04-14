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

        Debug.Log(other.transform.parent.name+" trigger entered!");

        CollideWithOther(other.transform.parent.gameObject);

    }

	/// *********************************************************************************** /// 
	/// On collision enter
	/// *********************************************************************************** /// 
    public void OnCollisionEnter(Collision other) {

        Debug.Log(other.gameObject.name+" collider entered!");

    	float force = 3.0f;

    	Vector3 dir = other.contacts[0].point - transform.position;
        dir = -dir.normalized;
        other.gameObject.GetComponent<Rigidbody>().AddForce(dir*force);

        CollideWithOther(other.transform.parent.gameObject);

    }


	/// *********************************************************************************** /// 
	/// Collision handling 
	/// *********************************************************************************** /// 
    protected void CollideWithOther(GameObject other) {

    	SoundSource.clip = screech;
    	SoundSource.Play();

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(DamageToDo, other.transform.position, gameObject);
        other.SendMessage ("ApplyDamage", damageToSend, SendMessageOptions.DontRequireReceiver);
    }
    
}
