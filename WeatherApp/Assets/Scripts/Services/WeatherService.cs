using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public interface IWeatherService : IService, IInitializable
{
    Action<WeatherService.Status> OnStatusChanged { get; set; }
    CurrentWeather LastWeatherReport { get; }
    void GetWeatherDataForLocation(LocationInfo locationData);

}

[System.Serializable]
public struct CurrentWeather
{
    public float temperature;
    public float windspeed;
    public int winddirection;
    public int weathercode;
    public int is_day;
    public DateTime time;

    public string weatherConditionPlainText;
}

[System.Serializable]
public struct WeatherReport
{
    public float latitude;
    public float longitude;
    public CurrentWeather current_weather;
}

public class WeatherService : IWeatherService
{
    public enum Status
    {
        NoRequest,
        RequestInProgress,
        RequestError,
        RequestSuccess,
    }

    public Action<Status> OnStatusChanged { get; set; }

    public CurrentWeather LastWeatherReport { get; private set; }

    public void Init()
    {
        _coroutineRunner = ServiceLocator.Instance.GetService<ICoroutineRunnerService>();
    }

    public void GetWeatherDataForLocation(LocationInfo locationData)
    {
        _coroutineRunner.StartCoroutine(GetWeatherData(locationData));
    }

    IEnumerator GetWeatherData(LocationInfo locationData)
    {
        this?.OnStatusChanged(Status.RequestInProgress);

        var uri = new Uri($"https://api.open-meteo.com/v1/forecast?latitude={locationData.latitude}&longitude={locationData.longitude}&timezone=CET&daily=temperature_2m_max&current_weather=true");
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            
            this?.OnStatusChanged(Status.RequestError);
        }
        else
        {
            Debug.Log("Success getting weather report!");

            WeatherReport report = JsonUtility.FromJson<WeatherReport>(www.downloadHandler.text);
            var currentWeather = report.current_weather;
            currentWeather.weatherConditionPlainText = WeatherCodeToDescription(currentWeather.weathercode);
            LastWeatherReport = currentWeather;
            this?.OnStatusChanged(Status.RequestSuccess);
        }
    }

    static string WeatherCodeToDescription(int weatherCode)
    {
        switch (weatherCode)
        {
            case 0: return "Clear sky";
            case 1: return "Mainly clear sky";
            case 2: return "Partly cloudy sky";
            case 3: return "Overcast";
            case 45: return "Fog";
            case 48: return "Depositing rime fog";
            case 51: return "Light drizzle";
            case 53: return "Moderate drizzle";
            case 55: return "Dense drizzle";
            case 56: return "Freezing drizzle";
            case 57: return "Dense freezing drizzle";
            case 61: return "Slight rain";
            case 63: return "Moderate rain";
            case 65: return "Heavy rain";
            case 66: return "Light freezing rain";
            case 67: return "Heavy freezing rain";
            case 71: return "Slight snow fall";
            case 73: return "Moderate snow fall";
            case 75: return "Heavy snow fall";
            case 77: return "Snow grains";
            case 80: return "Slight rain shower";
            case 81: return "Moderate rain shower";
            case 82: return "Violent rain shower";
            case 85: return "Slight snow shower";
            case 86: return "Heavy snow shower";
            case 95: return "Thunderstorm!";
            case 96: return "Thunderstorm with slight hail";
            case 99: return "Thunderstorm with heavy hail";
            default: return "";
        }
    }

    ICoroutineRunnerService _coroutineRunner;
}