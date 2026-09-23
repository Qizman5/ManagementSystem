using System;
using Microsoft.Maui.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = new ItemsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ItemsViewModel vm)
            {
                if (vm.LoadItemsCommand != null && vm.LoadItemsCommand.CanExecute(null))
                {
                    await vm.LoadItemsCommand.ExecuteAsync(null);
                }
            }
        }

        // Обробник натискання кнопки "Вийти" у XAML
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Підтвердження", "Ви дійсно бажаєте вийти з акаунту?", "Так", "Ні");
            
            if (confirm)
            {
                SecureStorage.Default.Remove("jwt_token");

                if (Shell.Current is AppShell appShell)
                {
                    appShell.SetAdminAccess(false);
                }

                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        // Обробник натискання кнопки "Створити нову операцію" у XAML
        public async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.GoToAsync("//CreateOperationPage");
                }
                else
                {
                    await Navigation.PushAsync(new CreateOperationPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Navigation Error]: {ex.Message}");
                await Navigation.PushAsync(new CreateOperationPage());
            }
        }
    }
}
