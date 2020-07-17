using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Starter : MonoBehaviour
{
    public GameObject serverObject;
    public GameObject clientObject;

    public void becomeServer()
    {
        Instantiate(serverObject);
        this.gameObject.SetActive(false);
    }

    public void becomeClient()
    {
        Instantiate(clientObject);
        //We turn off the canvas through client code to make sure we get all the values we need before turning it off. 
        //Doing this is very messy and awful code. Too bad!
    }

}
