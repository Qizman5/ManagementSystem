using ClientApp; // Підключаємо основний namespace, де знаходиться ActionViewModel
using ClientApp.ViewModels; // На випадок, якщо інші моделі знаходяться тут

namespace ClientApp.Views
{
    public partial class CreateOperationPage : ContentPage
    {
        public CreateOperationPage()
        {
            InitializeComponent();
            BindingContext = new ActionViewModel();
        }

        private async void OnSaveOperationClicked(object sender, EventArgs e)
        {
            if (BindingContext is ActionViewModel vm)
            {
                // 1. Викликаємо асинхронну команду збереження/створення
                if (vm.CreateActionCommand.CanExecute(null))
                {
                    await vm.CreateActionCommand.ExecuteAsync(null);
                }

                // 2. Гарантований перехід назад на сторінку товарів
                await Shell.Current.GoToAsync("//ItemsPage");
            }
        }
    }
}
