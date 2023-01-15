using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthController
{
    private static FirebaseAuthController instance = null;
    public static FirebaseAuthController Instance 
    {
        get 
        {
            if (instance == null)
                instance = new FirebaseAuthController();

            return instance;
        }
    }

    private FirebaseAuth auth;
    private FirebaseUser user;

    private string displayName;
    private string emailAddress;
    private Uri photoUrl;

    public Action<bool> OnChangedLoginState;

    public string UserId => user?.UserId ?? string.Empty;
    public string DisplayName => displayName;
    public string EmailAddress => emailAddress;
    public Uri PhotoUrl => photoUrl;

    public void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnAuthStateChanged;

        OnAuthStateChanged(this, null);
    }

    private void OnAuthStateChanged(object p_sender, EventArgs p_eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool t_signedIn = (auth.CurrentUser != user && auth.CurrentUser != null);
            if (!t_signedIn && user != null)
            {
                Debug.Log("Signed out : " + user.UserId);
                OnChangedLoginState?.Invoke(false);
            }

            user = auth.CurrentUser;
            if (t_signedIn)
            {
                Debug.Log("Signed in : " + user.UserId);
                displayName = user.DisplayName ?? string.Empty;
                emailAddress = user.Email ?? string.Empty;
                photoUrl = user.PhotoUrl ?? null;
                OnChangedLoginState?.Invoke(true);
            }
        }
    }

    public void CreateUser(string p_email, string p_password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(p_email, p_password).ContinueWith(t_task => {
            if (t_task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (t_task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error : " + t_task.Exception);

                int t_errorCode = GetFirebaseErrorCode(t_task.Exception);
                switch (t_errorCode)
                {
                    case (int)AuthError.EmailAlreadyInUse:
                        Debug.LogError("Email Already In Use");
                        break;
                    case (int)AuthError.InvalidEmail:
                        Debug.LogError("Invalid Email");
                        break;
                    case (int)AuthError.WeakPassword:
                        Debug.LogError("Weak Password");
                        break;
                }
                return;
            }

            FirebaseUser t_newUser = t_task.Result;
            Debug.LogFormat("Firebase user created successfully : {0}  ({1})", t_newUser.DisplayName, t_newUser.UserId);
        });
    }

    private int GetFirebaseErrorCode(AggregateException p_exception)
    {
        FirebaseException t_firebaseException= null;

        foreach (Exception t_exception in p_exception.Flatten().InnerExceptions)
        {
            t_firebaseException = t_exception as FirebaseException;
            if (t_firebaseException != null) break;
        }

        return t_firebaseException?.ErrorCode ?? 0;
    }

    public void SignIn(string p_email, string p_password)
    {
        auth.SignInWithEmailAndPasswordAsync(p_email, p_password).ContinueWith(t_task => {
            if (t_task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (t_task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error : " + t_task.Exception);

                int t_errorCode = GetFirebaseErrorCode(t_task.Exception);
                switch (t_errorCode)
                {
                    case (int)AuthError.WrongPassword:
                        Debug.LogError("Wrong Password");
                        break;
                    case (int)AuthError.UnverifiedEmail:
                        Debug.LogError("Unverified Email");
                        break;
                    case (int)AuthError.InvalidEmail:
                        Debug.LogError("Invalid Email");
                        break;
                }
                return;
            }

            FirebaseUser t_newUser = t_task.Result;
            Debug.LogFormat("Firebase signed in successfully : {0}  ({1})", t_newUser.DisplayName, t_newUser.UserId);
        });
    }

    public void SignOut()
    {
        auth.SignOut();
    }
}
