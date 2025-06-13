// DadosCadastraisPage.xaml.cs
using Microsoft.Maui.Controls;
using System;
using UBSFacil.Models;
using UBSFacil.Services;

namespace UBSFacil.View
{
    public partial class DadosCadastraisPage : ContentPage
    {
        private Usuario _usuarioLogado; // A referência ao usuário que está sendo editado

        // O construtor deve receber o objeto Usuario do usuário logado
        public DadosCadastraisPage(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
        }

        // Este método é chamado sempre que a página aparece na tela
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarDadosUsuario(); // Carrega os dados do usuário nos campos de entrada
        }

        private void CarregarDadosUsuario()
        {
            // Preenche os campos de entrada com os dados atuais do usuário
            EntryNomeCompleto.Text = _usuarioLogado.NomeCompleto;
            EntryEmail.Text = _usuarioLogado.Email;
            EntryTelefone.Text = _usuarioLogado.Telefone;
            // A senha não é exibida por segurança, apenas preenchida se o usuário quiser mudar
            EntryNovaSenha.Text = string.Empty; // Sempre limpa o campo de nova senha
        }

        private async void OnAtualizarDadosClicked(object sender, EventArgs e)
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(EntryNomeCompleto.Text) ||
                string.IsNullOrWhiteSpace(EntryEmail.Text) ||
                string.IsNullOrWhiteSpace(EntryTelefone.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos obrigatórios.", "OK");
                return;
            }

            // Atualiza o objeto _usuarioLogado com os novos dados
            _usuarioLogado.NomeCompleto = EntryNomeCompleto.Text.Trim();
            _usuarioLogado.Telefone = EntryTelefone.Text.Trim();

            // Validação e-mail: se o e-mail mudou, verifique se já existe (opcional, mais complexo)
            string novoEmailNormalizado = EntryEmail.Text.Trim().ToLower();
            if (_usuarioLogado.Email.ToLower() != novoEmailNormalizado)
            {
                // Se o e-mail mudou, você pode querer verificar no banco se ele já existe
                // Para simplicidade, vamos permitir a mudança, mas em app real, isso exigiria mais lógica
                _usuarioLogado.Email = novoEmailNormalizado;
            }

            // Atualiza a senha se o campo Nova Senha não estiver vazio
            if (!string.IsNullOrWhiteSpace(EntryNovaSenha.Text))
            {
                // Em uma aplicação real, aqui você hash/salt a nova senha antes de salvar!
                _usuarioLogado.Senha = EntryNovaSenha.Text;
            }

            try
            {
                // Chama o método de atualização no DatabaseService
                int rowsAffected = await DatabaseService.Instance.UpdateUserAsync(_usuarioLogado);

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Sucesso", "Seus dados foram atualizados com sucesso!", "OK");
                    // Opcional: Voltar para a página anterior ou atualizar dados em outra tela.
                    // Se você tiver uma representação global do usuário logado (ex: em uma classe AppState),
                    // você pode querer atualizar essa instância também para refletir as mudanças imediatamente.
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível atualizar seus dados. Tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao atualizar: {ex.Message}", "OK");
            }
        }
    }
}
