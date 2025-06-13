using UBSFacil.Models;
using UBSFacil.Services; // É crucial ter este using para acessar DatabaseService
using SQLite; // Ainda é útil se você usar tipos do SQLite.NET, mas o acesso principal será via DatabaseService
using System.Linq; // Para usar métodos Linq como Any()

namespace UBSFacil.View
{
    public partial class HomePage : ContentPage
    {
        private Usuario _usuario;
        // Remova a propriedade _database daqui.
        // Você não vai mais interagir diretamente com SQLiteAsyncConnection aqui.
        // private readonly SQLiteAsyncConnection _database; // <-- REMOVA ESTA LINHA

        public HomePage(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;

            // --- CORREÇÃO AQUI ---
            // Você não precisa obter a conexão diretamente.
            // O DatabaseService.Instance já encapsula a lógica de banco de dados.
            // Remova ou comente a linha abaixo:
            // _database = DatabaseService.GetConnection(); // <-- REMOVA/COMENTE ESTA LINHA

            SaudacaoLabel.Text = $"Olá, {_usuario.NomeCompleto}!";

            // Chame CarregarAgendamentos no OnAppearing para garantir que carregue sempre que a página aparecer
            // e também para que possa ser async e await
            // CarregarAgendamentos(); // <-- Remova esta chamada do construtor
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarAgendamentos(); // Chama CarregarAgendamentos de forma assíncrona
        }

        private async Task CarregarAgendamentos() // Mude para Task para poder ser awaited
        {
            try
            {
                // Acesso ao DatabaseService através da instância Singleton
                var databaseService = DatabaseService.Instance;

                // Use o método GetAppointmentsAsync do DatabaseService
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
            // Já está na HomePage
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
