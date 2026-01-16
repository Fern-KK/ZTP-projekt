### Aplikacja do zarządzania notatkami i zadaniami. 
Aplikacja umożliwia użytkownikowi tworzenie, edytowanie i organizowanie notatek oraz list zadań. Notatki i zadania można opatrywać tagami (np. „praca”, „pomysł”) lub przypisywać do kategorii (np. „dom”, „projekt”). Zadania można oznaczać jako wykonane i nadawać im priorytety (np. „wysoki”, „niski”) oraz termin realizacji. Aplikacja obsługuje wyszukiwanie po słowach kluczowych, grupowanie według tagów lub kategorii, sortowanie według terminów lub priorytetów oraz generowanie raportów o zbliżających się terminach (np. „na najbliższy tydzień”) i podsumowań o wykonanych i zaległych zadaniach.




dotnet publish -c Release -r win-x64 --self-contained true  -p:PublishSingleFile=true  -p:EnableCompressionInSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

