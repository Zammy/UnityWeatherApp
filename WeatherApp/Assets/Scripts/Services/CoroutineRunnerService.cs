using System.Collections;
using UnityEngine;

public interface ICoroutineRunnerService : IService
{
    public Coroutine StartCoroutine(IEnumerator routine);
    public void StopCoroutine(Coroutine routine);
}

public class CoroutineRunnerService : MonoBehaviour, ICoroutineRunnerService
{
    void Awake()
    {
        ServiceLocator.Instance.RegisterService(this);
    }

    void OnDestroy()
    {
        ServiceLocator.Instance.UnregisterService(this);
    }

}