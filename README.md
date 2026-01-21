## Aplikacja do zarządzania notatkami i zadaniami. 
Aplikacja umożliwia użytkownikowi tworzenie, edytowanie i organizowanie notatek oraz list zadań. Notatki i zadania można opatrywać tagami (np. „praca”, „pomysł”) lub przypisywać do kategorii (np. „dom”, „projekt”). Zadania można oznaczać jako wykonane i nadawać im priorytety (np. „wysoki”, „niski”) oraz termin realizacji. Aplikacja obsługuje wyszukiwanie po słowach kluczowych, grupowanie według tagów lub kategorii, sortowanie według terminów lub priorytetów oraz generowanie raportów o zbliżających się terminach (np. „na najbliższy tydzień”) i podsumowań o wykonanych i zaległych zadaniach.

Aplikacja wykorzystuje [NotesUserServer](https://github.com/hiko667/NotesUserServer) do zapisu danych online.
## Użyte wzorce:
### Composite
Tworzy obiekty Note, Task, TaskList, Group
### Builder
pomaga w budowie Note, Task, TaskList,
### Visitor
odwiedza obiekty composite i przeprowadza na nich operacje bez ingerencji w ich stróktórę (głównie zlicza rzeczy do statystyk lub pomaga w wyszukiwaniu)
## Autorzy
- [@Fern-KK](https://github.com/Fern-KK)
- [@hiko667](https://github.com/hiko667) 
- [@Marchewer](https://github.com/Marchewer)





dotnet publish -c Release -r win-x64 --self-contained true  -p:PublishSingleFile=true  -p:EnableCompressionInSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

