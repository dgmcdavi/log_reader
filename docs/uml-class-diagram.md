# Diagrama UML de Classes - LogReader

## Visão Geral

Este diagrama representa a estrutura de classes do projeto **LogReader**.

---

## Diagrama de Classes (Mermaid)

```mermaid
classDiagram
    %% ===================== MODELS =====================
    class LogEntry {
        -static Regex LogPattern
        -DateTime Timestamp
        -string Level
        -string Message
        -bool IsValid
        -string RawLine
        +LogEntry()
        +static bool TryParse(string line, out LogEntry entry)
        +override string ToString()
        +DateTime Timestamp {get}
        +string Level {get}
        +string Message {get}
        +bool IsValid {get}
        +string RawLine {get}
    }

    class LogStatistics {
        -Dictionary<string, int> _levelCounts
        +int InfoCount {get}
        +int WarningCount {get}
        +int ErrorCount {get}
        +int ValidEntries {get}
        +int InvalidEntries {get}
        +int TotalLines {get}
        +void Increment(LogEntry entry)
        +double GetPercentage(string level)
        +override string ToString()
    }

    %% ===================== SERVICES =====================
    class LogFileService {
        +bool ValidateFilePath(string filePath, out string errorMessage)
        +IEnumerable~LogEntry~ ReadLogFile(string filePath)
    }

    class LogFilterService {
        -static HashSet<string> ValidLevels
        +IEnumerable~LogEntry~ FilterByLevel(IEnumerable~LogEntry~ entries, string level)
        +IEnumerable~LogEntry~ FilterByDateRange(IEnumerable~LogEntry~ entries, DateTime startDateTime, DateTime endDateTime)
        +IEnumerable~LogEntry~ FilterByDateRange(IEnumerable~LogEntry~ entries, DateOnly startDate, DateOnly endDate)
        +IEnumerable~LogEntry~ SearchByTerm(IEnumerable~LogEntry~ entries, string term)
    }

    %% ===================== CONTROLLERS =====================
    class LogController {
        -LogFileService _fileService
        -LogFilterService _filterService
        -ConsoleView _view
        -IEnumerable~LogEntry~ _allEntries
        -LogStatistics _statistics
        +LogController(LogFileService, LogFilterService, ConsoleView)
        +void Run()
        -string GetValidFilePath()
        -bool LoadAndProcessFile(string filePath)
        -LogStatistics CalculateStatistics(IEnumerable~LogEntry~ entries)
        -void RunMainMenuLoop()
        -void RunLevelFilterLoop()
        -void RunSearchByTerm()
        -void RunFilterByDateRange()
        -void ShowPercentages()
        -void SaveSummaryToFile()
    }

    %% ===================== VIEWS =====================
    class ConsoleView {
        -const string MainMenuOptions
        -const string LevelFilterOptions
        +void ShowWelcome()
        +string PromptFilePath()
        +void ShowError(string message)
        +void ShowInfo(string message)
        +void ShowSuccess(string message)
        +void ShowSummary(LogStatistics stats)
        +void ShowMainMenu()
        +void ShowLevelFilterMenu()
        +string PromptMainMenuChoice()
        +string PromptLevelFilterChoice()
        +string? MapLevelChoiceToLevel(string choice)
        +string? PromptSearchTerm()
        +(DateTime start, DateTime end)? PromptDateRange()
        +string? PromptSavePath()
        +void ShowEntries(IEnumerable~LogEntry~ entries, string filterLabel)
        +void ShowPercentages(LogStatistics stats)
        -void WriteColoredEntry(LogEntry entry)
        +void WaitForEnter()
        +void ShowGoodbye()
    }

    %% ===================== PROGRAM =====================
    class Program {
        +static int Main(string[] args)
    }

    %% ===================== RELATIONSHIPS =====================
    %% Composition/Aggregation
    LogController *-- LogFileService : uses
    LogController *-- LogFilterService : uses
    LogController *-- ConsoleView : uses
    LogController *-- LogEntry : manages (collection)
    LogController *-- LogStatistics : creates/uses
    
    LogStatistics --> LogEntry : aggregates
    LogFileService --> LogEntry : creates (TryParse)
    LogFilterService --> LogEntry : filters
    
    ConsoleView --> LogEntry : displays
    ConsoleView --> LogStatistics : displays
    
    Program --> LogController : creates
    Program --> LogFileService : creates
    Program --> LogFilterService : creates
    Program --> ConsoleView : creates

    %% Dependencies
    LogEntry ..> System.Text.RegularExpressions.Regex : uses
    LogStatistics ..> System.Collections.Generic.Dictionary : uses
    LogFileService ..> System.IO.File : uses
    LogFilterService ..> System.Linq.Enumerable : uses
    ConsoleView ..> System.Console : uses
```