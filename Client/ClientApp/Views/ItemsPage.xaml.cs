using System;
using Microsoft.Maui.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            _viewModel = new ItemsViewModel();
            BindingContext = _viewModel;
        }

        public ItemsPage(ItemsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is ItemsViewModel vm)
            {
                await vm.LoadItemsAsync();
            }
        }

        private async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            // Навігація через стек NavigationPage
            await Navigation.PushAsync(new ActionPage());
        }
    }
}
