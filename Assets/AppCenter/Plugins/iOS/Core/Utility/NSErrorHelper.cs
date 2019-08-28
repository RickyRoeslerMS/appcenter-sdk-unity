// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace Microsoft.AppCenter.Unity.Internal.Utility
{
    public static partial class NSErrorHelper
    {
        public static System.Exception ToSystemException(IntPtr nsErrorPtr)
        {
            if (nsErrorPtr == IntPtr.Zero)
            {
                return null;
            }
            var domain = app_center_unity_nserror_domain(nsErrorPtr);
            var errorCode = app_center_unity_nserror_code(nsErrorPtr);
            var description = app_center_unity_nserror_description(nsErrorPtr);
            return new System.Exception(string.Format("Domain: {0}, error code: {1}, description: {2}", domain, errorCode, description));
        }

        [DllImport("__Internal")]
        private static extern string app_center_unity_nserror_domain(IntPtr error);

        [DllImport("__Internal")]
        private static extern long app_center_unity_nserror_code(IntPtr error);

        [DllImport("__Internal")]
        private static extern string app_center_unity_nserror_description(IntPtr error);
    }
}
#endif
