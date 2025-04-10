using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SecondMenu : MonoBehaviour
{
    public void BackBT()
    {
        GameObject.Find("SecondMenu").SetActive(false);
    }
    public async void LoginBT()
    {
      
        string name = GameObject.Find("NameInput").GetComponent<TMP_InputField>().text;
        string password = GameObject.Find("PasswordInput").GetComponent<TMP_InputField>().text;
        string email = GameObject.Find("EmailInput").GetComponent<TMP_InputField>().text;

        await MainData.UIFunctions.LogIn(name, email, password);//async

        if (MainData.Main.playerData != null)
        {
            GameObject.Find("SecondMenu").SetActive(false);
        }
    }
}
