// View/CadastroPage.xaml.cs
using UBSFacil.Models;
using UBSFacil.Services;

namespace UBSFacil.View
{
    public partial class CadastroPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public CadastroPage() // Construtor SEM parâmetros
        {
            InitializeComponent();
            _databaseService = DatabaseService.Instance; // Usando o Singleton
        }

        private async void OnCadastrarButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_nome_completo.Text) ||
                string.IsNullOrWhiteSpace(txt_email.Text) ||
                string.IsNullOrWhiteSpace(txt_telefone.Text) ||
                string.IsNullOrWhiteSpace(txt_senha_cadastro.Text))
            {
                await DisplayAlert("Campos Vazios", "Por favor, preencha todos os campos.", "OK");
                return;
            }
            // Você poderia adicionar um campo "Confirmar Senha" no XAML e verificar aqui se as senhas coincidem.

            var novoUsuario = new Usuario
            {
                NomeCompleto = txt_nome_completo.Text,
                Email = txt_email.Text,
                Telefone = txt_telefone.Text,
                Senha = txt_senha_cadastro.Text // 🚨 Lembre-se do HASHING DE SENHA AQUI!
            };

            bool sucesso = await _databaseService.RegisterUserAsync(novoUsuario);

            if (sucesso)
            {
                await DisplayAlert("Cadastro Realizado", "Sua conta foi criada com sucesso! Faça o login.", "OK");

                // Opcional: Limpar campos
                txt_nome_completo.Text = string.Empty;
                txt_email.Text = string.Empty;
                txt_telefone.Text = string.Empty;
                txt_senha_cadastro.Text = string.Empty;

                // Tenta remover a página de cadastro da pilha antes de ir para o login
                if (Navigation.NavigationStack.Count > 1 && Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] == this)
                {
                    Navigation.RemovePage(this);
                }
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                await DisplayAlert("Falha no Cadastro", "Este email já pode estar em uso ou ocorreu um erro.", "OK");
            }
        }
    }
}