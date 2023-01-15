using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

public class FirebaseAuthUIController : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    
    public TMP_Text outputText;

    private void Start()
    {
        FirebaseAuthController.Instance.OnChangedLoginState += OnChangedLoginState;
        FirebaseAuthController.Instance.InitializeFirebase();
    }

    private void OnChangedLoginState(bool p_signedIn)
    {
        outputText.text = p_signedIn ? "Signed In : " : "Signed Out : ";
        outputText.text += FirebaseAuthController.Instance.UserId;
    }

    public void CreateUser()
    {
        string t_email = emailInputField.text;
        string t_password = passwordInputField.text;

        FirebaseAuthController.Instance.CreateUser(t_email, t_password);
    }

    public void SignIn()
    {
        string t_email = emailInputField.text;
        string t_password = passwordInputField.text;

        FirebaseAuthController.Instance.SignIn(t_email, t_password);
    }

    public void SignOut()
    {
        FirebaseAuthController.Instance.SignOut();
    }
}
