using Udhaar.Common;
using System;
using System.Collections.Generic;
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
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.Storage;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Udhaar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class debitList : Page
    {
        private String PERSONAL_NAME;
        private String PERSONAL_PHONE;
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public debitList()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            initFunc();
        }
        public async void initFunc()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/personalInfo.txt"));
            using (StreamReader sread = new StreamReader(await file.OpenStreamForReadAsync()))
            {

                PERSONAL_NAME = sread.ReadLine();
                PERSONAL_PHONE = sread.ReadLine();
                sread.Dispose();
            }
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
                MobileServiceCollection<Credit, Credit> creditRecords;
                p.IsActive = true;
                creditRecords = await creditTable.ToCollectionAsync<Credit>();
                Credit tmp_credit = new Credit();
                IList<SummaryWrapper> debitList = new List<SummaryWrapper>();
                p.IsActive = false;
                foreach (var creditRecord in creditRecords)
                {
                    SummaryWrapper s = new SummaryWrapper();
                    s.colour = "Red";
                    if ((creditRecord.Personal_name).Equals(PERSONAL_NAME, StringComparison.OrdinalIgnoreCase) && (creditRecord.Personal_number).Equals(PERSONAL_PHONE, StringComparison.OrdinalIgnoreCase) && creditRecord.Amount < 0 && (creditRecord.Valid == true))
                    {
                        s.Name = creditRecord.Name;
                        s.Amount = Math.Abs(creditRecord.Amount);
                        s.Number = creditRecord.Phone_no;
                        s.date = creditRecord.date.Date.ToString();
                        debitList.Add(s);
                    }
                    else if ((creditRecord.Name).Equals(PERSONAL_NAME, StringComparison.OrdinalIgnoreCase) && (creditRecord.Phone_no).Equals(PERSONAL_PHONE, StringComparison.OrdinalIgnoreCase) && creditRecord.Amount > 0 && (creditRecord.Valid == true))
                    {
                        s.Name = creditRecord.Personal_name;
                        s.Amount = Math.Abs(creditRecord.Amount);
                        s.Number = creditRecord.Personal_number;
                        s.date = creditRecord.date.Date.ToString();
                        debitList.Add(s);
                    }

                }
                sumDisp.ItemsSource = debitList;

            }
            catch
            {
                MessageDialog md = new MessageDialog("Network can't connect. Check Internet Connection");
                md.ShowAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        #endregion
    }
}
