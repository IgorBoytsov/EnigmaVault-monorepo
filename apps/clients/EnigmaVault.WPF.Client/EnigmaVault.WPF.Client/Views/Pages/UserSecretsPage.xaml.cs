using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EnigmaVault.WPF.Client.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserSecretsPage.xaml
    /// </summary>
    public partial class UserSecretsPage : Page
    {
        public UserSecretsPage()
        {
            InitializeComponent();

            Loaded += YourControlOrWindow_Loaded;
        }

        //TODO: Перенести код в Behavior
        #region Управление Popop с отображением архивированных записей 

        private void YourControlOrWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (OpenArchiveManagementButton != null)
            {
                OpenArchiveManagementButton.Checked += OpenArchiveManagementButton_Checked;
                OpenArchiveManagementButton.Unchecked += OpenArchiveManagementButton_Unchecked;
                UpdatePopupPosition();
            }
        }

        private void OpenArchiveManagementButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePopupPosition();
            ArchiveManagementPopup.IsOpen = true;
        }

        private void OpenArchiveManagementButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ArchiveManagementPopup.IsOpen = false;
        }

        private void UpdatePopupPosition()
        {
            if (ArchiveManagementPopup != null && OpenArchiveManagementButton != null)
            {
                //double screenHeight = SystemParameters.PrimaryScreenHeight;
                //double buttonBottom = OpenArchiveManagementButton.PointToScreen(new Point(0, OpenArchiveManagementButton.ActualHeight)).Y;
                //ArchiveManagementPopup.MaxHeight = Math.Min(400, screenHeight - buttonBottom - 10);

                ArchiveManagementPopup.Height = ArchivedSecretsListView.Height + 100;
                ArchiveManagementPopup.MaxHeight = this.ActualHeight - 150;
            }
        }

        private CustomPopupPlacement[] PlacePopup(Size popupSize, Size targetSize, Point offset)
        {
            if (OpenArchiveManagementButton != null)
            {
                var target = OpenArchiveManagementButton;
                var buttonPosition = target.PointToScreen(new Point(0, 0));
                target.PointFromScreen(buttonPosition);

                // Для открытия справа от правого нижнего угла кнопки и направления вверх

                double xOffset = target.ActualWidth; // Смещение вправо на ширину кнопки
                double yOffset = 0;                  // Начинать с нижней границы кнопки
                return new[] 
                { 
                    //new CustomPopupPlacement(new Point(xOffset, yOffset - popupSize.Height), PopupPrimaryAxis.Vertical)
                    new CustomPopupPlacement(new Point(xOffset + 10, (yOffset - popupSize.Height) + 25), PopupPrimaryAxis.Vertical) 
                };
            }
            return new[] 
            {
                new CustomPopupPlacement(new Point(0, 0), PopupPrimaryAxis.Vertical) 
            };
        }

        #endregion
    }
}
