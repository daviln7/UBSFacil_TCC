using UBSFacil.Models;
using UBSFacil.Services; 
using SQLite; 
using System.Linq; 

namespace UBSFacil.View
{
    public partial class HomePage : ContentPage
    {
        private Usuario _usuario;

        public HomePage(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;

            SaudacaoLabel.Text = $"Olá, {_usuario.NomeCompleto}!";

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarAgendamentos(); 
        }

        private async Task CarregarAgendamentos() 
        {
            try
            {
                var databaseService = DatabaseService.Instance;

                var agendamentos = await databaseService.GetAppointmentsAsync(_usuario.Id);

                if (agendamentos != null && agendamentos.Any())
                {
                    AgendamentosCollectionView.ItemsSource = agendamentos;
                    AgendamentosCollectionView.IsVisible = true;
                    SemAgendamentosLabel.IsVisible = false;
                }
                else
                {
                    AgendamentosCollectionView.IsVisible = false;
                    SemAgendamentosLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar agendamentos: {ex.Message}", "OK");
            }
        }

        private async void OnInicioClicked(object sender, EventArgs e)
        {
            
        }

        private async void OnAgendamentoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgendamentosPage(_usuario));
        }

        private async void OnDadosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DadosCadastraisPage(_usuario));
        }
    }
}
