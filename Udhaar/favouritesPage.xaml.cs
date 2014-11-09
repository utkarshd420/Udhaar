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
using Windows.Phone.PersonalInformation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.Storage;
namespace Udhaar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class favouritesPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private String PERSONAL_NAME;
        private String PERSONAL_PHONE;
        public favouritesPage()
        {
            initFunc();
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            
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
            IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
            MobileServiceCollection<Credit, Credit> creditRecords;
            p.IsActive = true;
            creditRecords = await creditTable.ToCollectionAsync<Credit>();
            IList<Credit> req = new List<Credit>();
            p.IsActive = false;
            ///Got the data
            foreach (var creditRecord in creditRecords)
            {
                Boolean one = (creditRecord.Name).Equals(PERSONAL_NAME,StringComparison.OrdinalIgnoreCase);
                Boolean two = (creditRecord.Phone_no).Equals( PERSONAL_PHONE,StringComparison.OrdinalIgnoreCase);
                Boolean three = creditRecord.Valid ==false;
                Boolean four = one && two && three;
                if (four)
                {
                    /*if(creditRecord.Amount<0)
                        color=Green
                    else
                        color = Red*/
                    Credit temp = new Credit();
                    temp = creditRecord;
                    if (temp.Amount < 0)
                        temp.Amount = Math.Abs(temp.Amount);
                    else
                        temp.Amount = (-1) * temp.Amount;
                    req.Add(temp);
                }
            }
            reqDisp.ItemsSource = req;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        #endregion

        private void settingsNavigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(settingsPage));
        }

        private void favouritesNavigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(favouritesPage));
        }

        private void amountNavigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void contactsNavigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(contactsPage));
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {

            Button me =  sender as Button;
            StackPanel parentPanel = me.Parent as StackPanel;
            TextBlock tb = parentPanel.Children[0] as TextBlock;

            String id = tb.Text;
            IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
            MobileServiceCollection<Credit, Credit> creditRecords;
            p.IsActive = true;
            creditRecords = await creditTable.ToCollectionAsync<Credit>();
            IList<Credit> req = new List<Credit>();
            p.IsActive = false;
            ///Got the data
            foreach (var creditRecord in creditRecords)
            {
                p.IsActive = true;
                if((creditRecord.Id).Equals(id,StringComparison.OrdinalIgnoreCase))
                {
                    creditRecord.Valid = true;
                    await creditTable.UpdateAsync(creditRecord);
                    break;
                }
            }
            p.IsActive = false;
            Frame.Navigate(typeof(favouritesPage));
        }

        private async void Deny_Click(object sender, RoutedEventArgs e)
        {
            Button me = (Button)sender;
            String id = me.Content.ToString();
            IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
            MobileServiceCollection<Credit, Credit> creditRecords;
            p.IsActive = true;
            creditRecords = await creditTable.ToCollectionAsync<Credit>();
            p.IsActive = false;
            IList<Credit> req = new List<Credit>();

            ///Got the data
            foreach (var creditRecord in creditRecords)
            {
                p.IsActive = true;
                if ((creditRecord.Id).Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    
                    creditRecord.Valid = false;
                    await creditTable.DeleteAsync(creditRecord);
                    break;
                    
                }
            }
            p.IsActive = false;
            Frame.Navigate(typeof(favouritesPage));
        }
    }
}
