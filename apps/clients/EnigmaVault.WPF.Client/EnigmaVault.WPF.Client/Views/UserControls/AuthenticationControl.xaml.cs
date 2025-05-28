using System.Windows;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationControl.xaml
    /// </summary>
    public partial class AuthenticationControl : UserControl
    {

        #region LoginProperty

        // Login
        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register(
                nameof(Login),
                typeof(string),
                typeof(AuthenticationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Login
        {
            get => (string)GetValue(LoginProperty);
            set => SetValue(LoginProperty, value);
        }

        #endregion

        #region PasswordProperty

        // Password
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password),
                typeof(string),
                typeof(AuthenticationControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        #endregion

        public AuthenticationControl()
        {
            InitializeComponent();
        }
    }
}
