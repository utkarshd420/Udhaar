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
using Windows.ApplicationModel.Contacts;
using Windows.Phone.PersonalInformation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.Storage;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Udhaar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAmount : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private String PERSONAL_NAME;
        private String PERSONAL_PHONE;

        public  AddAmount()
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
            using(StreamReader sread = new StreamReader(await file.OpenStreamForReadAsync()))
            {

                    PERSONAL_NAME =  sread.ReadLine();
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
       

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
        }

        #endregion
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Contact c = (Contact)e.Parameter;
            userInfo.Text = c.FirstName.ToString() + " " + c.LastName.ToString();
            IList<ContactPhone> li_co = c.Phones;
            userPhone.Text = li_co[0].Number.ToString();
        }

        private async void Debit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Credit credit = new Credit();
                credit.Personal_name = PERSONAL_NAME;
                credit.Personal_number = PERSONAL_PHONE;
                credit.Valid = false;
                credit.Name = userInfo.Text;
                credit.Phone_no = userPhone.Text;
                credit.Amount = 0;
                ///Getting the entire 
                IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
                MobileServiceCollection<Credit, Credit> creditRecords;
                p.IsActive = true;
                creditRecords = await creditTable.ToCollectionAsync<Credit>();
                ///Got the data
                
                Credit tmp_credit = new Credit();
                p.IsActive = false;
                /*foreach (var creditRecord in creditRecords)
                {
                    if (creditRecord.Name == credit.Name && creditRecord.Phone_no == credit.Phone_no && creditRecord.Personal_number == PERSONAL_PHONE && creditRecord.Personal_name == PERSONAL_NAME)
                    {
                        tmp_credit = creditRecord;
                        flag = 1;
                    }
                }*/
                credit.Amount = (-1)*Convert.ToDouble(amountInfo.Text);
                p.IsActive = true;
                await App.MobileService.GetTable<Credit>().InsertAsync(credit);
                p.IsActive = false;
                /*if (flag == 1)
                {
                    tmp_credit.Amount = tmp_credit.Amount + credit.Amount;
                    await App.MobileService.GetTable<Credit>().UpdateAsync(tmp_credit);
                }
                else
                {
                    await App.MobileService.GetTable<Credit>().InsertAsync(credit);
                }*/

                MessageDialog md = new MessageDialog("Amount Debited");
                md.ShowAsync();
            }
            catch
            {
                MessageDialog md = new MessageDialog("Network can't connect. Check Internet Connection");
                md.ShowAsync();
            }
        }

        private async void Credit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                Credit credit = new Credit();
                credit.Personal_name = PERSONAL_NAME;
                credit.Personal_number = PERSONAL_PHONE;
                credit.Valid = false;
                credit.Name = userInfo.Text;
                credit.Phone_no= userPhone.Text;
                credit.Amount = 0;
                ///Getting the entire 
                IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
                MobileServiceCollection<Credit, Credit> creditRecords;
                p.IsActive = true;
                creditRecords = await creditTable.ToCollectionAsync<Credit>();
                p.IsActive = false;
                ///Got the data
                int flag = 0;
                Credit tmp_credit= new Credit();
                /*foreach(var creditRecord in creditRecords)
                {
                    if (creditRecord.Name == credit.Name && creditRecord.Phone_no == credit.Phone_no && creditRecord.Personal_number == PERSONAL_PHONE && creditRecord.Personal_name == PERSONAL_NAME)
                    {
                        tmp_credit = creditRecord;
                        flag = 1;
                    }
                }*/
                credit.Amount = Convert.ToDouble(amountInfo.Text);
                p.IsActive = true;
                await App.MobileService.GetTable<Credit>().InsertAsync(credit);
                p.IsActive = false;
                /*if (flag == 1)
                {
                    tmp_credit.Amount = tmp_credit.Amount + credit.Amount;
                    await App.MobileService.GetTable<Credit>().UpdateAsync(tmp_credit);
                }
                else
                {
                    await App.MobileService.GetTable<Credit>().InsertAsync(credit);
                }*/
                
                MessageDialog md = new MessageDialog("Amount Credited");
                md.ShowAsync();
            }
            catch(Exception ea)
            {
                MessageDialog md = new MessageDialog(ea.ToString());
                md.ShowAsync();
            }

        }
    }
}
