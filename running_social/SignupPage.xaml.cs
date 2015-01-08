using running_social.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class SignupPage : Page
    {
        private NavigationHelper _navigationHelper;
        private ObservableDictionary _defaultViewModel = new ObservableDictionary();

        public SignupPage()
        {
            this.InitializeComponent();

            this._navigationHelper = new NavigationHelper(this);
            this._navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this._navigationHelper.SaveState += this.NavigationHelper_SaveState;
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

        /// <summary>
        /// Checks the status of the signup fields with VerifyFields() and,
        /// if verified, shows (or hides) the signup button.
        /// </summary>
        /// <param name="sender">One of UsernameBox, PasswordBox, EmailBox</param>
        /// <param name="e">action event args</param>
        private void TextChanged_VerifyFields(object sender, TextChangedEventArgs e)
        {
            if (VerifyFields(sender))
            {
                RegisterButton.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Copies text between PasswordBox and PasswordBox2, then calls
        /// TextChanged_VerifyFields().
        /// </summary>
        /// <param name="sender">Either PasswordBox or PasswordBox2</param>
        /// <param name="e">action event args</param>
        private void PasswordChanged_VerifyFields(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Equals(sender))
            {
                PasswordBox2.Text = PasswordBox.Password;
            } else {
                PasswordBox.Password = PasswordBox2.Text;
            }
            TextChanged_VerifyFields(PasswordBox, null);
        }

        /// <summary>
        /// Verifies that all of the fields on the signup page are correctly filled out.
        /// The name and password must not be empty, and the email must match the pattern:
        ///     .+@.+\..+ (I realize there are better email filters but don't care because
        ///     email is too complicated to easily account for everything, and nobody
        ///     seems to agree on what is valid).
        /// </summary>
        /// <param name="sender">One of UsernameBox, PasswordBox, EmailBox</param>
        /// <returns>true if everything is valid to send to server, false otherwise</returns>
        private bool VerifyFields(object sender)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            string email = EmailBox.Text;
            Regex emailRegex = null;

            ErrorBlock.Visibility = Visibility.Collapsed;
            if (username.Length == 0 && sender.Equals(UsernameBox))
            {
                ErrorBlock.Text = "error: username must not be empty";
                ErrorBlock.Visibility = Visibility.Visible;
                return false;
            }
            if (password.Length == 0 && sender.Equals(PasswordBox))
            {
                ErrorBlock.Text = "error: password must not be empty";
                ErrorBlock.Visibility = Visibility.Visible;
                return false;
            }
            if (email.Length == 0 && sender.Equals(EmailBox))
            {
                ErrorBlock.Text = "error: email must not be empty";
                ErrorBlock.Visibility = Visibility.Visible;
                return false;
            }
            emailRegex = new Regex(".+@.+\\..+");
            if (username.Length == 0 ||
                password.Length == 0 ||
                !emailRegex.IsMatch(email))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Monitors the checkbox that enables or disables the plain text password.
        /// When checked, the TextBox is visible. When not checked, the PasswordBox
        /// is visible.
        /// </summary>
        /// <param name="sender">The CheckBox</param>
        /// <param name="e">action event args</param>
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox) sender;
            if (cb.IsChecked == true)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordBox2.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordBox2.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }
    }
}
