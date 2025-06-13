// Services/DatabaseService.cs (Versão Final e Corrigida)
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UBSFacil.Models;

namespace UBSFacil.Services
{
    public class DatabaseService
    {
        #region PROPRIEDADES E SINGLETON (NÃO ALTERAR)
        private static DatabaseService _instance;
        private static readonly object _lock = new object();
        private SQLiteAsyncConnection _database;
        private bool _initialized = false;

        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "UBSFacilData.db3");

        private DatabaseService()
        {
            _database = new SQLiteAsyncConnection(DbPath);
        }

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new DatabaseService();
                    }
                }
                return _instance;
            }
        }
        #endregion

        private async Task InitializeAsync()
        {
            if (!_initialized)
            {
                await _database.CreateTableAsync<Usuario>();
                await _database.CreateTableAsync<UnidadeSaude>();
                await _database.CreateTableAsync<Agendamento>();
                await _database.CreateTableAsync<Favorito>();

                _initialized = true;

                await SeedInitialDataAsync();
            }
        }

        private async Task SeedInitialDataAsync()
        {
            if (await _database.Table<UnidadeSaude>().CountAsync() == 0)
            {
                var ubsList = new List<UnidadeSaude>
                {
                    new UnidadeSaude { Nome = "UBS Vila Suissa", Endereco = "Rua das Flores, 123", Imagem = "ubs_icon.png" },
                    new UnidadeSaude { Nome = "UBS Jd. Universo", Endereco = "Av. dos Astros, 456", Imagem = "ubs_icon.png" },
                    new UnidadeSaude { Nome = "UBS Pq. Olímpico", Endereco = "Rua das Medalhas, 789", Imagem = "ubs_icon.png" }
                };

                await _database.InsertAllAsync(ubsList);
            }
        }

        // --- MÉTODOS DE USUÁRIO ---

        /// <summary>
        /// Registra um novo usuário no banco de dados.
        /// </summary>
        /// <returns>Retorna 'true' se o registro for bem-sucedido, 'false' se o e-mail já existir.</returns>
        public async Task<bool> RegisterUserAsync(Usuario usuario)
        {
            await InitializeAsync();

            var emailNormalizado = usuario.Email.Trim().ToLower();

            var existingUser = await _database.Table<Usuario>()
                                             .Where(u => u.Email.ToLower() == emailNormalizado)
                                             .FirstOrDefaultAsync();

            if (existingUser != null)
                return false; // E-mail já cadastrado

            usuario.Email = emailNormalizado; // Normaliza antes de salvar
            int rows = await _database.InsertAsync(usuario);
            return rows > 0;
        }

        /// <summary>
        /// Verifica as credenciais de login de um usuário.
        /// </summary>
        /// <returns>Retorna o objeto 'Usuario' se as credenciais forem válidas, ou 'null' caso contrário.</returns>
        public async Task<Usuario> LoginUserAsync(string email, string senha)
        {
            await InitializeAsync();

            var emailNormalizado = email.Trim().ToLower();

            // Em produção, validar hash de senha!
            return await _database.Table<Usuario>()
                                  .Where(u => u.Email.ToLower() == emailNormalizado && u.Senha == senha)
                                  .FirstOrDefaultAsync();
        }

        // --- MÉTODOS DE USUÁRIO (NOVOS PARA DADOS CADASTRAIS) ---

        /// <summary>
        /// Obtém um usuário pelo seu ID.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <returns>O objeto Usuario correspondente ou null se não encontrado.</returns>
        public async Task<Usuario> GetUserByIdAsync(int userId)
        {
            await InitializeAsync();
            return await _database.Table<Usuario>().Where(u => u.Id == userId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Atualiza as informações de um usuário existente no banco de dados.
        /// </summary>
        /// <param name="usuario">O objeto Usuario com os dados a serem atualizados (o Id deve corresponder a um usuário existente).</param>
        /// <returns>Retorna o número de linhas afetadas (1 se a atualização foi bem-sucedida, 0 caso contrário).</returns>
        public async Task<int> UpdateUserAsync(Usuario usuario)
        {
            await InitializeAsync();
            // A normalização do e-mail é importante, mas aqui só atualizamos.
            // A validação de e-mail único já foi feita no registro, mas considere uma validação na atualização se o e-mail mudar.
            return await _database.UpdateAsync(usuario);
        }

        // --- MÉTODOS DE FAVORITOS ---

        public async Task<bool> IsUnitFavoriteAsync(int usuarioId, int unidadeId)
        {
            await InitializeAsync();

            var favorito = await _database.Table<Favorito>()
                                         .Where(f => f.UsuarioId == usuarioId && f.UnidadeId == unidadeId)
                                         .FirstOrDefaultAsync();

            return favorito != null;
        }

        public async Task ToggleFavoriteAsync(int usuarioId, int unidadeId)
        {
            await InitializeAsync();

            var isFavorite = await IsUnitFavoriteAsync(usuarioId, unidadeId);

            if (isFavorite)
            {
                var favoritoExistente = await _database.Table<Favorito>()
                                                     .Where(f => f.UsuarioId == usuarioId && f.UnidadeId == unidadeId)
                                                     .FirstOrDefaultAsync();

                if (favoritoExistente != null)
                    await _database.DeleteAsync(favoritoExistente);
            }
            else
            {
                var novoFavorito = new Favorito { UsuarioId = usuarioId, UnidadeId = unidadeId };
                await _database.InsertAsync(novoFavorito);
            }
        }

        public async Task<List<UnidadeSaude>> GetFavoriteUnitsAsync(int usuarioId)
        {
            await InitializeAsync();

            var favoritos = await _database.Table<Favorito>()
                                           .Where(f => f.UsuarioId == usuarioId)
                                           .ToListAsync();

            var favoriteIds = favoritos.Select(f => f.UnidadeId).ToList();

            return await _database.Table<UnidadeSaude>()
                                  .Where(u => favoriteIds.Contains(u.Id))
                                  .ToListAsync();
        }

        // --- MÉTODOS DE AGENDAMENTOS ---

        public async Task<List<Agendamento>> GetAppointmentsAsync(int usuarioId)
        {
            await InitializeAsync();

            return await _database.Table<Agendamento>()
                                  .Where(a => a.UsuarioId == usuarioId)
                                  .OrderBy(a => a.DataAgendamento)
                                  .ToListAsync();
        }

        public async Task<int> AddAppointmentAsync(Agendamento agendamento)
        {
            await InitializeAsync();
            return await _database.InsertAsync(agendamento);
        }
    }
}