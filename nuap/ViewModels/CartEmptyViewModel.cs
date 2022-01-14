namespace nuap.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Views;

    public class CartEmptyViewModel: BaseViewModel
    {
        public ICommand InitPageCommand
        {
            get
            {
                return new RelayCommand(InitPage);
            }
        }

        public ICommand ReturnCommand
        {
            get
            {
                return new RelayCommand(Return);
            }
        }

        public CartEmptyViewModel()
        {
        }

        private async void InitPage()
        {
            var mainViewModel = MainViewModel.GetInstance();
            if (mainViewModel.User.id == 93)
            {
                mainViewModel.HomeDefault = new HomeDefaultViewModel();
                await Application.Current.MainPage.Navigation.PopAsync();
                await Application.Current.MainPage.Navigation.PushAsync(new DefaultInitPage());
            }
            else
            {
                mainViewModel.Home = new HomeViewModel();
                await Application.Current.MainPage.Navigation.PopAsync();
                await Application.Current.MainPage.Navigation.PushAsync(new HomeTabbedPage());
            }
        }

        private async void Return()
        {
            var mainViewModel = MainViewModel.GetInstance();
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}