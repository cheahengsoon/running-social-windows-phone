using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace running_social.Resources
{
    class UserAccount
    {
        private String _username;
        private String _screenName;
        private String _email;
        private String _password;
        /// <summary>
        /// Used to encrypt and decrypt data to communicate with the server
        /// </summary>
        private String _serverEncryptionKey;
        /// <summary>
        /// The primary account is the one registered for this app for this device.
        /// There is only one primary account per app per device.
        /// </summary>
        private Boolean _isPrimary;
        private List<Run> _runs;
        /// <summary>
        /// The primary account must be authorized before using the app.
        /// </summary>
        private Boolean _isAuthorized;
        private ImageStream _picture;
        private List<UserAccount> _friends;

        private static UserAccount _primaryUserAccount = null;

        public UserAccount(String username)
        {
            Init(username, null, null);
        }

        public UserAccount(String username, String email, String password)
        {
            Init(username, email, password);
        }

        public void Init(String username, String email, String password)
        {
            if (email != null && password != null)
            {
                _isPrimary = true;
                _email = email;
                _password = password;
            }
            else
            {
                _isPrimary = false;
                _email = null;
                _password = null;
            }

            _username = username;
            _screenName = null;
            _runs = null;
            _isAuthorized = false;
            _picture = null;
            _serverEncryptionKey = null;
        }

        public static UserAccount GetPrimaryAccount()
        {
            if (_primaryUserAccount == null)
            {
                // TODO: load the primary account from the local DB
            }
            return _primaryUserAccount;
        }

        /// <summary>
        /// Syncs data with the server, including sending data to the server
        /// and pulling data from the server.
        /// Will only sync accounts from the primary account.
        /// </summary>
        /// <param name="toSync">The account to be synced.</param>
        public void SyncWithServer(UserAccount toSync)
        {
            if (!_isPrimary)
            {
                return;
            }
            // TODO: sync the account toSync with the server
        }

        public async Task<bool> Authorize(String email, String password)
        {
            if (_isAuthorized)
            {
                return true;
            }
            if (!_isPrimary)
            {
                return false;
            }

            bool authorized = false;
            // TODO: authorize with server
            if (!authorized)
            {
                return false;
            }

            // TODO: load runs from internal database
            // _runs = 
            // _statistics = 
            // _picture = 
            _password = "";
            _primaryUserAccount = this;
            return true;
        }

        public Dictionary<string, StatisticsClass> Statistics { get; set; }
        public String Username { get; set; }
        public ImageStream Picture { get; set; }
        public List<Run> Runs
        {
            get
            {
                if (!_isPrimary)
                {
                    return null;
                }
                return _runs;
            }
            set { _runs = value; }
        } 

        /// <summary>
        /// Calculates and holds statistics for UserAccounts.
        /// </summary>
        public class StatisticsClass
        {
            private int _numRuns;
            private float _avgMeters;
            private int _avgMinutes;
            private float _avgMinutesPerMile;

            public StatisticsClass(List<Run> runs)
            {
                GenerateStatistics(runs);
            }

            private void GenerateStatistics(List<Run> runs)
            {
                // TODO: generate and return statistics about the user's runs
            }

            private void Deserialize(String serializedStats)
            {
                // TODO: deserialize data
            }
        }
    }
}
