<div align="center">

# ♟️ SchachKI

**Schach spielen gegen eine KI**

> Das Projekt ist nicht aktuell

Ein Schulprojekt (GFS), bei dem ein KI-Modell der Schachgegner ist.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D4?style=for-the-badge&logo=windows&logoColor=white)
![OpenAI](https://img.shields.io/badge/OpenAI-412991?style=for-the-badge&logo=openai&logoColor=white)

</div>

---

## 📖 Über das Projekt

SchachKI ist eine Windows-Desktop-App, in der man Schach gegen eine KI spielt.
Statt einer Schach-Engine (wie Stockfish oder ähnliches) übernimmt hier ein KI-Modell die Züge des Gegners.

Die App kümmert sich um das Spielbrett, die Regeln und die Prüfung der Züge – die KI bekommt bei jedem Zug
die aktuelle Stellung und alle erlaubten Züge mitgeschickt und entscheidet sich für einen davon.

> Das Projekt ist im Rahmen einer **GFS** entstanden.

## ✨ Features

- 🖱️ **Drag & Drop** – Figuren einfach mit der Maus ziehen
- ✅ **Zugprüfung** – nur legale Züge sind möglich, mit Prüfung, ob der eigene König im Schach steht
- 🤖 **KI-Gegner** – angebunden über die Gemini-API (`gpt-4.1-mini`)
- 🎚️ **Drei Schwierigkeitsstufen** – `Easy`, `Normal` und `Hard`
- 🔁 **Fehlerbehandlung** – schlägt die KI einen ungültigen Zug vor, wird automatisch neu gefragt
- 📜 **Zugliste** – alle gespielten Züge werden neben dem Brett angezeigt
- 🟨 **Markierung des letzten Zugs** auf dem Brett

## 🧠 Wie funktioniert die KI?

1. Beim Start bekommt die KI eine **Systemanweisung** mit ihrer Farbe und der Schwierigkeit.
2. Nach jedem Zug des Spielers wird ein **Prompt** mit allen bisherigen Zügen und den legalen Zügen geschickt.
3. Die KI antwortet mit genau einem Zug im Format `von` + `nach` (z. B. `g8f6`).
4. Die App prüft den Zug mit der eigenen Regel-Logik und führt ihn auf dem Brett aus.

## 🚧 Bekannte Einschränkungen

Da es sich um ein Schulprojekt handelt, sind noch nicht alle Schachregeln umgesetzt:

- Rochade, En passant und Bauernumwandlung fehlen noch
- Das Spiel endet, wenn ein König geschlagen wird (kein klassisches Schachmatt / Patt)
- Farbe und Schwierigkeit sind derzeit fest im Code eingestellt
  
---

<div align="center">
<sub>Erstellt als Schulprojekt · Made with C# & ☕</sub>
</div>
