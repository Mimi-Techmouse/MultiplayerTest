using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class mt_infopanel : MonoBehaviour
{
    public TMP_Text Title = null;
    public TMP_Text Content = null;

    public void TogglePanel(bool show, string sT = "", string sC = "") {
    	Title.text = sT;
    	Content.text = sC;

    	gameObject.SetActive(show);
    }
}
