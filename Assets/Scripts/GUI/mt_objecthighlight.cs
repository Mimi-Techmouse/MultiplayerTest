using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mt_objecthighlight : MonoBehaviour {

	public bool isHighlighted = false;
	protected LayerMask layerMask;
	public List<HighlightObj> childRenderers;
	public Shader outlineShader;

	protected float highlightDuration = 0.05f;
	protected float lastHighlighted = 0.0f;

	[System.Serializable]
	public class HighlightObj {
		public Renderer myRenderer;
		public Shader originalShader;

		public HighlightObj(Renderer r) {
			myRenderer = r;
			if (r != null) {
				originalShader = r.material.shader;
			}
		}
	}

	private void Awake() {
		InitializeRendererList ();
	}
    
    private void Start() {
		outlineShader = Shader.Find ("Outlined/UltimateOutline");
    }

	public void StartHighlight() {
		Debug.Log ("highlighting " + this.name);
		if (!isHighlighted) {
			SwapShaders ();
			//StartCoroutine (CheckForHighlightEnd ());
		}

		lastHighlighted = Time.time;
	}

	public void StopHighlight() {
		RestoreShaders ();
	}

	protected virtual void InitializeRendererList() {
		childRenderers = new List<HighlightObj> ();
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach (Renderer r in renderers) {
			if (r.gameObject.GetComponent<ParticleSystem> () != null)
				continue;
			childRenderers.Add (new HighlightObj (r));
		}

		if (GetComponent<Renderer> () != null)
			childRenderers.Add(new HighlightObj (GetComponent<Renderer> ()));
	}

	public virtual void SwapShaders() {
		isHighlighted = true;
		foreach (HighlightObj obj in childRenderers) {
			if (!obj.myRenderer.gameObject.activeSelf)
				continue;
			obj.myRenderer.material.shader = outlineShader;
		}
	}

	//Restore shaders
	protected virtual void RestoreShaders() {
		Debug.Log ("stopping highlight " + this.name);
		isHighlighted = false;
		foreach (HighlightObj obj in childRenderers) {
			obj.myRenderer.material.shader = obj.originalShader;
		}
	}

	//highlightDuration
	IEnumerator CheckForHighlightEnd() {
		while ((Time.time-lastHighlighted) < highlightDuration) {
			yield return null;
		}
		RestoreShaders ();
	}

}
