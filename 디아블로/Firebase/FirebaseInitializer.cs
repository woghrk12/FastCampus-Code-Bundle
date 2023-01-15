using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FirebaseInitializer
{
    private static List<Action<DependencyStatus>> initializeCallbacks = new List<Action<DependencyStatus>>();
    private static DependencyStatus dependencyStatus;

    private static bool isInitialized = false;

    public static void Initialize(Action<DependencyStatus> p_callback)
    {
        lock (initializeCallbacks)
        {
            if (isInitialized)
            {
                p_callback(dependencyStatus);
                return;
            }

            initializeCallbacks.Add(p_callback);
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(t_task => {
                lock (initializeCallbacks)
                {
                    dependencyStatus = t_task.Result;
                    isInitialized = true;
                    CallInitializedCallbacks();
                }
            });
        }
    }

    private static void CallInitializedCallbacks()
    {
        lock (initializeCallbacks)
        {
            foreach (var t_callback in initializeCallbacks)
            {
                t_callback(dependencyStatus);
            }

            initializeCallbacks.Clear();
        }
    }
}
