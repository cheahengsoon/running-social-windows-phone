using running_social.Common;
using running_social.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace running_social
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private NavigationHelper _navigationHelper;
        private ObservableDictionary _defaultViewModel = new ObservableDictionary();

        public SettingsPage()
        {
            this.InitializeComponent();

            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this._navigationHelper.SaveState += this.NavigationHelper_SaveState;

            SettingsRoot.SelectionChanged += SettingsRootOnSelectionChanged;
        }

        private void SettingsRootOnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            PivotItem pivotItem = (PivotItem)args.AddedItems[0];
            String pivotItemName = pivotItem.Header.ToString();
            if (pivotItemName == "my profile")
            {
                ViewProfileButton.Visibility = Visibility.Visible;
            }
            else
            {
                ViewProfileButton.Visibility = Visibility.Collapsed;                
            }
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this._navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this._defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            ScreenNameEdit.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this._navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AppBarButton_Click_Home(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void AppBarButton_View_Profile(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserProfile));
        }

        private void Button_Click_Edit_Screen_Name(object sender, RoutedEventArgs e)
        {
            ScreenNameView.Visibility = Visibility.Collapsed;
            ScreenNameEdit.Visibility = Visibility.Visible;
            if (!UIHelper.ScrollToElement(ProfileScrollViewer, ScreenNameEdit))
            {
                UIHelper.ScrollToElement(ProfileScrollViewer, ScreenNameView);
            }
        }

        private void Button_Click_Save_Screen_Name(object sender, RoutedEventArgs e)
        {
            ScreenNameView.Visibility = Visibility.Visible;
            ScreenNameEdit.Visibility = Visibility.Collapsed;
            // TODO: save screen name to profile
            UIHelper.ScrollToElement(ProfileScrollViewer, ScreenNameView);
        }

        private void Button_Click_Cancel_Screen_Name(object sender, RoutedEventArgs e)
        {
            ScreenNameView.Visibility = Visibility.Visible;
            ScreenNameEdit.Visibility = Visibility.Collapsed;
            UIHelper.ScrollToElement(ProfileScrollViewer, ScreenNameEdit);
        }
    }
}
