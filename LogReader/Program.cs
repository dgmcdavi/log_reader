using LogReader.Controllers;
using LogReader.Services;
using LogReader.Views;

namespace LogReader
{
    /// <summary>
    /// Ponto de entrada da aplicação Leitor de Logs.
    /// Configura injeção de dependências e inicia o controlador principal.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Método principal - inicia a aplicação.
        /// </summary>
        /// <param name="args">Argumentos da linha de comando (não utilizados).</param>
        public static int Main(string[] args)
        {
            try
            {
                // Composição de dependências (Manual DI)
                var fileService = new LogFileService();
                var filterService = new LogFilterService();
                var view = new ConsoleView();
                var controller = new LogController(fileService, filterService, view);

                // Executa a aplicação
                controller.Run();

                return 0; // Sucesso
            }
            catch (Exception ex)
            {
                // Fallback para erros não tratados no controller
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERRO CRÍTICO] {ex.Message}");
                Console.ResetColor();
                return 1; // Erro
            }
        }
    }
}