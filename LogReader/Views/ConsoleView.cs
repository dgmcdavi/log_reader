using System;
using System.Collections.Generic;
using LogReader.Models;

namespace LogReader.Views
{
    /// <summary>
    /// Interface de usuário para exibição no console.
    /// Responsável por toda interação com o usuário via terminal.
    /// </summary>
    public class ConsoleView
    {
        private const string MainMenuOptions = @"
===== MENU PRINCIPAL =====
1 - Filtrar por nível
2 - Buscar por termo
3 - Filtrar por intervalo de datas
4 - Exibir porcentagens por nível
5 - Salvar resumo em arquivo
0 - Sair
==========================";

        private const string LevelFilterOptions = @"
===== FILTRAR POR NÍVEL =====
1 - Todas as entradas
2 - INFO
3 - WARNING
4 - ERROR
0 - Voltar
============================";

        /// <summary>
        /// Exibe mensagem de boas-vindas.
        /// </summary>
        public void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("   LEITOR DE LOGS - v1.0");
            Console.WriteLine("=================================");
            Console.WriteLine();
        }

        /// <summary>
        /// Solicita o caminho do arquivo de log ao usuário.
        /// </summary>
        /// <returns>Caminho informado pelo usuário.</returns>
        public string PromptFilePath()
        {
            Console.Write("Informe o caminho do arquivo .log: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Exibe mensagem de erro.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        public void ShowError(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[ERRO] {message}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Exibe mensagem de informação.
        /// </summary>
        /// <param name="message">Mensagem informativa.</param>
        public void ShowInfo(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n[INFO] {message}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Exibe mensagem de sucesso.
        /// </summary>
        /// <param name="message">Mensagem de sucesso.</param>
        public void ShowSuccess(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUCESSO] {message}");
            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Exibe o resumo estatístico dos logs.
        /// </summary>
        /// <param name="stats">Estatísticas a exibir.</param>
        public void ShowSummary(LogStatistics stats)
        {
            Console.WriteLine();
            Console.WriteLine(stats.ToString());
            Console.WriteLine();
        }

        /// <summary>
        /// Exibe o menu principal com opções extras.
        /// </summary>
        public void ShowMainMenu()
        {
            Console.WriteLine(MainMenuOptions);
        }

        /// <summary>
        /// Exibe o menu de filtro por nível.
        /// </summary>
        public void ShowLevelFilterMenu()
        {
            Console.WriteLine(LevelFilterOptions);
        }

        /// <summary>
        /// Solicita a escolha do menu principal.
        /// </summary>
        /// <returns>Opção escolhida como string.</returns>
        public string PromptMainMenuChoice()
        {
            Console.Write("Escolha uma opção: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Solicita a escolha de filtro por nível.
        /// </summary>
        /// <returns>Opção escolhida como string (1, 2, 3, 4, 0).</returns>
        public string PromptLevelFilterChoice()
        {
            Console.Write("Escolha uma opção: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Converte a opção do menu de nível para o nível de log correspondente.
        /// </summary>
        /// <param name="choice">Opção do menu (1, 2, 3, 4, 0).</param>
        /// <returns>Nível de log ou null para voltar.</returns>
        public string? MapLevelChoiceToLevel(string choice)
        {
            return choice switch
            {
                "1" => "TODAS",
                "2" => "INFO",
                "3" => "WARNING",
                "4" => "ERROR",
                "0" => null,
                _ => "INVALID"
            };
        }

        /// <summary>
        /// Solicita um termo de busca ao usuário.
        /// </summary>
        /// <returns>Termo informado ou null se cancelado.</returns>
        public string? PromptSearchTerm()
        {
            Console.Write("\nDigite o termo para buscar nas mensagens (ou Enter para cancelar): ");
            var term = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(term) ? null : term;
        }

        /// <summary>
        /// Solicita intervalo de datas e horário ao usuário.
        /// </summary>
        /// <returns>Tupla com (dataHoraInicial, dataHoraFinal) ou null se cancelado.</returns>
        public (DateTime start, DateTime end)? PromptDateRange()
        {
            Console.WriteLine("\n--- Filtrar por Intervalo de Data e Hora ---");
            Console.WriteLine("Formato: AAAA-MM-DD [HH:MM:SS] (hora/minuto/segundo são opcionais)");
            Console.Write("Data/hora inicial (AAAA-MM-DD [HH:MM:SS]) ou Enter para cancelar: ");
            var startInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(startInput))
                return null;

            Console.Write("Data/hora final (AAAA-MM-DD [HH:MM:SS]) ou Enter para mesma data/hora: ");
            var endInput = Console.ReadLine()?.Trim();

            if (!DateTime.TryParse(startInput, out var startDateTime))
            {
                ShowError("Data/hora inicial inválida. Use formato AAAA-MM-DD ou AAAA-MM-DD HH:MM:SS.");
                return null;
            }

            DateTime endDateTime;
            if (string.IsNullOrEmpty(endInput))
            {
                endDateTime = startDateTime;
            }
            else if (!DateTime.TryParse(endInput, out endDateTime))
            {
                ShowError("Data/hora final inválida. Use formato AAAA-MM-DD ou AAAA-MM-DD HH:MM:SS.");
                return null;
            }

            if (endDateTime < startDateTime)
            {
                ShowError("Data/hora final não pode ser anterior à data/hora inicial.");
                return null;
            }

            return (startDateTime, endDateTime);
        }

        /// <summary>
        /// Solicita caminho para salvar o resumo.
        /// </summary>
        /// <returns>Caminho do arquivo ou null se cancelado.</returns>
        public string? PromptSavePath()
        {
            Console.Write("\nDigite o caminho do arquivo .txt para salvar (ou Enter para cancelar): ");
            var path = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(path) ? null : path;
        }

        /// <summary>
        /// Exibe as entradas de log filtradas.
        /// </summary>
        /// <param name="entries">Entradas a exibir.</param>
        /// <param name="filterLabel">Rótulo do filtro aplicado.</param>
        public void ShowEntries(IEnumerable<LogEntry> entries, string filterLabel)
        {
            var entryList = entries?.ToList() ?? new List<LogEntry>();

            Console.WriteLine($"\n=== Entradas [{filterLabel}] ===");

            if (entryList.Count == 0)
            {
                Console.WriteLine("(Nenhuma entrada encontrada para este filtro)");
                Console.WriteLine();
                return;
            }

            foreach (var entry in entryList)
            {
                WriteColoredEntry(entry);
            }

            Console.WriteLine($"Total exibido: {entryList.Count}");
            Console.WriteLine();
        }

        /// <summary>
        /// Exibe porcentagens de cada nível.
        /// </summary>
        /// <param name="stats">Estatísticas com dados.</param>
        public void ShowPercentages(LogStatistics stats)
        {
            Console.WriteLine("\n=== PORCENTAGENS POR NÍVEL ===");
            
            if (stats.ValidEntries == 0)
            {
                Console.WriteLine("Nenhuma entrada válida para calcular porcentagens.");
                Console.WriteLine();
                return;
            }

            Console.WriteLine($"INFO:    {stats.GetPercentage("INFO"):F1}%");
            Console.WriteLine($"WARNING: {stats.GetPercentage("WARNING"):F1}%");
            Console.WriteLine($"ERROR:   {stats.GetPercentage("ERROR"):F1}%");
            Console.WriteLine();
        }

        /// <summary>
        /// Escreve uma entrada com cores baseadas no nível.
        /// </summary>
        private void WriteColoredEntry(LogEntry entry)
        {
            var originalColor = Console.ForegroundColor;

            if (!entry.IsValid)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(entry.ToString());
            }
            else
            {
                switch (entry.Level)
                {
                    case "INFO":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "WARNING":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "ERROR":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                Console.WriteLine(entry.ToString());
            }

            Console.ForegroundColor = originalColor;
        }

        /// <summary>
        /// Aguarda o usuário pressionar Enter para continuar.
        /// </summary>
        public void WaitForEnter()
        {
            Console.Write("Pressione Enter para continuar...");
            Console.ReadLine();
        }

        /// <summary>
        /// Exibe mensagem de despedida.
        /// </summary>
        public void ShowGoodbye()
        {
            Console.WriteLine("\nEncerrando o programa. Até logo!");
        }
    }
}