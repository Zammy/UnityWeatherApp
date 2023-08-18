using UnityEngine;
using Toastylib;

public class SendToastOnButtonClick : MonoBehaviour
{
    public void OnButtonClicked()
    {
        Toasty.DisplayToastMessage("This is a msg");
    }
}
