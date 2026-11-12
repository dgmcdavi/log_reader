using System;
using System.Collections.Generic;
using System.IO;
using LogReader.Models;
using LogReader.Services;
using LogReader.Views;

namespace LogReader.Controllers
{
    /// <summary>
    /// Controlador principal que orquestra a aplicação.
    /// Coordena Services, View e gerencia o fluxo do programa.
    /// </summary>
    public class LogController
    {
        private readonly LogFileService _fileService;
        private readonly LogFilterService _filterService;
        private readonly ConsoleView _view;

        private IEnumerable<LogEntry> _allEntries = new List<LogEntry>();
        private LogStatistics _statistics = new LogStatistics();

        /// <summary>
        /// Inicializa o controlador com as dependências necessárias.
        /// </summary>
        /// <param name="fileService">Serviço de arquivo.</param>
        /// <param name="filterService">Serviço de filtro.</param>
        /// <param name="view">Interface de usuário.</param>
        public LogController(LogFileService fileService, LogFilterService filterService, ConsoleView view)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _filterService = filterService ?? throw new ArgumentNullException(nameof(filterService));
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Executa o loop principal da aplicação.
        /// </summary>
        public void Run()
        {
            try
            {
                _view.ShowWelcome();

                // Etapa 1: Obter e validar caminho do arquivo
                string filePath = GetValidFilePath();
                if (string.IsNullOrEmpty(filePath))
                {
                    _view.ShowGoodbye();
                    return;
                }

                // Etapa 2: Ler e processar arquivo
                if (!LoadAndProcessFile(filePath))
                {
                    return;
                }

                // Etapa 3: Exibir resumo
                _view.ShowSummary(_statistics);

                // Etapa 4: Loop do menu principal (com desafios extras)
                RunMainMenuLoop();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro inesperado: {ex.Message}");
            }
            finally
            {
                _view.ShowGoodbye();
            }
        }

