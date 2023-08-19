using UnityEngine;

namespace Toastylib
{
    public class MyAndroidJavaObject
    {
        public MyAndroidJavaObject() { }

#if UNITY_ANDROID
        public MyAndroidJavaObject(AndroidJavaObject javaObject)
        {
            _javaObject = javaObject;
        }
#endif

        internal void Call(string method)
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
            _javaObject.Call(method);
#endif
        }

        internal void RunOnUIThread()
        {
#if UNITY_EDITOR
            Toasty.ShowToast();
#elif UNITY_ANDROID
            _javaObject.Call("runOnUiThread", new AndroidJavaRunnable(Toasty.ShowToast));
#endif
        }

        internal AndroidJavaObject CallNoParams(string method)
        {
#if UNITY_EDITOR
            return null;
#elif UNITY_ANDROID
            return _javaObject.Call<AndroidJavaObject>(method);
#endif
        }
#if UNITY_ANDROID
        AndroidJavaObject _javaObject;
#endif
    }

    public class MyAndroidJavaClass
    {
#if UNITY_EDITOR
        public string ToastText { get; private set; }
#endif

        public MyAndroidJavaClass(string className)
        {
#if UNITY_EDITOR
             return;
#elif UNITY_ANDROID
            _javaClass = new AndroidJavaClass(className);
#endif
        }

        internal MyAndroidJavaObject GetStaticObject(string fieldName)
        {
#if UNITY_EDITOR
            return new MyAndroidJavaObject();
#elif UNITY_ANDROID
            return new MyAndroidJavaObject(_javaClass.GetStatic<AndroidJavaObject>(fieldName));
#endif
        }

        internal MyAndroidJavaObject CallMakeText(AndroidJavaObject context, string toastText)
        {
#if UNITY_EDITOR
            ToastText = toastText;
            return new MyAndroidJavaObject();
#elif UNITY_ANDROID
            return new MyAndroidJavaObject(_javaClass.CallStatic<AndroidJavaObject>("makeText", context, new AndroidJavaObject("java.lang.String", toastText), _javaClass.GetStatic<int>("LENGTH_LONG")));
#endif
        }

#if UNITY_ANDROID
        AndroidJavaClass _javaClass;
#endif
    }
}
