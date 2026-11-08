# SchachKI

Schach gegen ein KI-Modell statt gegen eine normale Schach-Engine. Hab ich 2025 als GFS in der Schule gemacht, das Projekt ist abgeschlossen.

Windows-App in C# mit .NET 8 und Windows Forms. Brett, Regeln und Zugprüfung sind selbst programmiert. Nach jedem Zug bekommt das Modell (OpenAI, `gpt-4.1-mini`) die bisherigen Züge, die Stellung als FEN und alle erlaubten Züge und sucht sich einen davon aus. Schlägt es einen ungültigen Zug vor, wird einfach nochmal gefragt. Es gibt drei Schwierigkeiten (Easy, Normal, Hard), die Figuren zieht man per Drag & Drop.

Was noch fehlt: Rochade, En passant und Bauernumwandlung. Das Spiel endet, wenn ein König geschlagen wird, ein richtiges Schachmatt gibt es nicht. Farbe und Schwierigkeit stehen fest im Code.

Starten: `OPENAI_API_KEY` als Umgebungsvariable setzen, `SchachKI.sln` in Visual Studio öffnen und laufen lassen.
