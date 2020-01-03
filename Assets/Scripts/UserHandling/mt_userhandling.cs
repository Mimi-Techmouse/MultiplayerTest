using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class mt_userhandling : MonoBehaviour {

	public TMP_InputField r_nameField = null;
	public TMP_InputField r_passField = null;
	public TMP_InputField r_emailField = null;

	public TMP_InputField l_nameField = null;
	public TMP_InputField l_passField = null;

	public void RegisterUser() {
        StartCoroutine(RunRegister());
	}
    
    IEnumerator RunRegister() {

    	string url = "http://multiplayer.ninjachip.com/process/register.php";

    	string sName = r_nameField.text;
    	string sPassword = r_passField.text;
    	string sEmail = r_emailField.text;

	    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("username="+sName+"&password="+sPassword+"&email="+sEmail));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    } 
}