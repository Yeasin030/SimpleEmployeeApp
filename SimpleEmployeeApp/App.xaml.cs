namespace SimpleEmployeeApp;

public partial class App : Application
{
    [Obsolete]
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new Views.MainPage());
    }
}