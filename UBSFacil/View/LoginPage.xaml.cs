// View/LoginPage.xaml.cs
using UBSFacil.Models;
using UBSFacil.Services;
using UBSFacil.View;

namespace UBSFacil.View
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public LoginPage()
        {
            InitializeComponent();
            _databaseService = DatabaseService.Instance;
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_usuario.Text) ||
                string.IsNullOrWhiteSpace(txt_senha.Text))
            {
                await DisplayAlert("Campos Vazios", "Por favor, insira seu email e senha.", "OK");
                return;
            }

            Usuario usuario = await _databaseService.LoginUserAsync(txt_usuario.Text, txt_senha.Text);

            if (usuario != null)
            {
                // ✅ Colocando a HomePage dentro de uma NavigationPage
                Application.Current.MainPage = new NavigationPage(new HomePage(usuario));
            }
            else
            {
                await DisplayAlert("Falha no Login", "Email ou senha incorretos.", "OK");
            }
        }
    }
}
