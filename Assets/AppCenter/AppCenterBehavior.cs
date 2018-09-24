﻿// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.AppCenter.Unity;
using UnityEngine;
using System;
using System.Reflection;
using Microsoft.AppCenter.Unity.Internal;
using System.Linq;

[HelpURL("https://docs.microsoft.com/en-us/appcenter/sdk/crashes/unity")]
public class AppCenterBehavior : MonoBehaviour
{
    public static event Action InitializingServices;
    public static event Action InitializedAppCenterAndServices;
    public static event Action Started;

    private static AppCenterBehavior instance;

    public AppCenterSettings settings;

    private void Awake()
    {
        // Make sure that App Center have only one instance.
        if (instance != null)
        {
            Debug.LogError("App Center should have only one instance!");
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initialize App Center.
        if (settings == null)
        {
            Debug.LogError("App Center isn't configured!");
            return;
        }
        StartAppCenter();

        if (Started != null)
        {
            Started.Invoke();
        }
    }

    private void StartAppCenter()
    {
        var services = settings.Services;
        PrepareEventHandlers(services);
        InvokeInitializingServices();
        AppCenter.SetWrapperSdk();

        // On iOS we start crash service here, to give app an opportunity to assign handlers after crash and restart in Awake method
#if UNITY_IOS
        foreach (var service in services)
        {
            var startCrashes = service.GetMethod("StartCrashes");
            if (startCrashes != null)
                startCrashes.Invoke(null, null);
        }
#endif

        // On iOS and Android App Center starting automatically.
#if UNITY_EDITOR || (!UNITY_IOS && !UNITY_ANDROID)
        AppCenter.LogLevel = settings.InitialLogLevel;
        if (settings.CustomLogUrl.UseCustomUrl)
        {
            AppCenter.SetLogUrl(settings.CustomLogUrl.Url);
        }
        var appSecret = AppCenter.GetSecretForPlatform(settings.AppSecret);
        var nativeServiceTypes = AppCenter.ServicesToNativeTypes(services);
        AppCenterInternal.Start(appSecret, nativeServiceTypes, services.Length);
#endif
        InvokeInitializedServices();
    }

    private static void PrepareEventHandlers(Type[] services)
    {
        foreach (var service in services)
        {
            var method = service.GetMethod("PrepareEventHandlers");
            if (method != null)
            {
                method.Invoke(null, null);
            }
        }
    }

    private static void InvokeInitializingServices()
    {
        if (InitializingServices != null)
        {
            InitializingServices.Invoke();
        }
    }

    private static void InvokeInitializedServices()
    {
        if (InitializedAppCenterAndServices != null)
        {
            InitializedAppCenterAndServices.Invoke();
        }
    }
}
