using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_playersensor : MonoBehaviour
{
	public GameObject LockTarget = null;
	public GameObject LockSprite;
	public mt_targetdisplay TargetDisplay = null;

	public List<GameObject> enemies = null;
	private float lastTested = 0.0f;
	public float senseInterval = 1.0f;
	public float senseRadius = 100.0f;

	public GameObject MapSpace = null;
	public GameObject SpinnyThing = null;
	public List<GameObject> mapObject = null;

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

    // Don't check for enemies too often!
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

		PositionLockUI();
		DrawMap();
	}

	public void PositionLockUI() {
		if (LockTarget == null) {
			LockSprite.SetActive(false);
			return;
		}

		Vector2 screenPoint = Camera.main.WorldToScreenPoint(LockTarget.transform.position);
		Vector2 crossHairPoint = Camera.main.WorldToScreenPoint(PlayerPlane.Crosshair.Get().transform.position);
		Vector2 toMove = screenPoint-crossHairPoint;

		Vector2 center = new Vector2(Screen.width/2, Screen.height/2);

		LockSprite.SetActive(true);
		LockSprite.transform.localPosition = new Vector3(toMove.x, toMove.y, 0);

		/*if (screenPoint.x < Screen.width && screenPoint.y < Screen.height && screenPoint.x >= 0 && screenPoint.y >= 0) {
			LockSprite.SetActive(false);
			return;
		} else {
			LockSprite.SetActive(true);
		}*/
	}

	public void GetSensed() {

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

	public virtual void OnMessage_FindAndLockTarget() {
		float thickness = 1.5f; 
		Vector3 origin = PlayerPlane.Crosshair.Get().transform.position;
		Vector3 direction = transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.SphereCast(origin, thickness, direction, out hit, senseRadius)) {
			Transform item = hit.collider.transform;
			mt_thingofinterest itemHandler = item.GetComponent<mt_thingofinterest>();
			if (itemHandler != null && itemHandler.gameObject != gameObject) {
				Debug.Log("locking on to: "+item.name);
				SetLockTarget(itemHandler.gameObject);
			}
		}
	}

	public void SetLockTarget(GameObject t) {
		mt_objecthighlight highlight;
		if (LockTarget != null) {
			highlight = LockTarget.GetComponent<mt_objecthighlight>();
			if (highlight != null) {
				highlight.StopHighlight();
			}
		}

		LockTarget = t;

		highlight = t.GetComponent<mt_objecthighlight>();
		if (highlight != null) {
			highlight.StartHighlight();
		}

		mt_thingofinterest itemHandler = LockTarget.GetComponent<mt_thingofinterest>();
		TargetDisplay.SetSpriteSheet(itemHandler.SpriteName);
		TargetDisplay.SpriteSheetName = itemHandler.SpriteName;
	}

    protected virtual GameObject OnValue_LockTarget {
        get {
            return LockTarget;
        }
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
