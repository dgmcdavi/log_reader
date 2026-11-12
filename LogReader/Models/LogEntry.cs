using System;
using System.Text.RegularExpressions;

namespace LogReader.Models
{
    /// <summary>
    /// Representa uma entrada de log parseada com data, nível e mensagem.
    /// </summary>
    public class LogEntry
    {
        private static readonly Regex LogPattern = new Regex(
            @"^\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\] \[(INFO|WARNING|ERROR)\] (.+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Data e hora da entrada do log.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Nível da mensagem (INFO, WARNING, ERROR).
        /// </summary>
        public string Level { get; private set; } = string.Empty;

        /// <summary>
        /// Mensagem do log.
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// Indica se a entrada foi parseada com sucesso e é válida.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Linha original (para debug ou exibição de linhas inválidas).
        /// </summary>
        public string RawLine { get; private set; } = string.Empty;

        private LogEntry() { }

        /// <summary>
        /// Tenta parsear uma linha de log no formato esperado.
        /// Formato: [AAAA-MM-DD HH:mm:ss] [NIVEL] Mensagem
        /// </summary>
        /// <param name="line">Linha bruta do arquivo de log.</param>
        /// <param name="entry">Entrada parseada (null se inválida).</param>
        /// <returns>True se a linha é válida e foi parseada com sucesso.</returns>
        public static bool TryParse(string line, out LogEntry entry)
        {
            entry = new LogEntry { RawLine = line ?? string.Empty };

            if (string.IsNullOrWhiteSpace(line))
            {
                entry.IsValid = false;
                return false;
            }

            var match = LogPattern.Match(line.Trim());
            if (!match.Success)
            {
                entry.IsValid = false;
                return false;
            }

            if (!DateTime.TryParse(match.Groups[1].Value, out DateTime timestamp))
            {
                entry.IsValid = false;
                return false;
            }

            string level = match.Groups[2].Value.ToUpperInvariant();
            if (level != "INFO" && level != "WARNING" && level != "ERROR")
            {
                entry.IsValid = false;
                return false;
            }

            entry.Timestamp = timestamp;
            entry.Level = level;
            entry.Message = match.Groups[3].Value;
            entry.IsValid = true;

            return true;
        }

        /// <summary>
        /// Retorna a representação em string da entrada para exibição.
        /// </summary>
        public override string ToString()
        {
            if (!IsValid)
            {
                return $"[INVÁLIDA] {RawLine}";
            }
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message}";
        }
    }
}