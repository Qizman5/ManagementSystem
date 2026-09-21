using System;
using Microsoft.Maui.Controls;
using ClientApp.ViewModels;

namespace ClientApp.Views
{
    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel _viewModel;

        // Конструктор без параметрів
        public ItemsPage()
        {
            InitializeComponent();
            _viewModel = new ItemsViewModel();
            BindingContext = _viewModel;
        }

        // Конструктор для Dependency Injection
        public ItemsPage(ItemsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Автоматично завантажуємо та оновлюємо список товарів при відкритті сторінки
            if (BindingContext is ItemsViewModel vm)
            {
                await vm.LoadItemsAsync();
            }
        }

        private async void OnCreateOperationClicked(object sender, EventArgs e)
        {
            // Перехід на сторінку створення товару/операції
            await Shell.Current.GoToAsync(nameof(ActionPage));
        }
    }
}
