using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using UBSFacil.Models;
using UBSFacil.Services;

namespace UBSFacil.View
{
    public partial class AgendamentosPage : ContentPage
    {
        private Usuario _usuarioLogado; 
        private List<UnidadeSaude> _listaUBSs; 

        public AgendamentosPage(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;

            DatePickerAgendamento.Date = DateTime.Today;
            DatePickerAgendamento.MinimumDate = DateTime.Today; 
            TimePickerAgendamento.Time = DateTime.Now.TimeOfDay;

            CarregarUBSs(); 
        }

        private async void CarregarUBSs()
        {

            var databaseService = DatabaseService.Instance;

            _listaUBSs = new List<UnidadeSaude>
            {
                new UnidadeSaude { Id = 1, Nome = "UBS Vila Suissa", Endereco = "Rua das Flores, 123", Imagem = "ubs_icon.png" },
                new UnidadeSaude { Id = 2, Nome = "UBS Jd. Universo", Endereco = "Av. dos Astros, 456", Imagem = "ubs_icon.png" },
                new UnidadeSaude { Id = 3, Nome = "UBS Pq. Olímpico", Endereco = "Rua das Medalhas, 789", Imagem = "ubs_icon.png" }
            };

            PickerUBS.ItemsSource = _listaUBSs.Select(u => u.Nome).ToList();

            if (_listaUBSs.Any())
            {
                PickerUBS.SelectedIndex = 0;
            }
        }

        private async void OnConfirmarAgendamentoClicked(object sender, EventArgs e)
        {

            if (PickerUBS.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Por favor, selecione uma Unidade de Saúde.", "OK");
                return;
            }

            DateTime dataHoraAgendamento = DatePickerAgendamento.Date.Add(TimePickerAgendamento.Time);

            if (dataHoraAgendamento < DateTime.Now)
            {
                await DisplayAlert("Erro", "Não é possível agendar para uma data/hora no passado.", "OK");
                return;
            }

            string nomeUBSSelecionada = PickerUBS.SelectedItem.ToString();

            var novoAgendamento = new Agendamento
            {
                UsuarioId = _usuarioLogado.Id,
                DataAgendamento = dataHoraAgendamento,
                UnidadeSaude = nomeUBSSelecionada, 
                Observacoes = EditorObservacoes.Text 
            };

            try
            {
                int rowsAffected = await DatabaseService.Instance.AddAppointmentAsync(novoAgendamento);

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Sucesso", "Agendamento confirmado!", "OK");

                    await Navigation.PopAsync(); 
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível salvar o agendamento.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao agendar: {ex.Message}", "OK");
            }
        }
    }
}