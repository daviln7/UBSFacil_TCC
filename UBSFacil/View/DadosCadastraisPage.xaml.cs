using Microsoft.Maui.Controls;
using System;
using UBSFacil.Models;
using UBSFacil.Services;

namespace UBSFacil.View
{
    public partial class DadosCadastraisPage : ContentPage
    {
        private Usuario _usuarioLogado; 

        public DadosCadastraisPage(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CarregarDadosUsuario(); 
        }

        private void CarregarDadosUsuario()
        {
            EntryNomeCompleto.Text = _usuarioLogado.NomeCompleto;
            EntryEmail.Text = _usuarioLogado.Email;
            EntryTelefone.Text = _usuarioLogado.Telefone;

            EntryNovaSenha.Text = string.Empty; 
        }

        private async void OnAtualizarDadosClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(EntryNomeCompleto.Text) ||
                string.IsNullOrWhiteSpace(EntryEmail.Text) ||
                string.IsNullOrWhiteSpace(EntryTelefone.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos obrigatórios.", "OK");
                return;
            }


            _usuarioLogado.NomeCompleto = EntryNomeCompleto.Text.Trim();
            _usuarioLogado.Telefone = EntryTelefone.Text.Trim();


            string novoEmailNormalizado = EntryEmail.Text.Trim().ToLower();
            if (_usuarioLogado.Email.ToLower() != novoEmailNormalizado)
            {

                _usuarioLogado.Email = novoEmailNormalizado;
            }

            if (!string.IsNullOrWhiteSpace(EntryNovaSenha.Text))
            {

                _usuarioLogado.Senha = EntryNovaSenha.Text;
            }

            try
            {

                int rowsAffected = await DatabaseService.Instance.UpdateUserAsync(_usuarioLogado);

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Sucesso", "Seus dados foram atualizados com sucesso!", "OK");
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
