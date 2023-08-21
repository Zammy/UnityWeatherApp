using System;
using System.Collections;
using UnityEngine;

public interface ILocationService : IService, IInitializable
{
    Action<LocationService.Status> OnStatusChanged { get; set; }
    void StartLocationSeeking(int accuracyInMeters);
    void StopLocationSeeking();
    LocationInfo GetCurrentLocationInfo();
}

public class LocationService : ILocationService
{
    public enum Status
    {
        Stopped = LocationServiceStatus.Stopped,
        Initializing = LocationServiceStatus.Initializing,
        Running = LocationServiceStatus.Running,
        Failed = LocationServiceStatus.Failed,
        NotEnabledByUser
    }

    public Action<LocationService.Status> OnStatusChanged { get; set; }

    public void Init()
    {
        _coroutineRunner = ServiceLocator.Instance.GetService<ICoroutineRunnerService>();
    }

    public LocationInfo GetCurrentLocationInfo()
    {
        return Input.location.lastData;
    }

    public void StartLocationSeeking(int accuracyInMeters)
    {
        _locationServiceCoroutine = _coroutineRunner.StartCoroutine(InitLocationService(accuracyInMeters));
    }

    public void StopLocationSeeking()
    {
        if (_locationServiceCoroutine != null)
        {
            _coroutineRunner.StopCoroutine(_locationServiceCoroutine);
        }
        Input.location.Stop();
    }

    IEnumerator InitLocationService(int accuracyInMeters)
    {
        Input.location.Start(accuracyInMeters);

        if (!Input.location.isEnabledByUser)
        {
            this?.OnStatusChanged(Status.NotEnabledByUser);
            yield break;
        }

        var waitForASecond = new WaitForSeconds(1f);
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            this?.OnStatusChanged(Status.Initializing);
            yield return waitForASecond;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            this?.OnStatusChanged(Status.Failed);
            yield break;
        }

        this?.OnStatusChanged(Status.Running);
    }

    ICoroutineRunnerService _coroutineRunner;
    Coroutine _locationServiceCoroutine;
}