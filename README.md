# CalculatorWindows

Aplicație desktop de tip calculator, inspirată de Calculatorul din Windows, realizată în **C# / WPF** pe **.NET 8**.
Aplicația oferă două moduri de lucru: **Standard** și **Programmer**.

## Funcționalități

### Modul Standard
- Operații aritmetice de bază: adunare `+`, scădere `-`, înmulțire `×`, împărțire `÷`
- Funcții: procent `%`, invers `1/x`, pătrat `x²`, radical `√x`, schimbare semn `±`, zecimale `.`
- Ștergere: `C` (tot), `CE` (intrarea curentă), `⌫` (ultimul caracter)
- Memorie: `MC`, `MR`, `M+`, `M-`, `MS` și `M▾` (vizualizarea valorilor salvate)
- Meniul **File**:
  - **Copy / Cut / Paste** – lucru cu clipboard-ul (disponibile și din meniul contextual al afișajului)
  - **Data Grouping** – afișează numărul cu separatori de mii
  - **Cascade Operation** – când este activ, rezultatul intermediar se calculează la fiecare operator apăsat
- Meniul **Help → About**

### Modul Programmer
- Lucru în bazele **HEX**, **DEC**, **OCT** și **BIN**, cu afișarea simultană a valorii în toate cele patru baze
- Butoanele de cifre invalide pentru baza curentă sunt dezactivate automat (ex. `2`–`9` și `A`–`F` în BIN)
- Cifre hexazecimale `A`–`F`
- Operații: `+`, `-`, `×`, `÷`, `%` (modulo), evaluate cu respectarea priorității operatorilor
- Memorie: `MS` (salvare) și `Mv` (vizualizare)
- Copy / Cut / Paste

### Comutarea între moduri
Modul se alege din lista derulantă din colțul stânga-sus al ferestrei.

## Comenzi de la tastatură

| Tastă | Acțiune |
|-------|---------|
| `0`–`9` (inclusiv NumPad) | Introducere cifre |
| `A`–`F` | Cifre hexazecimale (doar în modul Programmer) |
| `+` `-` `*` `/` | Operatori |
| `Enter` | Calculează rezultatul (`=`) |
| `Backspace` | Șterge ultimul caracter |
| `Esc` | Șterge tot |
| `.` | Separator zecimal (modul Standard) |
| `Ctrl+C` / `Ctrl+X` / `Ctrl+V` | Copy / Cut / Paste |

## Setări

Aplicația salvează automat o parte din setări în fișierul `calculator_settings.json`, în directorul de lucru:
- ultimul mod folosit
- ultima bază numerică folosită în modul Programmer
- starea opțiunii Data Grouping

## Cerințe

- Windows
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (opțional) Visual Studio 2022 cu workload-ul „.NET desktop development”

## Compilare și rulare

Cu Visual Studio: deschideți `CalculatorWindows.sln` și apăsați **F5**.

Din linia de comandă:

```bash
dotnet restore
dotnet build
dotnet run --project CalculatorWindows/CalculatorWindows.csproj
```

## Structura proiectului

```
CalculatorWindows/
├── CalculatorWindows.sln
└── CalculatorWindows/
    ├── App.xaml / App.xaml.cs              # Punctul de intrare al aplicației
    ├── MainWindow.xaml / .xaml.cs          # Interfața și logica modului Standard
    ├── Programmer.xaml / .xaml.cs          # Interfața și logica modului Programmer
    ├── CalculatorOperations.cs             # Evaluarea expresiilor și funcțiile matematice
    ├── MemoryManager.cs                    # Gestionarea memoriei (MS, MR, M+, M-, MC)
    ├── SettingsManager.cs                  # Salvarea / încărcarea setărilor în JSON
    ├── CalculatorWindows.cs                # Modelul de setări (CalculatorSettings)
    └── ModeSelectorManager.cs              # Logica de comutare între moduri
```

## Tehnologii

- C# / WPF (.NET 8, `net8.0-windows`)
- [Newtonsoft.Json](https://www.newtonsoft.com/json) 13.0.3 – pentru serializarea setărilor

## Limitări cunoscute

- În modul Programmer, butoanele `←`, `→`, `(` și `)` nu au încă funcționalitate.
- Opțiunea **Data Grouping** este implementată doar în modul Standard.

## Autor

Munteanu Marius-Iulian
