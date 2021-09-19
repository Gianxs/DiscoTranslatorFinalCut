﻿
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace DiscoTranslatorFinalCut
{
    // Credits to Sinai at https://github.com/sinai-dev/Explorer

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "External methods")]
    public static class ICallHelper
    {
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr il2cpp_resolve_icall([MarshalAs(UnmanagedType.LPStr)] string name);

        private static readonly Dictionary<string, Delegate> iCallCache = new Dictionary<string, Delegate>();

        public static T GetICall<T>(string iCallName) where T : Delegate
        {
            if (iCallCache.ContainsKey(iCallName))
            {
                return (T)iCallCache[iCallName];
            }

            var ptr = il2cpp_resolve_icall(iCallName);

            if (ptr == IntPtr.Zero)
            {
                throw new MissingMethodException($"Could not resolve internal call by name '{iCallName}'!");
            }

            var iCall = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
            iCallCache.Add(iCallName, iCall);

            return (T)iCall;
        }
    }
}
