using running_social.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.Notification.Management;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using running_social.Resources;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace running_social
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CurrentRunPage : Page, INotifyPropertyChanged
    {
        private List<Button> buttons = new List<Button>();

        private enum rstatus
        {
            started,
            starting,
            paused,
            pausing,
            stopped,
            stopping
        };

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private rstatus runStatus = rstatus.stopped;
        public event PropertyChangedEventHandler PropertyChanged;
        private Visibility _visibilityOption;

        public CurrentRunPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            buttons.Add(PlayButton11);
            buttons.Add(PlayButton21);
            buttons.Add(PlayButton12);
            buttons.Add(PlayButton22);
            buttons.Add(PauseButton11);
            buttons.Add(PauseButton21);
            buttons.Add(StopButton11);
            buttons.Add(StopButton21);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        // This method is called by the Set accessor of each property.
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void DisableAll()
        {
            foreach (var button in buttons)
            {
                button.IsEnabled = false;
            }
        }

        private void ShowActive()
        {
            if (runStatus == rstatus.started)
            {
                PauseButton11.IsEnabled = true;
                PauseButton21.IsEnabled = true;
                PauseButton11.Visibility = Visibility.Visible;
                PauseButton21.Visibility = Visibility.Visible;
            }
            if (runStatus == rstatus.paused)
            {
                PlayButton12.IsEnabled = true;
                PlayButton22.IsEnabled = true;
                StopButton11.IsEnabled = true;
                StopButton21.IsEnabled = true;
                PlayStopGrid1.Visibility = Visibility.Visible;
                PlayStopGrid2.Visibility = Visibility.Visible;
            }
            if (runStatus == rstatus.stopped)
            {
                PlayButton11.IsEnabled = true;
                PlayButton21.IsEnabled = true;
                PlayButton11.Visibility = Visibility.Visible;
                PlayButton21.Visibility = Visibility.Visible;
            }
            NotifyPropertyChanged();
        }

        private void CollapseAll()
        {
            PauseButton11.Visibility = Visibility.Collapsed;
            PauseButton21.Visibility = Visibility.Collapsed;
            PlayStopGrid1.Visibility = Visibility.Collapsed;
            PlayStopGrid2.Visibility = Visibility.Collapsed;
            PlayButton11.Visibility = Visibility.Collapsed;
            PlayButton21.Visibility = Visibility.Collapsed;
            NotifyPropertyChanged();
        }

        public void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();
            runStatus = rstatus.starting;
            Start_Run();
        }

        public bool Start_Run()
        {
            if (runStatus != rstatus.started &&
                runStatus != rstatus.stopped)
            {
                return false;
            }

            Geocoordinates.GetGeolocator().SubscribeToUpdates();

            runStatus = rstatus.started;
            CollapseAll();
            ShowActive();

            return true;
        }

        public void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();
            runStatus = rstatus.pausing;
            Pause_Run();
        }

        public bool Pause_Run()
        {
            if (runStatus != rstatus.started)
            {
                return false;
            }

            Geocoordinates.GetGeolocator().StopSubscription();

            runStatus = rstatus.paused;
            CollapseAll();
            ShowActive();

            return true;
        }

        public void StopButton_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();
            runStatus = rstatus.stopping;
            Stop_Run();
        }

        public bool Stop_Run()
        {
            if (runStatus != rstatus.paused)
            {
                return false;
            }

            Geocoordinates.GetGeolocator().StopSubscription();

            runStatus = rstatus.stopped;
            CollapseAll();
            ShowActive();

            return true;
        }

        private void AppBarButton_Click_Settings(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void AppBarButton_Click_Home(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
