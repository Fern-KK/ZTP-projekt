#### Aby wysztko z Avalonią działało trzeba dodać do VSCode takie rozszerzenia:
- Avalonia Templates
- Avalonia for VSCode



#### Podzieliłem wcześniejszt kod na 3 pliki: 
- Composite.cs
- Builder.cs
- GlobalGroups.cs

#### Istotne pliki Avalonii:
- MainWindow.axaml.cs - to jak plik js w stronach internetowych. Tak funkcjonalnie jest Program.cs naszego programu, gdzie MainWindow() jest odpowiednikiem normalnego Main()
- MainWindow.axaml - To jak plik html w stronach internetowej
- App.axaml - to jak plik css, gdzie definuje style 

aby odpalić trzeba zrobić run któregokolwiek z plików .cs


dotnet publish -c Release -r win-x64 --self-contained true  -p:PublishSingleFile=true  -p:EnableCompressionInSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
