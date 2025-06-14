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
        public async Task<bool> RegisterUserAsync(Usuario usuario)
        {
            await InitializeAsync();

            var emailNormalizado = usuario.Email.Trim().ToLower();

            var existingUser = await _database.Table<Usuario>()
                                             .Where(u => u.Email.ToLower() == emailNormalizado)
                                             .FirstOrDefaultAsync();

            if (existingUser != null)
                return false; 

            usuario.Email = emailNormalizado; 
            int rows = await _database.InsertAsync(usuario);
            return rows > 0;
        }

        public async Task<Usuario> LoginUserAsync(string email, string senha)
        {
            await InitializeAsync();

            var emailNormalizado = email.Trim().ToLower();

            return await _database.Table<Usuario>()
                                  .Where(u => u.Email.ToLower() == emailNormalizado && u.Senha == senha)
                                  .FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetUserByIdAsync(int userId)
        {
            await InitializeAsync();
            return await _database.Table<Usuario>().Where(u => u.Id == userId).FirstOrDefaultAsync();
        }
        public async Task<int> UpdateUserAsync(Usuario usuario)
        {
            await InitializeAsync();

            return await _database.UpdateAsync(usuario);
        }

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