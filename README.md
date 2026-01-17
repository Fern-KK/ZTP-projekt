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



### Aplikacja do zarządzania notatkami i zadaniami. 
Aplikacja umożliwia użytkownikowi tworzenie, edytowanie i organizowanie notatek oraz list zadań. Notatki i zadania można opatrywać tagami (np. „praca”, „pomysł”) lub przypisywać do kategorii (np. „dom”, „projekt”). Zadania można oznaczać jako wykonane i nadawać im priorytety (np. „wysoki”, „niski”) oraz termin realizacji. Aplikacja obsługuje wyszukiwanie po słowach kluczowych, grupowanie według tagów lub kategorii, sortowanie według terminów lub priorytetów oraz generowanie raportów o zbliżających się terminach (np. „na najbliższy tydzień”) i podsumowań o wykonanych i zaległych zadaniach.




