using Ferdin_TB_Hub.ItemDetail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Ferdin_TB_Hub.HomePage_NavigationView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        public ObservableCollection<Item> Items { get; set; }

        public class Item
        {
            public string Title { get; set; }
            public Uri Image { get; set; }
        }

        public Home()
        {
            this.InitializeComponent();
            Items = new ObservableCollection<Item>();
            // Add sample data
            Items.Add(new Item { Title = "Item 1", Image = new Uri("ms-appx:///Assets/Default_UserIcon.png") });
            Items.Add(new Item { Title = "Item 2", Image = new Uri("ms-appx:///Assets/Default_UserIcon.png") });
            Items.Add(new Item { Title = "Item 3", Image = new Uri("ms-appx:///Assets/Default_UserIcon.png") });
            Items.Add(new Item { Title = "Item 4", Image = new Uri("ms-appx:///Assets/Default_UserIcon.png") });
            Items.Add(new Item { Title = "Item 5", Image = new Uri("ms-appx:///Assets/Default_UserIcon.png") });

            // Add more items as needed
            myListView.ItemsSource = Items;
        }

        private void SourceImage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Navigate to detail page.
            // Suppress the default animation to avoid conflict with the connected animation.
            Frame.Navigate(typeof(ItemDetailPage), null, new SuppressNavigationTransitionInfo());
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView()
                .PrepareToAnimate("forwardAnimation", SourceImage);
            // You don't need to explicitly set the Configuration property because
            // the recommended Gravity configuration is default.
            // For custom animation, use:
            // animation.Configuration = new BasicConnectedAnimationConfiguration();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation animation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("backAnimation");
            if (animation != null)
            {
                animation.TryStart(SourceImage);
            }
        }
    }
}
