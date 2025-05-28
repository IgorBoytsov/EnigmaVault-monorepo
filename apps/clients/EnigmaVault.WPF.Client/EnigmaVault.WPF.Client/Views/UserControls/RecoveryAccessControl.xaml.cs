using System.Windows;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для RecoveryAccessControl.xaml
    /// </summary>
    public partial class RecoveryAccessControl : UserControl
    {

        #region LoginProperty

        public static readonly DependencyProperty LoginProperty =
            DependencyProperty.Register(
                nameof(Login),
                typeof(string),
                typeof(RecoveryAccessControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Login
        {
            get => (string)GetValue(LoginProperty);
            set => SetValue(LoginProperty, value);
        }

        #endregion

        #region EmailProperty

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register(
                nameof(Email),
                typeof(string),
                typeof(RecoveryAccessControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Email
        {
            get => (string)GetValue(EmailProperty);
            set => SetValue(EmailProperty, value);
        }

        #endregion

        #region RecoveryCodeProperty

        public static readonly DependencyProperty RecoveryCodeProperty =
            DependencyProperty.Register(
                nameof(RecoveryCode),
                typeof(string),
                typeof(RecoveryAccessControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string RecoveryCode
        {
            get => (string)GetValue(RecoveryCodeProperty);
            set => SetValue(RecoveryCodeProperty, value);
        }

        #endregion

        #region NewPasswordProperty

        public static readonly DependencyProperty NewPasswordProperty =
            DependencyProperty.Register(
                nameof(NewPassword),
                typeof(string),
                typeof(RecoveryAccessControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string NewPassword
        {
            get => (string)GetValue(NewPasswordProperty);
            set => SetValue(NewPasswordProperty, value);
        }

        #endregion

        public RecoveryAccessControl()
        {
            InitializeComponent();
        }
    }
}
