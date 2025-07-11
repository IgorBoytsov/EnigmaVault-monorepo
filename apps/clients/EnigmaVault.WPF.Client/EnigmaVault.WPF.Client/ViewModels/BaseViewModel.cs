﻿using EnigmaVault.Application.Dtos;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EnigmaVault.WPF.Client.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty<bool>(ref _isBusy, value);
        }

        private bool _isInitialize = false;
        public bool IsInitialize
        {
            get => _isInitialize;
            set => SetProperty<bool>(ref _isInitialize, value);
        }

        private UserDto? _currentUser;
        public UserDto? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }
    }
}