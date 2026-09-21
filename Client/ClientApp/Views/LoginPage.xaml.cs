using ClientApp.ViewModels;

namespace ClientApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Приклад переходу після успішного входу
        await Shell.Current.GoToAsync($"//{nameof(ItemsPage)}");
    }
}