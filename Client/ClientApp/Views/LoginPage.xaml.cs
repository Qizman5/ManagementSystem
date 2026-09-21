using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Якщо команда з ViewModel не викликалася автоматично, викликаємо її вручну
            if (BindingContext is LoginViewModel vm)
            {
                if (vm.LoginCommand.CanExecute(null))
                {
                    await vm.LoginCommand.ExecuteAsync(null);
                }
            }
        }
    }
}
