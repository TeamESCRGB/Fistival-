using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utils
{
    public static class DebugUtils
    {
        public static void UnImplemented<T>(T arg,[CallerMemberName] string methodName ="")
        {
            Debug.LogError($"UnImplemented Error in {methodName} in {typeof(T).Name}");
        }
    }
}
