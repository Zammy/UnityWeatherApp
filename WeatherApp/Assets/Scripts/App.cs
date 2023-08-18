using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TextMeshProUGUI StatusText;
    [SerializeField] Button GetWeatherReportButton;

    void Start()
    {
        GetWeatherReportButton.interactable = false;
        GetWeatherReportButton.onClick.AddListener(OnWeatherReportButtonClicked);

        StatusText.text = "Not Initialized";
        StartCoroutine(InitLocationService());
    }

    void OnDestroy()
    {
        GetWeatherReportButton.onClick.RemoveAllListeners();
        Input.location.Stop();
    }

    IEnumerator InitLocationService()
    {
        Input.location.Start();

        if (!Input.location.isEnabledByUser)
        {
            StatusText.text = "Location Service not enabled";
            yield break;
        }

        var waitForASecond = new WaitForSeconds(1f);
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            StatusText.text = "Location Service: Initializing";
            yield return waitForASecond;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            StatusText.text = "Location Service: Failed";
            yield break;
        }

        StatusText.text = "Location Service: Getting location";

        GetWeatherReportButton.interactable = true;
    }

    void OnWeatherReportButtonClicked()
    {
        StatusText.text = "Retriving weather info";
        GetWeatherReportButton.interactable = false;

        var locationData = Input.location.lastData;
        Debug.Log($"Get weahter report for longitude {locationData.longitude} , latitude {locationData.latitude}");

        StartCoroutine(GetWeatherData(locationData));
    }

    IEnumerator GetWeatherData(LocationInfo locationData)
    {
        var uri = new Uri($"https://api.open-meteo.com/v1/forecast?latitude={locationData.latitude}&longitude={locationData.longitude}&timezone=CET&daily=temperature_2m_max&current_weather=true");
        UnityWebRequest www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            StatusText.text = "Error trying to get weather data!";
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("Success getting weather report!");

            WeatherReport report = JsonUtility.FromJson<WeatherReport>(www.downloadHandler.text);
            var currentWeather = report.current_weather;
            Toastylib.Toasty.DisplayToastMessage($"{WeatherCodeToDescription(currentWeather.weathercode)} Temp: {currentWeather.temperature} °C Wind:{currentWeather.windspeed} kmh");
        }

        GetWeatherReportButton.interactable = true;
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
    }

    [System.Serializable]
    public struct WeatherReport
    {
        public float latitude;
        public float longitude;
        public CurrentWeather current_weather;
        //    {
        //  "latitude": 19.125,
        //  "longitude": 72.875,
        //  "generationtime_ms": 0.4099607467651367,
        //  "utc_offset_seconds": 19800,
        //  "timezone": "Asia/Calcutta",
        //  "timezone_abbreviation": "IST",
        //  "elevation": 6,
        //  "current_weather": {
        //    "temperature": 27.8,
        //    "windspeed": 9.4,
        //    "winddirection": 277,
        //    "weathercode": 3,
        //    "is_day": 0,
        //    "time": "2023-08-18T19:30"
        //  },
        //  "daily_units": {
        //    "time": "iso8601",
        //    "temperature_2m_max": "�C"
        //  },
        //  "daily": {
        //    "time": [
        //      "2023-08-18",
        //      "2023-08-19",
        //      "2023-08-20",
        //      "2023-08-21",
        //      "2023-08-22",
        //      "2023-08-23",
        //      "2023-08-24"
        //    ],
        //    "temperature_2m_max": [
        //      30.4,
        //      28.3,
        //      29.2,
        //      27.5,
        //      28.2,
        //      29.4,
        //      29.3
        //    ]
        //    }
        //}
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
}
