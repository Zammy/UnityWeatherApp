using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TextMeshProUGUI StatusText;
    [SerializeField] Button GetWeatherReportButton;

    void Start()
    {
        Debug.Log("App.Start");

        GetWeatherReportButton.interactable = false;
        GetWeatherReportButton.onClick.AddListener(OnWeatherReportButtonClicked);

        StatusText.text = "Not Initialized";

        _locationService = ServiceLocator.Instance.GetService<ILocationService>();
        _locationService.OnStatusChanged += OnLocationServiceStatusChanged;
        //we need low accuracy for weather report
        _locationService.StartLocationSeeking(accuracyInMeters: 1000);

        _weatherService = ServiceLocator.Instance.GetService<IWeatherService>();
        _weatherService.OnStatusChanged += OnWeatherServiceStatusChanged;
    }

    void OnDestroy()
    {
        _locationService.OnStatusChanged -= OnLocationServiceStatusChanged;
        _locationService.StopLocationSeeking();

        GetWeatherReportButton.onClick.RemoveAllListeners();
    }

    void OnLocationServiceStatusChanged(LocationService.Status status)
    {
        switch (status)
        {
            case LocationService.Status.NotEnabledByUser:
                {
                    StatusText.text = "Location Service not enabled";
                    break;
                }
            case LocationService.Status.Initializing:
                {
                    StatusText.text = "Location Service: Initializing";
                    break;
                }
            case LocationService.Status.Failed:
                {
                    StatusText.text = "Location Service: Failed";
                    break;
                }
            case LocationService.Status.Running:
                {
                    StatusText.text = "Location Service: Getting location";
                    GetWeatherReportButton.interactable = true;
                    break;
                }
            default:
                break;
        }
    }

    void OnWeatherServiceStatusChanged(WeatherService.Status status)
    {
        switch (status)
        {
            case WeatherService.Status.RequestInProgress:
                {
                    StatusText.text = "Retriving weather info";
                    break;
                }
            case WeatherService.Status.RequestError:
                {
                    StatusText.text = "Error trying to get weather data!";
                    GetWeatherReportButton.interactable = true;
                    break;
                }
            case WeatherService.Status.RequestSuccess:
                {
                    var currentWeather = _weatherService.LastWeatherReport;
                    Toastylib.Toasty.DisplayToastMessage($"{currentWeather.weatherConditionPlainText} Temp: {currentWeather.temperature} °C Wind:{currentWeather.windspeed} kmh");
                    GetWeatherReportButton.interactable = true;
                    break;
                }
            default:
                break;
        }
    }

    void OnWeatherReportButtonClicked()
    {
        GetWeatherReportButton.interactable = false;

        var locationData = _locationService.GetCurrentLocationInfo();
        Debug.Log($"Get weahter report for longitude {locationData.longitude} , latitude {locationData.latitude}");

        _weatherService.GetWeatherDataForLocation(locationData);
    }

    ILocationService _locationService;
    IWeatherService _weatherService;
}
