using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.NewAccount
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateAccountPage : Page
    {
        public CreateAccountPage()
        {
            InitializeComponent();
        }

        private void GoBackToMainPage(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            _ = Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            _ = contentFrame.Navigate(typeof(CreateBuyer));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;

            switch (item.Tag.ToString())
            {
                case "CreateBuyer":
                    _ = contentFrame.Navigate(typeof(CreateBuyer));
                    break;

                case "CreateSeller":
                    _ = contentFrame.Navigate(typeof(CreateSeller));
                    break;

            }
        }
    }
}

