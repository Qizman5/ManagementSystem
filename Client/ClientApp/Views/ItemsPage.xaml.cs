using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
            // Передаємо HttpClient для усунення помилки CS7036
            BindingContext = new ItemsViewModel(new HttpClient());
        }

        private async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreateOperationPage");
        }
    }
}