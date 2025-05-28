using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;

namespace EnigmaVault.WPF.Client.ViewModels.Pages
{
    internal class ProfilePageVM : BasePageViewModel, IUpdatable
    {
        private readonly IAuthorizationService _authorizationService;

        public ProfilePageVM(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;

            CurrentUser = _authorizationService.CurrentUser;
        }

        void IUpdatable.Update<TData>(TData value, TransmittingParameter parameter)
        {
            
        }
    }
}