using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_targetdisplay : mt_spritesheet {

	private float lastTested = 0.0f;
	public float updateSpeed = 1.0f;

	public void Update() {

		if (SpriteSheetName == "")
			return;

		if (Time.time - lastTested < updateSpeed)
			return;

		int i = (index+1)%maxIndex;
		Debug.Log("index: "+index+" % "+maxIndex+" = "+i);
		SetSpriteByIndex(i);
		lastTested = Time.time;
	}

}
