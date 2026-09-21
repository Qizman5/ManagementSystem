using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class CreateOperationPage : ContentPage
    {
        public CreateOperationPage()
        {
            InitializeComponent();
            BindingContext = new ActionViewModel();
        }

        private void OnNumericEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && !string.IsNullOrEmpty(e.NewTextValue))
            {
                // Забороняємо введення мінуса та будь-яких нецифрових символів
                string cleanText = new string(e.NewTextValue.Where(char.IsDigit).ToArray());

                if (entry.Text != cleanText)
                {
                    entry.Text = cleanText;
                }
            }
        }

        private async void OnSaveOperationClicked(object sender, EventArgs e)
        {
            if (BindingContext is ActionViewModel vm)
            {
                if (vm.ProductId <= 0)
                {
                    vm.Message = "ID товару має бути додатним числом!";
                    return;
                }

                if (vm.CreateActionCommand.CanExecute(null))
                {
                    await vm.CreateActionCommand.ExecuteAsync(null);
                }
            }
        }
    }
}
