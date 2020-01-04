using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt : MonoBehaviour
{
    public enum RESPONSE_USER {
    	NONE, 				//0
    	NO_USERNAME, 		//1
    	NO_PASSWORD,		//2
    	NO_EMAIL,			//3
    	BAD_USERNAME,		//4
    	REGISTER_FAIL,		//5
    	REGISTER_SUCCESS,	//6
    	DOESNT_EXIST,		//7
    	LOGIN_FAIL,			//8
    	LOGIN_SUCCESS		//9
    }

}
