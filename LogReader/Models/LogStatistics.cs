using System;
using System.Collections.Generic;

namespace LogReader.Models
{
    /// <summary>
    /// Mantém estatísticas agregadas sobre as entradas de log processadas.
    /// </summary>
    public class LogStatistics
    {
        private readonly Dictionary<string, int> _levelCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "INFO", 0 },
            { "WARNING", 0 },
            { "ERROR", 0 }
        };

        /// <summary>
        /// Quantidade de entradas INFO.
        /// </summary>
        public int InfoCount => _levelCounts["INFO"];

        /// <summary>
        /// Quantidade de entradas WARNING.
        /// </summary>
        public int WarningCount => _levelCounts["WARNING"];

        /// <summary>
        /// Quantidade de entradas ERROR.
        /// </summary>
        public int ErrorCount => _levelCounts["ERROR"];

        /// <summary>
        /// Total de entradas válidas (reconhecidas).
        /// </summary>
        public int ValidEntries { get; private set; }

        /// <summary>
        /// Total de entradas inválidas (não reconhecidas).
        /// </summary>
        public int InvalidEntries { get; private set; }

        /// <summary>
        /// Total de linhas processadas (válidas + inválidas).
        /// </summary>
        public int TotalLines => ValidEntries + InvalidEntries;

        /// <summary>
        /// Incrementa os contadores com base em uma entrada de log.
        /// </summary>
        /// <param name="entry">Entrada de log a ser contabilizada.</param>
        public void Increment(LogEntry entry)
        {
            if (entry == null)
            {
                InvalidEntries++;
                return;
            }

            if (entry.IsValid)
            {
                ValidEntries++;
                if (_levelCounts.ContainsKey(entry.Level))
                {
                    _levelCounts[entry.Level]++;
                }
            }
            else
            {
                InvalidEntries++;
            }
        }

        /// <summary>
        /// Calcula a porcentagem de um nível específico em relação ao total de entradas válidas.
        /// </summary>
        /// <param name="level">Nível (INFO, WARNING, ERROR).</param>
        /// <returns>Porcentagem (0-100) ou 0 se não houver entradas válidas.</returns>
        public double GetPercentage(string level)
        {
            if (ValidEntries == 0)
                return 0;

            if (!_levelCounts.ContainsKey(level.ToUpperInvariant()))
                return 0;

            return (_levelCounts[level.ToUpperInvariant()] / (double)ValidEntries) * 100;
        }

        /// <summary>
        /// Retorna a representação formatada do resumo conforme especificação.
        /// </summary>
        public override string ToString()
        {
            return $@"===== RESUMO DOS LOGS =====

INFO:       {InfoCount}
WARNING:    {WarningCount}
ERROR:      {ErrorCount}

Entradas válidas:   {ValidEntries}
Entradas inválidas: {InvalidEntries}
Total de linhas:    {TotalLines}";
        }
    }
}