public class AppBootstrapper : Bootstrapper
{
    protected override void Awake()
    {
        base.Awake();

        var serviceLocator = ServiceLocator.Instance;
        serviceLocator.RegisterService(new LocationService());
    }

}
