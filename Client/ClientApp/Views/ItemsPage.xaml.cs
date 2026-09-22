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

        private async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            try
            {
                // Надійний перехід на сторінку створення операції
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
                // Якщо маршрут Shell некоректний — відкриваємо через стандартну навігацію
                await Navigation.PushAsync(new CreateOperationPage());
            }
        }
    }
}
