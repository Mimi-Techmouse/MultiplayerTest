using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class mt_userhandling : MonoBehaviour {

    public GameObject LoginPanel = null;
    public GameObject RegisterPanel = null;
    public GameObject InfoPanel = null;
    public TMP_Text InfoText = null;
    public Button InfoButton = null;

	public TMP_InputField r_nameField = null;
	public TMP_InputField r_passField = null;
	public TMP_InputField r_emailField = null;

	public TMP_InputField l_nameField = null;
	public TMP_InputField l_passField = null;

    public bool sendingData = false;
    public mt.RESPONSE_USER lastResponse = mt.RESPONSE_USER.NONE;

    private void Awake() {

        if (PlayerPrefs.HasKey("user_name")) {
            l_nameField.text = PlayerPrefs.GetString("user_name");
        }
        if (PlayerPrefs.HasKey("user_password")) {
            l_passField.text = PlayerPrefs.GetString("user_password");
        }

    }

	public void RegisterUser() {
        StartSend();
        StartCoroutine(GetRegister());
	}

    public void LoginUser() {
        StartSend();
        StartCoroutine(GetLogin());
    }

    //Clear the immportant variables and send data
    public void StartSend() {
        sendingData = true;
        lastResponse = mt.RESPONSE_USER.NONE;
        DisplaySendingScreen();
    }

    //For when we're sending data to the server
    public void DisplaySendingScreen() {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        InfoPanel.SetActive(true);
        InfoText.text = "Thinking. . .";
        InfoButton.interactable = false;
    }

    public void DisplayResponse() {

        InfoButton.interactable = true;
        switch(lastResponse) {
            case mt.RESPONSE_USER.NONE:                 InfoText.text = "No response from server."; InfoButton.interactable = false; break;
            case mt.RESPONSE_USER.NO_USERNAME:          InfoText.text = "No username provided."; break;
            case mt.RESPONSE_USER.NO_PASSWORD:          InfoText.text = "No password provided."; break;
            case mt.RESPONSE_USER.NO_EMAIL:             InfoText.text = "No email provided."; break;
            case mt.RESPONSE_USER.BAD_USERNAME:         InfoText.text = "Username already taken."; break;
            case mt.RESPONSE_USER.REGISTER_FAIL:        InfoText.text = "Failed to register new user."; break;
            case mt.RESPONSE_USER.REGISTER_SUCCESS:     InfoText.text = "Success! You may log in now."; break;
            case mt.RESPONSE_USER.DOESNT_EXIST:         InfoText.text = "Username does not exist."; break;
            case mt.RESPONSE_USER.LOGIN_FAIL:           InfoText.text = "Log in attempt failed. Check your password and try again."; break;
            case mt.RESPONSE_USER.LOGIN_SUCCESS:        InfoText.text = "Success! You are logged in."; break;
        }

    }

    public void InfoButtonClick() {

        if (lastResponse == mt.RESPONSE_USER.NONE) {
            InfoButton.interactable = false;
            return;
        }

        switch(lastResponse) {
            case mt.RESPONSE_USER.NONE:                 InfoText.text = "No response from server."; InfoButton.interactable = false; break;
            case mt.RESPONSE_USER.NO_USERNAME:          RegisterPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.NO_PASSWORD:          RegisterPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.NO_EMAIL:             RegisterPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.BAD_USERNAME:         RegisterPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.REGISTER_FAIL:        RegisterPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.REGISTER_SUCCESS:     LoginPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.DOESNT_EXIST:         LoginPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.LOGIN_FAIL:           LoginPanel.SetActive(true); InfoPanel.SetActive(false); break;
            case mt.RESPONSE_USER.LOGIN_SUCCESS:        SceneManager.LoadScene("Menu_Lobby"); break;
        }

    }
    
    //Swap out to this when on the same server
    //Might be a conflict with running post from my computer
    IEnumerator PostRegister() {

        string url = "http://multiplayer.ninjachip.com/process/registerUser.php";

    	string sName = r_nameField.text;
    	string sPassword = r_passField.text;
    	string sEmail = r_emailField.text;

	    List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("username="+sName+"&password="+sPassword+"&email="+sEmail));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();
        sendingData = false;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("info sent.");
            Debug.Log(www.downloadHandler.text);
            sendingData = false;
        }
    } 

    //Register the player; sending form via _GET variables
    //Change later when code is all on the same server
    IEnumerator GetRegister() {

        string url = "http://multiplayer.ninjachip.com/process/registerUser.php";

        string sName = r_nameField.text;
        string sPassword = r_passField.text;
        string sEmail = r_emailField.text;

        url += "?username="+sName+"&password="+sPassword+"&email="+sEmail;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            sendingData = false;

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError) {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            } else {
                lastResponse = (mt.RESPONSE_USER)int.Parse(webRequest.downloadHandler.text);

                //Save the registered username + password if successful
                if (lastResponse == mt.RESPONSE_USER.REGISTER_SUCCESS) {
                    PlayerPrefs.SetString("user_name", sName);
                    PlayerPrefs.SetString("user_password", sPassword);
                    l_nameField.text = sName;
                    l_passField.text = sPassword;
                }

                DisplayResponse();
            }
        }
    }

    //Log in the player; sending form via _GET variables
    //Change later when code is all on the same server
    IEnumerator GetLogin() {

        string url = "http://multiplayer.ninjachip.com/process/loginUser.php";

        string sName = l_nameField.text;
        string sPassword = l_passField.text;

        url += "?username="+sName+"&password="+sPassword;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            sendingData = false;

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError) {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            } else {
                lastResponse = (mt.RESPONSE_USER)int.Parse(webRequest.downloadHandler.text);
                if (lastResponse == mt.RESPONSE_USER.LOGIN_SUCCESS) {
                    PlayerPrefs.SetString("user_name", sName);
                    PlayerPrefs.SetString("user_password", sPassword);
                }

                DisplayResponse();
            }
        }
    }
}