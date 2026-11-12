# LogReader - Executáveis Multiplataforma

Este diretório contém os executáveis auto-contidos do projeto **LogReader** para Linux e Windows.

## 📁 Estrutura dos Arquivos

```
dist/
├── LogReader-linux          # Executável para Linux (x64)
├── LogReader-windows.exe    # Executável para Windows (x64)
└── README.md                # Este arquivo
```

## 🚀 Como Executar

### Linux
```bash
# Dar permissão de execução (se necessário)
chmod +x LogReader-linux

# Executar
./LogReader-linux
```

### Windows
```cmd
# No PowerShell ou CMD
LogReader-windows.exe
```

> **Nota**: Ambos os executáveis são **auto-contidos (self-contained)** — não requerem instalação do .NET Runtime no sistema alvo. Incluem tudo o que é necessário para rodar.

---

## 🛠️ Como Foram Gerados

### Pré-requisitos
- .NET SDK 10.0+ instalado (`dotnet --version` → `10.0.114`)

### Comandos Utilizados

#### 1. Build para Linux (x64)
```bash
cd /home/davi/projects/aaa_agent/projeto_unip/LogReader
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

#### 2. Build para Windows (x64)
```bash
cd /home/davi/projects/aaa_agent/projeto_unip/LogReader
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

### Explicação das Flags

| Flag | Descrição |
|------|-----------|
| `-c Release` | Compila em modo Release (otimizado, sem símbolos de debug) |
| `-r linux-x64` / `-r win-x64` | **Runtime Identifier (RID)** — define o SO e arquitetura alvo |
| `--self-contained true` | Inclui o .NET Runtime no executável (não precisa do .NET instalado na máquina alvo) |
| `-p:PublishSingleFile=true` | Empacota tudo em **um único arquivo executável** |
| `-p:PublishTrimmed=true` | Remove assemblies não utilizados (reduz tamanho do binário) |

### Saída dos Builds

Os arquivos são gerados em:
- **Linux**: `bin/Release/net10.0/linux-x64/publish/LogReader`
- **Windows**: `bin/Release/net10.0/win-x64/publish/LogReader.exe`

Foram copiados para esta pasta `dist/` com nomes mais claros:
- `LogReader-linux`
- `LogReader-windows.exe`

---

## 📋 Funcionalidades do LogReader

O programa lê arquivos de log no formato:
```
[AAAA-MM-DD HH:mm:ss] [NIVEL] Mensagem
```

Exemplo:
```
[2024-01-15 10:30:45] [INFO] Application started
[2024-01-15 10:31:02] [WARNING] High memory usage detected
[2024-01-15 10:32:10] [ERROR] Database connection failed
```

### Menu Principal
1. **Filtrar por nível** — INFO, WARNING, ERROR ou Todas
2. **Buscar por termo** — Pesquisa textual nas mensagens (case-insensitive)
3. **Filtrar por intervalo de datas** — Data apenas ou data+hora
4. **Exibir porcentagens por nível** — % de cada nível sobre o total válido
5. **Salvar resumo em arquivo** — Exporta estatísticas para .txt
0. **Sair**

---

## ✅ Teste Rápido

Crie um arquivo de teste:
```bash
cat > /tmp/test.log << 'EOF'
[2024-01-15 10:30:45] [INFO] Application started
[2024-01-15 10:31:02] [WARNING] High memory usage detected
[2024-01-15 10:32:10] [ERROR] Database connection failed
[2024-01-15 10:33:22] [INFO] Retrying connection
[2024-01-15 10:33:25] [INFO] Connection restored
[2024-01-15 10:35:00] [WARNING] Disk space low
[2024-01-15 10:36:15] [ERROR] Failed to write to log file
[2024-01-15 10:37:30] [INFO] Cleanup completed
EOF
```

Execute (Linux):
```bash
echo -e "/tmp/test.log\n0" | ./LogReader-linux
```

Saída esperada:
```
===== RESUMO DOS LOGS =====

INFO:       4
WARNING:    2
ERROR:      2

Entradas válidas:   8
Entradas inválidas: 0
Total de linhas:    8
```

---

## ⚠️ Notas Importantes

1. **Arquitetura**: Ambos compilados para **x64 (64-bit)**. Não rodam em sistemas 32-bit ou ARM sem recompilação.
2. **GLIBC (Linux)**: Requer GLIBC 2.28+ (presente na maioria das distros modernas: Ubuntu 20.04+, Debian 10+, Fedora 30+, etc.)
3. **Windows**: Requer Windows 10+ / Windows Server 2016+
4. **Tamanho**: ~15 MB (Linux) / ~13 MB (Windows) — típico para self-contained single-file com trimming
5. **Primeira execução**: Pode ser ligeiramente mais lenta devido à extração do bundle single-file em memória

---

## 🔧 Recompilar para Outras Arquiteturas

### Linux ARM64 (ex: Raspberry Pi 4, AWS Graviton)
```bash
dotnet publish -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

### Windows ARM64
```bash
dotnet publish -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

### macOS (Intel x64)
```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

### macOS (Apple Silicon ARM64)
```bash
dotnet publish -c Release -r osx-arm64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

> Liste todos os RIDs suportados: `dotnet runtime-info --list-rids`

---

## 📜 Licença

Mesma licença do projeto original (ver `../LICENSE` ou `../README.md` na raiz do repositório).

---

**Gerado em**: 2026-11-12  
**.NET SDK**: 10.0.114  
**Projeto**: LogReader (net10.0)