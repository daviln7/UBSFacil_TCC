// AgendamentosPage.xaml.cs
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
        private Usuario _usuarioLogado; // Para saber qual usu�rio est� agendando
        private List<UnidadeSaude> _listaUBSs; // Para popular o Picker

        // O construtor deve receber o usu�rio logado
        public AgendamentosPage(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;

            // Define a data e hora iniciais como a data e hora atuais
            DatePickerAgendamento.Date = DateTime.Today;
            DatePickerAgendamento.MinimumDate = DateTime.Today; // <--- LINHA ADICIONADA/CORRIGIDA AQUI
            TimePickerAgendamento.Time = DateTime.Now.TimeOfDay;

            CarregarUBSs(); // Carrega as UBSs no Picker
        }

        private async void CarregarUBSs()
        {
            // Pega a inst�ncia do DatabaseService
            var databaseService = DatabaseService.Instance;

            // TODO: No futuro, voc� pode ter um m�todo no DatabaseService para GetUnidadesSaudeAsync()
            // Por enquanto, vamos usar as que voc� mencionou.
            _listaUBSs = new List<UnidadeSaude>
            {
                new UnidadeSaude { Id = 1, Nome = "UBS Vila Suissa", Endereco = "Rua das Flores, 123", Imagem = "ubs_icon.png" },
                new UnidadeSaude { Id = 2, Nome = "UBS Jd. Universo", Endereco = "Av. dos Astros, 456", Imagem = "ubs_icon.png" },
                new UnidadeSaude { Id = 3, Nome = "UBS Pq. Ol�mpico", Endereco = "Rua das Medalhas, 789", Imagem = "ubs_icon.png" }
            };

            // Popula o Picker com os nomes das UBSs
            PickerUBS.ItemsSource = _listaUBSs.Select(u => u.Nome).ToList();

            // Seleciona o primeiro item por padr�o, se houver
            if (_listaUBSs.Any())
            {
                PickerUBS.SelectedIndex = 0;
            }
        }

        private async void OnConfirmarAgendamentoClicked(object sender, EventArgs e)
        {
            // Valida��o b�sica
            if (PickerUBS.SelectedItem == null)
            {
                await DisplayAlert("Erro", "Por favor, selecione uma Unidade de Sa�de.", "OK");
                return;
            }

            // Combina a data do DatePicker e a hora do TimePicker
            DateTime dataHoraAgendamento = DatePickerAgendamento.Date.Add(TimePickerAgendamento.Time);

            // Valida se a data/hora n�o � no passado
            if (dataHoraAgendamento < DateTime.Now)
            {
                await DisplayAlert("Erro", "N�o � poss�vel agendar para uma data/hora no passado.", "OK");
                return;
            }

            // Obt�m o nome da UBS selecionada
            string nomeUBSSelecionada = PickerUBS.SelectedItem.ToString();

            // Encontra o objeto UnidadeSaude correspondente (se precisar do Id)
            // UnidadeSaude ubsSelecionada = _listaUBSs.FirstOrDefault(u => u.Nome == nomeUBSSelecionada);

            // Cria um novo objeto Agendamento
            var novoAgendamento = new Agendamento
            {
                UsuarioId = _usuarioLogado.Id,
                DataAgendamento = dataHoraAgendamento,
                UnidadeSaude = nomeUBSSelecionada, // Salva o nome da UBS
                Observacoes = EditorObservacoes.Text // Pega o texto das observa��es
            };

            try
            {
                // Salva o agendamento no banco de dados usando o DatabaseService
                int rowsAffected = await DatabaseService.Instance.AddAppointmentAsync(novoAgendamento);

                if (rowsAffected > 0)
                {
                    await DisplayAlert("Sucesso", "Agendamento confirmado!", "OK");

                    // Opcional: Navegar de volta para a HomePage ou limpar os campos
                    await Navigation.PopAsync(); // Volta para a p�gina anterior (HomePage)
                }
                else
                {
                    await DisplayAlert("Erro", "N�o foi poss�vel salvar o agendamento.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao agendar: {ex.Message}", "OK");
            }
        }
    }
}