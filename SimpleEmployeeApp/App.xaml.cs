namespace SimpleEmployeeApp;

public partial class App : Application
{
    [Obsolete]
    public App()
    {
        // Remove unnecessary assignment and call to missing InitializeComponent
        MainPage = new NavigationPage(new Views.MainPage());
    }
}