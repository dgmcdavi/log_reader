using System;
using System.Collections.Generic;
using System.IO;
using LogReader.Models;

namespace LogReader.Services
{
    /// <summary>
    /// Serviço responsável pela leitura e processamento de arquivos de log.
    /// Utiliza streaming (File.ReadLines) para não carregar todo o conteúdo na memória.
    /// </summary>
    public class LogFileService
    {
        /// <summary>
        /// Valida se o caminho do arquivo existe e é acessível.
        /// </summary>
        /// <param name="filePath">Caminho do arquivo a validar.</param>
        /// <param name="errorMessage">Mensagem de erro se inválido.</param>
        /// <returns>True se o arquivo é válido e acessível.</returns>
        public bool ValidateFilePath(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                errorMessage = "Caminho do arquivo não informado.";
                return false;
            }

            if (!File.Exists(filePath))
            {
                errorMessage = $"Arquivo não encontrado: {filePath}";
                return false;
            }

            try
            {
                // Tenta abrir para leitura para verificar permissões
                using (File.OpenRead(filePath)) { }
            }
            catch (UnauthorizedAccessException)
            {
                errorMessage = $"Sem permissão para ler o arquivo: {filePath}";
                return false;
            }
            catch (IOException ex)
            {
                errorMessage = $"Erro de acesso ao arquivo: {ex.Message}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Lê o arquivo de log linha por linha (streaming) e parseia cada entrada.
        /// </summary>
        /// <param name="filePath">Caminho do arquivo .log.</param>
        /// <returns>Lista de entradas parseadas (válidas e inválidas).</returns>
        /// <exception cref="FileNotFoundException">Arquivo não encontrado.</exception>
        /// <exception cref="UnauthorizedAccessException">Sem permissão de leitura.</exception>
        /// <exception cref="IOException">Erro de I/O durante leitura.</exception>
        public IEnumerable<LogEntry> ReadLogFile(string filePath)
        {
            if (!ValidateFilePath(filePath, out string errorMessage))
            {
                throw new FileNotFoundException(errorMessage);
            }

            var entries = new List<LogEntry>();

            foreach (var line in File.ReadLines(filePath))
            {
                LogEntry.TryParse(line, out var entry);
                entries.Add(entry);
            }

            return entries;
        }
    }
}