using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_turretgunrotation : MonoBehaviour
{

    public float MaxRotation = 90.0f; //the amount you are allowed to stray from base angle
    private Quaternion baseRotation;
    private Quaternion targetRotation;

	protected mt_EventHandler m_PlayerPlane = null;
    public mt_EventHandler PlayerPlane
    {
        get
        {
            if (m_PlayerPlane == null)
                m_PlayerPlane = transform.root.GetComponent<mt_EventHandler>();
            return m_PlayerPlane;
        }
    }

	void Start() {
		baseRotation = transform.rotation;
	}

	void Update() {

		mt_EventHandler target = PlayerPlane.CurrentTarget.Get();
		if (target == null)
			return;
		//Debug.Log("target: "+target.name);

		Vector3 look = target.transform.position - transform.position;
         
        Quaternion q = Quaternion.LookRotation(look);
        q.z = 0;

        //We are within bounds! Get 'im!
        //Debug.Log("look: "+look);
        //Debug.Log("within angle? "+Quaternion.Angle (q, baseRotation));
        if (Quaternion.Angle (q, baseRotation) <= MaxRotation) {
            targetRotation = q;
        } 
         
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);

	}
}
