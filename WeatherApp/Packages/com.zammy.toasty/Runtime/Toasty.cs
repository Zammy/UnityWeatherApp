using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toastylib
{
    public static class Toasty
    {
        public static void DisplayToastMessage(string text)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Debug.LogError("Not running on Android!");
                return;
            }

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            sActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _messagesQueue.Enqueue(text);
            sActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowToast));
        }

        internal static void ShowToast()
        {
            Debug.Log("ShowToast");
            AndroidJavaObject context = sActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaClass toast = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", _messagesQueue.Dequeue());
            AndroidJavaObject toastInstance = toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, toast.GetStatic<int>("LENGTH_LONG"));
            toastInstance.Call("show");
        }

        static Queue<string> _messagesQueue = new Queue<string>();
        static AndroidJavaObject sActivity;
    }
}
