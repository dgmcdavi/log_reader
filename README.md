# Leitor de Logs em C# - Console Application

Aplicação de console desenvolvida em C#/.NET 8 para leitura, parsing e análise de arquivos de log com níveis `INFO`, `WARNING` e `ERROR`.

## 📋 Funcionalidades

- ✅ Leitura de arquivos `.log` via streaming (baixo uso de memória)
- ✅ Parsing de linhas no formato: `[AAAA-MM-DD HH:mm:ss] [NÍVEL] Mensagem`
- ✅ Contabilização de entradas por nível (INFO, WARNING, ERROR)
- ✅ Detecção e contagem de linhas inválidas
- ✅ Filtro interativo por nível de log
- ✅ **Filtro por intervalo de datas e horário** (hora/minuto/segundo opcionais)
- ✅ **Busca textual por termo** (case-insensitive)
- ✅ **Exibição de porcentagens por nível**
- ✅ **Salvar resumo em arquivo .txt**
- ✅ Tratamento robusto de erros (arquivo inexistente, sem permissão, I/O)
- ✅ Interface colorida no terminal
- ✅ Arquitetura MVC (Models, Views, Controllers, Services)

## 🚀 Como Executar
### Executável
- Ir para o arquivo "README.md" na pasta dist.
### Execução
#### Pré-requisitos
- .NET 8.0 SDK ou superior

```bash
# Clone/acesse o diretório do projeto
cd LogReader

# Compile
dotnet build

# Execute
dotnet run
```

### Exemplo de uso
```
=================================
   LEITOR DE LOGS - v1.0
=================================

Informe o caminho do arquivo .log: exemplo.log

[INFO] Lendo arquivo: exemplo.log

===== RESUMO DOS LOGS =====

INFO:       2
WARNING:    1
ERROR:      1

Entradas válidas:   4
Entradas inválidas: 2
Total de linhas:    6


===== MENU PRINCIPAL =====
1 - Filtrar por nível
2 - Buscar por termo
3 - Filtrar por intervalo de datas
4 - Exibir porcentagens por nível
5 - Salvar resumo em arquivo
0 - Sair
==========================
Escolha uma opção: 1

===== MENU DE FILTRO =====
1 - Todas as entradas
2 - INFO
3 - WARNING
4 - ERROR
0 - Voltar
==========================
Escolha uma opção: 1
```

## 📁 Estrutura do Projeto

```
LogReader/
├── Controllers/
│   └── LogController.cs      # Orquestração principal
├── Models/
│   ├── LogEntry.cs           # Modelo de entrada de log (parsing/validação)
│   └── LogStatistics.cs      # Estatísticas agregadas (porcentagens)
├── Services/
│   ├── LogFileService.cs     # Leitura/validação de arquivo (streaming)
│   └── LogFilterService.cs   # Filtros e buscas (nível, data, termo)
├── Views/
│   └── ConsoleView.cs        # Interface de terminal
├── Program.cs                # Entry point + DI manual
├── exemplo.log               # Arquivo de teste
├── exemplo-detalhado.log     # Arquivo de teste com horários
├── LogReader.csproj
└── README.md
```

## 📝 Formato de Log Suportado

```
[2026-11-02 08:15:00] [INFO] Aplicação iniciada
[2026-11-02 08:16:12] [WARNING] Espaço em disco abaixo de 20%
[2026-11-02 08:17:45] [ERROR] Falha ao conectar ao banco de dados
```

- **Níveis aceitos**: `INFO`, `WARNING`, `ERROR` (case-insensitive)
- **Data**: Formato ISO `AAAA-MM-DD HH:mm:ss`
- **Linhas inválidas**: Contabilizadas mas não interrompem processamento

---

## 🔧 Desafios Extras Implementados

### 1. Filtro por Intervalo de Datas e Horário
- **Menu**: Opção 3 → "Filtrar por intervalo de datas"
- **Formato**: `AAAA-MM-DD [HH:MM:SS]` (hora/minuto/segundo **opcionais**)
- **Comportamento**:
  - Apenas data (`2026-11-02`) → filtra dia inteiro (00:00:00 a 23:59:59)
  - Com horário (`2026-11-02 08:15:00`) → filtra intervalo exato
  - Data final opcional → usa mesma data/hora da inicial

### 2. Busca Textual por Termo
- **Menu**: Opção 2 → "Buscar por termo"
- **Busca**: Case-insensitive na mensagem
- **Exemplo**: buscar "configuração" encontra "Falha ao carregar configuração" e "Configuração recarregada"

### 3. Exibição de Porcentagens por Nível
- **Menu**: Opção 4 → "Exibir porcentagens por nível"
- **Cálculo**: `(nível / total válidas) * 100`
- **Exemplo**: INFO=2, WARNING=1, ERROR=1 → INFO: 50%, WARNING: 25%, ERROR: 25%

### 4. Salvar Resumo em Arquivo
- **Menu**: Opção 5 → "Salvar resumo em arquivo"
- **Saída**: Arquivo `.txt` com formato idêntico ao exibido no terminal

## 🛠️ Tecnologias

- **C# 12** / **.NET 8**
- **Arquitetura**: MVC + Services
- **Dependências**: Apenas .NET Standard Library (System.IO, System.Text.RegularExpressions, System.Collections.Generic)

## 📚 Documentação Detalhada

Consulte a pasta [`docs/`](docs/) para documentação completa:
- [Arquitetura](docs/arquitetura.md)
- [Models](docs/models.md)
- [Services](docs/services.md)
- [Controller](docs/controller.md)
- [View](docs/view.md)
- [Guia de Testes](docs/testes.md)

## 🧪 Testes

Execute os testes manuais com o arquivo `exemplo.log` incluído:

```bash
dotnet run
# Informe: exemplo.log
# Teste as opções 1, 2, 3, 4, 0
```

Resultado esperado:
- INFO: 2
- WARNING: 1
- ERROR: 1
- Válidas: 4
- Inválidas: 2
- Total: 6
