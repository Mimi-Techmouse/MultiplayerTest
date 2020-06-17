using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class mt_spritesheet : MonoBehaviour {

    public string SpriteSheetName = "";
    protected Dictionary<string, Sprite> spriteSheet;
    public int index = 0;
    public int maxIndex = 0;

    protected Image spriteRenderer = null;
    public Image UIImage
    {
        get
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<Image>();
            return spriteRenderer;
        } 
    }

    // Use this for initialization
    public void Start()
    {
    	if (SpriteSheetName == "")
    		return;

        LoadSpriteSheet();
    }

    public void SetSpriteSheet(string sName) {
    	SpriteSheetName = sName;
    	LoadSpriteSheet();
    }

    // Loads the sprites from a sprite sheet
    public void LoadSpriteSheet()
    {
        // Load the sprites from a sprite sheet file (png). 
        // Note: The file specified must exist in a folder named Resources
        Debug.Log("loading sprite sheet for "+ this.SpriteSheetName);
		Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/"+this.SpriteSheetName);
        spriteSheet = sprites.ToDictionary(x => x.name, x => x);
        maxIndex = spriteSheet.Count-1;

    }

    public void SetSpriteByIndex(int n)
    {
        if (spriteSheet == null) 
            return;

        index = n;

        string sName = SpriteSheetName + "_" + n;
        if (spriteSheet.ContainsKey(sName))
            UIImage.sprite = spriteSheet[sName];
        else
            Debug.Log("asked for illegal key " + sName);
    }

	public virtual int GetMaxIndex() {
		return maxIndex;
	}
}