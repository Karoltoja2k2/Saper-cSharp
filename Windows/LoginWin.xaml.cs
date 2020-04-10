using Saper.ApiAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Saper.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy Window1.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        public LoginWin()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string nickName = nickNameRegiserBox.Text;
            string email = emailRegisterBox.Text;
            string password = passwordRegisterbox.Password;
            RegisterForm registerForm = new RegisterForm(nickName, email, password);

            bool valid = Verify_Register_Form(registerForm);            

            if (!valid) { return; }

            HttpResponseMessage httpResponse = await ApiRequests.Register(registerForm);
            string msgResponse;
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                msgResponse = await httpResponse.Content.ReadAsStringAsync();
                Finish_Registration(msgResponse);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                msgResponse = await httpResponse.Content.ReadAsStringAsync();
                Process_Error(msgResponse);
            }
            else
            {
                msgResponse = "There is problem with server connection. Try again later.";
                Process_Error(msgResponse);
            }
        }

        private void Finish_Registration(string msg)
        {
            test.Text = msg;
        }

        private void Finish_Login(UserAuthenticated user, bool remember)
        {
            Properties.Settings.Default.logged = true;

            Properties.Settings.Default.remember = remember;
            Properties.Settings.Default.Id = user.Id;
            Properties.Settings.Default.NickName = user.NickName;
            Properties.Settings.Default.Token = user.Token;
            ApiRequests.client.DefaultRequestHeaders.Add("Auth-token", user.Token);
            if (remember)
            {                
                Properties.Settings.Default.Save();
            }
            this.Close();
        }

        private void Process_Error(string errorMessage)
        {
            test.Text = errorMessage;
        }

        private bool Verify_Register_Form(RegisterForm rF)
        {
            bool valid = true;

            if (string.IsNullOrEmpty(rF.NickName) || rF.NickName.Length < 8)
            {
                nickNameRegiserBox.BorderBrush = Brushes.Red;
                valid = false;
            }
            else { nickNameRegiserBox.BorderBrush = Brushes.Green; }

            if (!rF.Email.Contains('@'))
            {
                emailRegisterBox.BorderBrush = Brushes.Red;
                valid = false;
            }
            else { emailRegisterBox.BorderBrush = Brushes.Green; }

            if (string.IsNullOrEmpty(rF.Password) || rF.Password.Length < 8)
            {
                passwordRegisterbox.BorderBrush = Brushes.Red;
                valid = false;
            }
            else { passwordRegisterbox.BorderBrush = Brushes.Green; }

            return valid;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string nickname = nickNameLoginBox.Text;
            string password = passwordLoginBox.Password;
            bool remember = (bool)rememberBtn.IsChecked;

            LoginForm loginForm = new LoginForm(nickname, password);

            HttpResponseMessage httpResponse = await ApiRequests.Login(loginForm);

            if(httpResponse.StatusCode == HttpStatusCode.OK)
            {
                UserAuthenticated user = await httpResponse.Content.ReadAsAsync<UserAuthenticated>();
                Finish_Login(user, remember);
            }
            else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                nickNameLoginBox.BorderBrush = Brushes.Red;
                passwordLoginBox.BorderBrush = Brushes.Red;
                string msgResponse = await httpResponse.Content.ReadAsStringAsync();
                Process_Error(msgResponse);
            }
            else
            {
                string msgResponse = "There is problem with server connection. Try again later.";
                Process_Error(msgResponse);
            }
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