        /// <summary>
        /// Solicita e valida o caminho do arquivo até ser válido ou usuário cancelar.
        /// </summary>
        private string GetValidFilePath()
        {
            while (true)
            {
                string path = _view.PromptFilePath();

                if (string.IsNullOrWhiteSpace(path))
                {
                    _view.ShowError("Caminho não pode ser vazio. Tente novamente ou pressione Ctrl+C para sair.");
                    continue;
                }

                if (_fileService.ValidateFilePath(path, out string errorMessage))
                {
                    return path;
                }

                _view.ShowError(errorMessage);
                Console.Write("Deseja tentar outro arquivo? (S/N): ");
                var retry = Console.ReadLine()?.Trim().ToUpperInvariant();
                if (retry != "S" && retry != "SIM")
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Carrega o arquivo e calcula estatísticas.
        /// </summary>
        private bool LoadAndProcessFile(string filePath)
        {
            try
            {
                _view.ShowInfo($"Lendo arquivo: {filePath}");

                _allEntries = _fileService.ReadLogFile(filePath);
                _statistics = CalculateStatistics(_allEntries);

                return true;
            }
            catch (FileNotFoundException ex)
            {
                _view.ShowError(ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                _view.ShowError($"Sem permissão para acessar o arquivo: {ex.Message}");
                return false;
            }
            catch (System.IO.IOException ex)
            {
                _view.ShowError($"Erro de leitura do arquivo: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro inesperado ao processar arquivo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Calcula estatísticas a partir das entradas.
        /// </summary>
        private LogStatistics CalculateStatistics(IEnumerable<LogEntry> entries)
        {
            var stats = new LogStatistics();

            foreach (var entry in entries)
            {
                stats.Increment(entry);
            }

            return stats;
        }

        /// <summary>
        /// Executa o loop do menu principal com opções extras.
        /// </summary>
        private void RunMainMenuLoop()
        {
            while (true)
            {
                _view.ShowMainMenu();
                string choice = _view.PromptMainMenuChoice();

                switch (choice)
                {
                    case "1":
                        RunLevelFilterLoop();
                        break;
                    case "2":
                        RunSearchByTerm();
                        break;
                    case "3":
                        RunFilterByDateRange();
                        break;
                    case "4":
                        ShowPercentages();
                        break;
                    case "5":
                        SaveSummaryToFile();
                        break;
                    case "0":
                        return;
                    default:
                        _view.ShowError("Opção inválida. Tente novamente.");
                        _view.WaitForEnter();
                        break;
                }
            }
        }

        /// <summary>
        /// Loop do submenu de filtro por nível.
        /// </summary>
        private void RunLevelFilterLoop()
        {
            while (true)
            {
                _view.ShowLevelFilterMenu();
                string choice = _view.PromptLevelFilterChoice();
                string? level = _view.MapLevelChoiceToLevel(choice);

                if (level == null)
                {
                    // Usuário escolheu voltar
                    break;
                }

                if (level == "INVALID")
                {
                    _view.ShowError("Opção inválida. Tente novamente.");
                    _view.WaitForEnter();
                    continue;
                }

                // Aplicar filtro e exibir resultados
                var filteredEntries = _filterService.FilterByLevel(_allEntries, level);
                _view.ShowEntries(filteredEntries, level);
                _view.WaitForEnter();
            }
        }

        /// <summary>
        /// Executa busca por termo nas mensagens (desafio extra).
        /// </summary>
        private void RunSearchByTerm()
        {
            var term = _view.PromptSearchTerm();
            if (term == null)
            {
                return; // Usuário cancelou
            }

            var results = _filterService.SearchByTerm(_allEntries, term);
            _view.ShowEntries(results, $"BUSCA: \"{term}\"");
            _view.WaitForEnter();
        }

        /// <summary>
        /// Executa filtro por intervalo de datas e horário (desafio extra).
        /// </summary>
        private void RunFilterByDateRange()
        {
            var dateRange = _view.PromptDateRange();
            if (dateRange == null)
            {
                return; // Usuário cancelou ou erro
            }

            var (startDateTime, endDateTime) = dateRange.Value;
            var results = _filterService.FilterByDateRange(_allEntries, startDateTime, endDateTime);
            
            string label;
            bool hasTime = startDateTime.TimeOfDay != TimeSpan.Zero || endDateTime.TimeOfDay != TimeSpan.Zero;
            
            if (startDateTime.Date == endDateTime.Date && !hasTime)
            {
                label = $"DATA: {startDateTime:dd/MM/yyyy}";
            }
            else if (startDateTime == endDateTime)
            {
                label = $"DATA/HORA: {startDateTime:dd/MM/yyyy HH:mm:ss}";
            }
            else
            {
                string startFmt = hasTime ? "dd/MM/yyyy HH:mm:ss" : "dd/MM/yyyy";
                string endFmt = hasTime ? "dd/MM/yyyy HH:mm:ss" : "dd/MM/yyyy";
                label = $"PERÍODO: {startDateTime.ToString(startFmt)} a {endDateTime.ToString(endFmt)}";
            }
            
            _view.ShowEntries(results, label);
            _view.WaitForEnter();
        }

        /// <summary>
        /// Exibe porcentagens por nível (desafio extra).
        /// </summary>
        private void ShowPercentages()
        {
            _view.ShowPercentages(_statistics);
            _view.WaitForEnter();
        }

        /// <summary>
        /// Salva o resumo em arquivo .txt (desafio extra).
        /// </summary>
        private void SaveSummaryToFile()
        {
            var path = _view.PromptSavePath();
            if (path == null)
            {
                return; // Usuário cancelou
            }

            try
            {
                var summary = _statistics.ToString();
                File.WriteAllText(path, summary);
                _view.ShowSuccess($"Resumo salvo com sucesso em: {path}");
            }
            catch (UnauthorizedAccessException)
            {
                _view.ShowError("Sem permissão para escrever no arquivo especificado.");
            }
            catch (DirectoryNotFoundException)
            {
                _view.ShowError("Diretório não encontrado.");
            }
            catch (IOException ex)
            {
                _view.ShowError($"Erro ao salvar arquivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                _view.ShowError($"Erro inesperado ao salvar: {ex.Message}");
            }

            _view.WaitForEnter();
        }
    }
}