// View/MainPage.xaml.cs
using UBSFacil.View;

namespace UBSFacil.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Botão para ir para Login
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        // Botão para ir para Cadastro
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CadastroPage());
        }
    }
}