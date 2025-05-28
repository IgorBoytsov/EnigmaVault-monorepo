using EnigmaVault.Application.Dtos;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RegistrationControl.xaml
    /// </summary>
    public partial class RegistrationControl : UserControl
    {

        #region TitleProperty

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(RegistrationControl),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region ContentTextProperty

        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register(
                nameof(ContentText),
                typeof(string),
                typeof(RegistrationControl),
                new PropertyMetadata(string.Empty));

        public string ContentText
        {
            get => (string)GetValue(ContentTextProperty);
            set => SetValue(ContentTextProperty, value);
        }

        #endregion

        #region LoginProperty

        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register(
                nameof(Login),
                typeof(string),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Login
        {
            get => (string)GetValue(LoginProperty);
            set => SetValue(LoginProperty, value);
        }

        #endregion

        #region PasswordProperty

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        #endregion

        #region EmailProperty

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register(
                nameof(Email),
                typeof(string),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Email
        {
            get => (string)GetValue(EmailProperty);
            set => SetValue(EmailProperty, value);
        }

        #endregion
        
        #region Genders

        public static readonly DependencyProperty GendersProperty =
            DependencyProperty.Register(
                nameof(Genders),
                typeof(ObservableCollection<GenderDto>),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(new ObservableCollection<GenderDto>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<GenderDto> Genders
        {
            get => (ObservableCollection<GenderDto>)GetValue(GendersProperty);
            set => SetValue(GendersProperty, value);
        }

        #endregion

        #region SelectedGender

        public static readonly DependencyProperty SelectedGenderProperty =
            DependencyProperty.Register(
                nameof(SelectedGender),
                typeof(GenderDto),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(new GenderDto(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public GenderDto SelectedGender
        {
            get => (GenderDto)GetValue(SelectedGenderProperty);
            set => SetValue(SelectedGenderProperty, value);
        }

        #endregion

        #region Countries

        public static readonly DependencyProperty CountriesProperty =
            DependencyProperty.Register(
                nameof(Countries),
                typeof(ObservableCollection<CountryDto>),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(new ObservableCollection<CountryDto>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<CountryDto> Countries
        {
            get => (ObservableCollection<CountryDto>)GetValue(CountriesProperty);
            set => SetValue(CountriesProperty, value);
        }

        #endregion

        #region SelectedCountry

        public static readonly DependencyProperty SelectedCountryProperty =
            DependencyProperty.Register(
                nameof(SelectedCountry),
                typeof(CountryDto),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(new CountryDto(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public CountryDto SelectedCountry
        {
            get => (CountryDto)GetValue(SelectedCountryProperty);
            set => SetValue(SelectedCountryProperty, value);
        }

        #endregion

        #region CodeVerificationProperty

        public static readonly DependencyProperty CodeVerificationProperty =
            DependencyProperty.Register(
                nameof(CodeVerification),
                typeof(string),
                typeof(RegistrationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string CodeVerification
        {
            get => (string)GetValue(CodeVerificationProperty);
            set => SetValue(CodeVerificationProperty, value);
        }

        #endregion

        public RegistrationControl()
        {
            InitializeComponent();
        }
    }
}
