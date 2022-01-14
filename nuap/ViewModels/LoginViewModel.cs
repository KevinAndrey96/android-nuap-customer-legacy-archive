namespace nuap.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using System.Windows.Input;
    using System.Linq;
    using Xamarin.Forms;
    using Views;
    using Services;
    using Helpers;
    using nuap.Models;
    using System;
    using Newtonsoft.Json;

    public class LoginViewModel : BaseViewModel
    {
        private ApiService apiService;

        private string email;
        private string password;
        private bool isRunning;
        private bool isEnabled;

        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }

        public LoginViewModel()
        {
            this.checkLogin();
        }

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                return new RelayCommand(Register);
            }
        }

        public ICommand SelectAddressCommand
        {
            get
            {
                return new RelayCommand(SelectAddress);
            }
        }

        private void checkLogin()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
        }

        private async void Register()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Register = new RegisterViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        private async void SelectAddress()
        {
            User Usuario = new User();
            Usuario.api_token = "***REMOVED***";
            Usuario.role = "Usuario";
            Usuario.phone = "3333333333";
            Usuario.email = "default-user@nuap.com";
            Usuario.balance = "$ 0";
            Usuario.id = 93;

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.User = Usuario;

            if (Settings.AddressDefaultDetail != "")
            {
                Address AddressData = JsonConvert.DeserializeObject<Address>(Settings.AddressDefaultDetail);
                mainViewModel.AddressDetail = AddressData;

                mainViewModel.HomeDefault = new HomeDefaultViewModel();
                await Application.Current.MainPage.Navigation.PushAsync(new DefaultInitPage());
            }
            else
            {
                mainViewModel.CreateAddress = new CreateAddressViewModel();
                await Application.Current.MainPage.Navigation.PushAsync(new CreateAddressPage());
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Debes ingresar tu correo electrónico o celular",
                        "Aceptar"
                    );
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "Debes ingresar tu contraseña",
                        "Aceptar"
                    );
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                this.Password = string.Empty;

                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Aceptar");
                return;
            }

            var response = await this.apiService.GetUser(
                "https://thenuap.com",
                this.Email,
                this.Password,
                "usuario");

            if (response == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                this.Password = string.Empty;

                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ha ocurrido un error",
                    "Aceptar");
                return;
            }

            if (response.Data == null)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                this.Password = string.Empty;

                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Errors.First().Detail,
                    "Aceptar");
                return;
            }
            
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.User = response.Data;
            
            this.IsEnabled = true;

            if (response.Data.phone_validated == 1)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
                mainViewModel.GenerateOTP = new GenerateOtpViewModel();
                await Application.Current.MainPage.Navigation.PushAsync(new GenerateOtpPage());
            } else
            {
                Settings.Token = response.Data.api_token;
                Settings.UserType = response.Data.role;
                Settings.Phone = response.Data.phone;
                Settings.Email = response.Data.email;
                Settings.Id = response.Data.id.ToString();

                await Application.Current.MainPage.Navigation.PopAsync();
                mainViewModel.Home = new HomeViewModel();
                await Application.Current.MainPage.Navigation.PushAsync(new HomeTabbedPage());
            }
        }
    }
}
