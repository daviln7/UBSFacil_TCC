// DadosCadastraisPage.xaml.cs
using Microsoft.Maui.Controls;
using System;
using UBSFacil.Models;
using UBSFacil.Services;

namespace UBSFacil.View
{
    public partial class DadosCadastraisPage : ContentPage
    {
        private Usuario _usuarioLogado; // A refer�ncia ao usu�rio que est� sendo editado

        // O construtor deve receber o objeto Usuario do usu�rio logado
        public DadosCadastraisPage(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
        }

        // Este m�todo � chamado sempre que a p�gina aparece na tela
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarDadosUsuario(); // Carrega os dados do usu�rio nos campos de entrada
        }

        private void CarregarDadosUsuario()
        {
            // Preenche os campos de entrada com os dados atuais do usu�rio
            EntryNomeCompleto.Text = _usuarioLogado.NomeCompleto;
            EntryEmail.Text = _usuarioLogado.Email;
            EntryTelefone.Text = _usuarioLogado.Telefone;
            // A senha n�o � exibida por seguran�a, apenas preenchida se o usu�rio quiser mudar
            EntryNovaSenha.Text = string.Empty; // Sempre limpa o campo de nova senha
        }

        private async void OnAtualizarDadosClicked(object sender, EventArgs e)
        {
            // Valida��o b�sica
            if (string.IsNullOrWhiteSpace(EntryNomeCompleto.Text) ||
                string.IsNullOrWhiteSpace(EntryEmail.Text) ||
                string.IsNullOrWhiteSpace(EntryTelefone.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos obrigat�rios.", "OK");
                return;
            }

            // Atualiza o objeto _usuarioLogado com os novos dados
            _usuarioLogado.NomeCompleto = EntryNomeCompleto.Text.Trim();
            _usuarioLogado.Telefone = EntryTelefone.Text.Trim();

            // Valida��o e-mail: se o e-mail mudou, verifique se j� existe (opcional, mais complexo)
            string novoEmailNormalizado = EntryEmail.Text.Trim().ToLower();
            if (_usuarioLogado.Email.ToLower() != novoEmailNormalizado)
            {
                // Se o e-mail mudou, voc� pode querer verificar no banco se ele j� existe
                // Para simplicidade, vamos permitir a mudan�a, mas em app real, isso exigiria mais l�gica
                _usuarioLogado.Email = novoEmailNormalizado;
            }

            // Atualiza a senha se o campo Nova Senha n�o estiver vazio
            if (!string.IsNullOrWhiteSpace(EntryNovaSenha.Text))
            {
                // Em uma aplica��o real, aqui voc� hash/salt a nova senha antes de salvar!
                _usuarioLogado.Senha = EntryNovaSenha.Text;
            }

            try
            {
                // Chama o m�todo de atualiza��o no DatabaseService
                int rowsAffected = await DatabaseService.Instance.UpdateUserAsync(_usuarioLogado);

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Sucesso", "Seus dados foram atualizados com sucesso!", "OK");
                    // Opcional: Voltar para a p�gina anterior ou atualizar dados em outra tela.
                    // Se voc� tiver uma representa��o global do usu�rio logado (ex: em uma classe AppState),
                    // voc� pode querer atualizar essa inst�ncia tamb�m para refletir as mudan�as imediatamente.
                }
                else
                {
                    await DisplayAlert("Erro", "N�o foi poss�vel atualizar seus dados. Tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao atualizar: {ex.Message}", "OK");
            }
        }
    }
}
