using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Udhaar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
     
    public sealed partial class MainPage : Page
    {
        private String PERSONAL_NAME;
        private String PERSONAL_PHONE;
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
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
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
       
        
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            try 
            {
                
                IMobileServiceTable<Credit> creditTable = App.MobileService.GetTable<Credit>();
                MobileServiceCollection<Credit, Credit> creditRecords;
                p.IsActive = true;
                creditRecords = await creditTable.ToCollectionAsync<Credit>();
                p.IsActive=false;
                double c_amt = 0;
                double d_amt = 0;
                double t_amt = 0;
                Credit tmp_credit = new Credit();
                foreach (var creditRecord in creditRecords)
                {
                    if (creditRecord.Personal_name == PERSONAL_NAME && creditRecord.Personal_number == PERSONAL_PHONE && creditRecord.Amount>=0 && creditRecord.Valid == true){
                        c_amt += creditRecord.Amount;
                    }
                    else if (creditRecord.Personal_name == PERSONAL_NAME && creditRecord.Personal_number == PERSONAL_PHONE && creditRecord.Amount < 0 && creditRecord.Valid == true)
                    {
                        d_amt += creditRecord.Amount;
                    }
                    else if (creditRecord.Name == PERSONAL_NAME && creditRecord.Phone_no == PERSONAL_PHONE && creditRecord.Amount >= 0 && creditRecord.Valid == true)
                    {
                        d_amt -= creditRecord.Amount;
                    }
                    else if (creditRecord.Name == PERSONAL_NAME && creditRecord.Phone_no == PERSONAL_PHONE && creditRecord.Amount < 0 && creditRecord.Valid == true)
                    {
                        c_amt -= creditRecord.Amount;
                    }
                }
                t_amt = d_amt + c_amt;
                totalAmt.Text = "Total Balance : "+t_amt.ToString();
                totalCredit.Text = "Total Credit : "+(Math.Abs(c_amt)).ToString();
                totalDebit.Text = "Total Debit : "+(Math.Abs(d_amt)).ToString();

            }
            catch
            {
                MessageDialog md = new MessageDialog("Network can't connect. Check Internet Connection");
                md.ShowAsync();
            }
        }
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

        private void debitShow_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(debitList));

        }

        private void creditShow_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(creditList));
        }
       
    }
}
