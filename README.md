# RunExeForSQL
# Simple C# assembly for running commands and programs (exe) from MS SQL

Zamieszczona biblioteka umożliwia wywołanie dowolnego programu (komendy) 
i przekazanie wynikowego ExitCode (kodu zwrotnego wykonania programu) do środowiska Microsoft SQL Server.

Procedury mają następujące parametry:

    path - ścieżka (nazwa) programu, który zostanie uruchomiony
    args - ciąg znaków przekazany jako argumenty do programu
    timeOut - liczba milisekund. 
              Jeżeli po tym czasie program nie zakończy się jego wykonanie zostanie przerwane
              i zwrócony zostanie kod -201.

Występują dwie procedury:

### Run(string path, string args, int timeOut)

`public static int Run(string path, string args, int timeOut)`

Uruchamia program (path) z argumentami (args) na czas nie większy niż (timeOut). Zwraca kod wykonania lub -201.

### Runs(string path, string args, int timeOut)

`public static string Runs(string path, string args, int timeOut)`

Uruchamia program (path) z argumentami (args) na czas nie większy niż (timeOut). 
Zwraca kod wykonania jako ciąg znaków lub przyczynę niepowodzenia jako wartość ciągu znaków wyjątku wykonania.


## Sposób użycia w środowisku MS SQL

### Zarejestruj funkcję wywołania

    -- select the database
    USE [CentralStorage]
    GO
    -- DROP FUNCTION [dbo].[Run]
    -- GO
    -- DROP FUNCTION [dbo].[Runs]
    -- GO
    -- DROP ASSEMBLY [IDExec]
    -- GO
    
    sp_configure 'clr enabled', 1;
    GO
    reconfigure
    GO
    sp_dbcmptlevel 'CentralStorage', 90
    GO
    ALTER DATABASE CentralStorage SET TRUSTWORTHY ON
    GO
    USE [CentralStorage]
    GO
    CREATE ASSEMBLY [IDExec]
    FROM 'C:\Work\IDExec.dll' -- ścieżka do pliku biblioteki dostępna dla serwera SQL
    WITH PERMISSION_SET = UNSAFE
    GO
    CREATE FUNCTION [dbo].Run(@path nvarchar(max), @args nvarchar(max), @timeOut int)
    RETURNS int AS
    EXTERNAL NAME IDExec.[Arhat.IDExec.CLR].Run
    GO
    CREATE FUNCTION [dbo].Runs(@path nvarchar(max), @args nvarchar(max), @timeOut int)
    RETURNS nvarchar(max) AS
    EXTERNAL NAME IDExec.[Arhat.IDExec.CLR].Runs
    GO

### Przykładowe wywołania

`select dbo.Run('ping','8.8.8.8',10000)' 

Jeżeli ping będzie w czasie 10 sek w stanie wykonać połączenia to kod wykonania będzie miał wartość 0 inaczej -201.

`select dbo.Run('notepad.exe','',1000)' 

Zawsze zwróci -201 ponieważ notatnik automatycznie nie kończy działania.



