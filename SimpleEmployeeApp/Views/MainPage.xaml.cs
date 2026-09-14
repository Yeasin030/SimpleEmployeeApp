namespace SimpleEmployeeApp.Views;

public partial class MainPage : ContentPage
{
    private bool _hasAnimated;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_hasAnimated)
            return;

        _hasAnimated = true;
        Opacity = 0;
        TranslationY = 12;
        await Task.WhenAll(this.FadeTo(1, 350, Easing.CubicOut), this.TranslateTo(0, 0, 350, Easing.CubicOut));
    }

    private async void OnEmployeeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        var editPanel = this.FindByName<Border>("EditPanel");
        if (editPanel is null)
            return;

        editPanel.Opacity = 0;
        editPanel.TranslationY = 8;
        await Task.WhenAll(editPanel.FadeTo(1, 220, Easing.CubicOut), editPanel.TranslateTo(0, 0, 220, Easing.CubicOut));
    }
}