using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_playersensor : MonoBehaviour
{
	public List<GameObject> enemies = null;
	private float lastTested = 0.0f;
	public float senseInterval = 1.0f;
	public float senseRadius = 100.0f;

	public GameObject MapSpace = null;
	public GameObject SpinnyThing = null;
	public List<GameObject> mapObject = null;

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

		if (!PlayerPlane.isLocalPlayer.Get())
			return;

		if (Time.time - lastTested < senseInterval)
			return;

		GetSensed ();

	}

	public void LateUpdate() {

		if (!PlayerPlane.isLocalPlayer.Get())
			return;

		DrawMap();
	}

	public void GetSensed() {

		Debug.Log("fetching enemies.");

		float closestEnemey = Mathf.Infinity;

		lastTested = Time.time;
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, senseRadius);
		enemies = new List<GameObject> ();
		int n = 0;
		while (n < hitColliders.Length) {

			Transform item = hitColliders[n].transform;
			mt_thingofinterest itemHandler = item.GetComponent<mt_thingofinterest>();
			if (itemHandler != null && itemHandler.gameObject != gameObject) {
				//Debug.Log("found: "+item.name);
				enemies.Add(itemHandler.gameObject);
			}

			n++;

		}

	}

	public void DrawMap() {

		SpinnyThing.transform.rotation = transform.rotation;

		if (mapObject == null)
			mapObject = new List<GameObject> ();

		int n = 0;
		foreach (GameObject i in enemies) {

			if (i != null) {
				if (n >= mapObject.Count) {
					GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					mapObject.Add(newObj);
					mapObject[n].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
					mapObject[n].GetComponent<Renderer>().material.color = new Color(1,0,0,1);
				} else {
					mapObject[n].SetActive(true);
				}

				Vector3 enemyPos = i.transform.position;
				Vector3 mapPos = (enemyPos-transform.position).normalized;

				mapObject[n].transform.position = MapSpace.transform.position+(mapPos/15);
				mapObject[n].transform.parent = MapSpace.transform;
			}

			n++;
		}

		for (int i = n; i < mapObject.Count; i++) {
			mapObject[n].SetActive(false);
		}
	}
}
