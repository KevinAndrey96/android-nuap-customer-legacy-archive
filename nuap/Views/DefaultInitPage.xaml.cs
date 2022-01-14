using Xamarin.Forms;

namespace nuap.Views
{
    public partial class DefaultInitPage : ContentPage
    {
        public DefaultInitPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
