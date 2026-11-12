using System;
using System.Collections.Generic;
using System.Linq;
using LogReader.Models;

namespace LogReader.Services
{
    /// <summary>
    /// Serviço responsável por filtrar e buscar entradas de log.
    /// </summary>
    public class LogFilterService
    {
        private static readonly HashSet<string> ValidLevels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "INFO", "WARNING", "ERROR", "TODAS"
        };

        /// <summary>
        /// Filtra entradas por nível específico.
        /// </summary>
        /// <param name="entries">Coleção de entradas de log.</param>
        /// <param name="level">Nível para filtrar (INFO, WARNING, ERROR, TODAS).</param>
        /// <returns>Entradas filtradas.</returns>
        public IEnumerable<LogEntry> FilterByLevel(IEnumerable<LogEntry> entries, string level)
        {
            if (entries == null)
                return Enumerable.Empty<LogEntry>();

            if (string.IsNullOrWhiteSpace(level) || level.Equals("TODAS", StringComparison.OrdinalIgnoreCase))
                return entries.Where(e => e.IsValid);

            var normalizedLevel = level.ToUpperInvariant();
            if (!ValidLevels.Contains(normalizedLevel))
                return Enumerable.Empty<LogEntry>();

            return entries.Where(e => e.IsValid && e.Level.Equals(normalizedLevel, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Filtra entradas por intervalo de datas e horário (desafio extra).
        /// </summary>
        /// <param name="entries">Coleção de entradas de log.</param>
        /// <param name="startDateTime">Data e hora inicial (inclusiva).</param>
        /// <param name="endDateTime">Data e hora final (inclusiva).</param>
        /// <returns>Entradas dentro do intervalo.</returns>
        public IEnumerable<LogEntry> FilterByDateRange(IEnumerable<LogEntry> entries, DateTime startDateTime, DateTime endDateTime)
        {
            if (entries == null)
                return Enumerable.Empty<LogEntry>();

            // Se ambas as datas têm horário 00:00:00, assumir que o usuário quer filtrar por dia inteiro
            bool isDateOnly = startDateTime.TimeOfDay == TimeSpan.Zero && endDateTime.TimeOfDay == TimeSpan.Zero;
            
            if (isDateOnly)
            {
                var startDate = DateOnly.FromDateTime(startDateTime);
                var endDate = DateOnly.FromDateTime(endDateTime);
                return entries.Where(e => e.IsValid && DateOnly.FromDateTime(e.Timestamp) >= startDate && DateOnly.FromDateTime(e.Timestamp) <= endDate);
            }

            return entries.Where(e => e.IsValid && e.Timestamp >= startDateTime && e.Timestamp <= endDateTime);
        }

        /// <summary>
        /// Filtra entradas por intervalo de datas (apenas data, ignorando horário) - compatibilidade.
        /// </summary>
        /// <param name="entries">Coleção de entradas de log.</param>
        /// <param name="startDate">Data inicial (inclusiva).</param>
        /// <param name="endDate">Data final (inclusiva).</param>
        /// <returns>Entradas dentro do intervalo.</returns>
        public IEnumerable<LogEntry> FilterByDateRange(IEnumerable<LogEntry> entries, DateOnly startDate, DateOnly endDate)
        {
            if (entries == null)
                return Enumerable.Empty<LogEntry>();

            return entries.Where(e => e.IsValid && DateOnly.FromDateTime(e.Timestamp) >= startDate && DateOnly.FromDateTime(e.Timestamp) <= endDate);
        }

        /// <summary>
        /// Busca entradas que contenham um termo na mensagem (desafio extra).
        /// </summary>
        /// <param name="entries">Coleção de entradas de log.</param>
        /// <param name="term">Termo a buscar.</param>
        /// <returns>Entradas que contêm o termo.</returns>
        public IEnumerable<LogEntry> SearchByTerm(IEnumerable<LogEntry> entries, string term)
        {
            if (entries == null || string.IsNullOrWhiteSpace(term))
                return Enumerable.Empty<LogEntry>();

            return entries.Where(e => e.IsValid && e.Message.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}