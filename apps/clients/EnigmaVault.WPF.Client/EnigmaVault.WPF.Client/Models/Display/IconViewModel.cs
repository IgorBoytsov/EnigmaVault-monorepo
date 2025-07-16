using EnigmaVault.Application.Dtos;
using System.Windows.Media;

namespace EnigmaVault.WPF.Client.Models.Display
{
    internal class IconViewModel : ObservableObject
    {
        private IconDto _model;

        public IconViewModel(IconDto model)
        {
            _model = model;
        }

        public int IconId => _model.IdIcon;
        public int? IdUser => _model.IdUser;
        public string SvgCode => _model.SvgCode;
        public int IdIconCategory => _model.IdIconCategory;
        public bool IsCommon => _model.IsCommon;

        public string IconName
        {
            get => _model.IconName;
            private set
            {
                if (_model.IconName != value)
                {
                    _model = _model with { IconName = value };
                    OnPropertyChanged(nameof(IconName));
                }
            }
        }

        private string? _categoryName;
        public string? CategoryName
        {
            get => _categoryName;
            private set
            {
                SetProperty(ref _categoryName, value);
            }
        }
 
        private ImageSource? _svgIcon;
        public ImageSource? SvgIcon
        {
            get => _svgIcon;
            private set
            {
                SetProperty(ref _svgIcon, value);
            }
        }

        public void SetIcon(ImageSource svgCode) => SvgIcon = svgCode;

        public void SetCategoryName(string categoryName) => CategoryName = categoryName;

        public IconDto GetUnderlyingModel() => _model;
    }
}