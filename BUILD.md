# Build Instructions

## Quick Start

### Using .NET CLI

1. **Navigate to project directory:**
   ```bash
   cd "c:\Users\motyl\ADDRESS FILLER\REVERSE-ENGINEER\SecureNotePro"
   ```

2. **Build Debug version:**
   ```bash
   dotnet build
   ```

3. **Build Release version (for RE practice):**
   ```bash
   dotnet build -c Release
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

### Using Visual Studio

1. Open `SecureNotePro.csproj` in Visual Studio 2022
2. Select Build > Build Solution (or press Ctrl+Shift+B)
3. Run with F5 (Debug) or Ctrl+F5 (Release)

## Output Locations

- **Debug Build:** `bin\Debug\net8.0-windows\SecureNotePro.exe`
- **Release Build:** `bin\Release\net8.0-windows\SecureNotePro.exe`

## For Reverse Engineering Practice

The **Release build** is recommended for reverse engineering practice because:
- No debug symbols included
- Code is optimized
- More realistic challenge
- Smaller binary size

Build command:
```bash
dotnet build -c Release
```

Then analyze:
```
bin\Release\net8.0-windows\SecureNotePro.exe
```

## Prerequisites

- .NET 8.0 SDK or later
- Windows 10/11
- Visual Studio 2022 (optional, for IDE development)

Check .NET version:
```bash
dotnet --version
```

If you don't have .NET 8.0, download from: https://dotnet.microsoft.com/download
