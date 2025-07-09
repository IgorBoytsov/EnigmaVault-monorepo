﻿using EnigmaVault.WPF.Client.ViewModels.Abstractions;
using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace EnigmaVault.WPF.Client.Resources.Behaviors
{
    public class AsyncInitializeBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnLoaded;
            base.OnDetaching();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.DataContext is IAsyncInitializable viewModel)
            {
                await viewModel.InitializeAsync();
            }
        }
    }
}