using System.Collections.Generic;

namespace Toastylib
{
    public static class Toasty
    {
#if UNITY_EDITOR
        public static MyAndroidJavaClass sToastClass;
#endif
        public static void DisplayToastMessage(string text)
        {
            sMessagesQueue.Enqueue(text);

            var unityPlayer = new MyAndroidJavaClass("com.unity3d.player.UnityPlayer");
            sActivity = unityPlayer.GetStaticObject("currentActivity");
            sActivity.RunOnUIThread();
        }

        internal static void ShowToast()
        {
            var context = sActivity.CallNoParams("getApplicationContext");
            var toast = new MyAndroidJavaClass("android.widget.Toast");
            var toastInstance = toast.CallMakeText(context, sMessagesQueue.Dequeue());
            toastInstance.Call("show");

#if UNITY_EDITOR
            sToastClass = toast;
#endif
        }

        static Queue<string> sMessagesQueue = new Queue<string>();
        static MyAndroidJavaObject sActivity;
    }
}
