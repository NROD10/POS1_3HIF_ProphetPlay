# Dokumentation:

## Entwicklertagebuch
---

## 20. – 24. Mai 2025
**20. Mai**  
- Initial Commit und Einrichtung des Repositories (inkl. `.gitignore`)  
  *Umsetzung: Noah & Nathan*  
- Anlage der ersten Dokumentationsdateien (Klassendiagramme, Tasks, Flussdiagramme im `docs`-Ordner)  
  *Umsetzung: Noah & Nathan*

**23. Mai**  
- Design der Startseite abgeschlossen  
  *Umsetzung: Nathan*  
- Login- und Registrierungs-Fenster in WPF erstellt und gestaltet  
  *Umsetzung: Nathan*  
- Grundlagen für API-Anbindung: Methoden für News und Ligen geschrieben (Live-Spiele abrufen, Logout/Wechsel zwischen Fenstern)  
  *Umsetzung: Nathan Noah*

**24. Mai**  
- News-Titel im `MainWindow` hinzugefügt  
  *Umsetzung: Noah*

---

## 27. Mai – 2. Juni 2025
**27. Mai**  
- Ordnerstruktur für Backend-API angelegt; erste Endpunkte skizziert  
  *Umsetzung: Nathan*

**28. Mai**  
- News-Anzeige im Hauptfenster: `ListBox` mit Doppelklick-Detailfenster implementiert  
  *Umsetzung: Noah*

**1. Juni**  
- Infobox für News-Details finalisiert, UX-Feinschliff  
  *Umsetzung: Noah*

**2. Juni**  
- Diverse Merge- und WIP-Commits zur Code-Bereinigung und Vorbereitung weiterer Features  
  *Umsetzung: Nathan*

---

## 5. – 6. Juni 2025
**5. Juni**  
- Anzeige aller Ligen im Hauptfenster aktiv; Klick öffnet spezifisches `SpieleFenster` mit Live-Matches  
  *Umsetzung: Nathan*

**6. Juni**  
- Backend mit Connexion/Swagger aufgesetzt  
  *Umsetzung: NNathan*  
- WPF-App um Login/Registrierung gegen das Swagger-Backend erweitert  
  *Umsetzung: Nathan und Noah*  
- Nächster Schritt: Supabase-Integration vorbereiten  
  *Umsetzung: Nathan und Noah*

---

## 10. – 12. Juni 2025
**10. Juni**  
- Dynamische Überschrift im `SpieleFenster` mit korrektem Ligennamen implementiert  
  *Umsetzung: Noah*

**12. Juni**  
- Benutzertabelle aufgesplittet: Separate Rollen-Tabelle eingeführt und in Login-Flow integriert  
  *Umsetzung: Nathan*

---

## 13. – 17. Juni 2025
**13. Juni**  
- Weiterentwicklung `SpieleFenster`: Anzeige von Live-Spielen nach Datumsauswahl  
  *Umsetzung: Nathan*  
- Vorbereitung Stage-Environment (Branch „stage es“)  
  *Umsetzung: Nathan und Noah*  
- Zusammenführung von Tabellenaufteilung und Login-Integration  
  *Umsetzung: Nathan und Noah*

**17. Juni**  
- Supabase-Integration für News: Erstellung neuer Einträge via REST im Swagger getestet  
  *Umsetzung: Nathan*  
- Entwurf für Lösch-Endpoint begonnen (Admin- und Eigentümer-Prüfung)  
  *Umsetzung: Noah*

**18. Juni**
- Sinvolle Admin Implementierung. Admin kann nun News erstellen und löschen, User kann diese Lesen
- Umsetzung: Nathan

# Projektplanung (Lastenheft)
Projektziel Überlegung:
Entwicklung einer Desktop-Anwendung mit moderner Oberfläche (WPF), die Sport-News und Live-Spiele verschiedener Ligen darstellt. Inklusive Benutzerverwaltung, Login-System und Admin-Funktionalitäten (News-Erstellung & -Löschung).

Setup & Grundstruktur:
Initiales Git-Repository mit .gitignore einrichten
Ordnerstruktur für Dokumentation (Klassendiagramme, Flussdiagramme etc.)

UI-Design & erste Komponenten:
Startseite-Design
Login- & Registrierungsfenster mit WPF


Backend-API:
Aufsetzen der API mit Connexion & Swagger
Endpunkte für Login/News erstellen
Supabase-Anbindung für News (CRUD über REST)
API-Tests & Fehlerbehandlung

News im Hauptfenster anzeigen (ListBox + Detailansicht)
Ligenanzeige und Spielefenster bei Klick

Benutzerverwaltung:
Rollensystem mit Admin/User (Tabellenstruktur angepasst)
Integration in Login-Flow
Rechtebasiertes Löschen & Erstellen von News


# Umsetzungsdetails (Pflichtenheft) – ProphetPlay
---
## 1. Softwarevoraussetzungen

| Komponente               | Version / Anforderung                       |
|--------------------------|---------------------------------------------|
| **Betriebssystem**       | Windows 10 64-Bit oder höher                |
| **.NET Runtime**         | .NET 6.0 (LTS)                              |
| **Visual Studio**        | 2022 Community Edition oder höher           |
| **Python**               | 3.12.x                                      |
| **Pip / Virtualenv**     | pip 22+ / virtualenv                        |
| **Flask / Connexion**    | flask 2.3+, connexion 3.0+                  |
| **Passlib**              | passlib 1.7+                                |
| **Requests**             | requests 2.28+                              |
| **Supabase**             | Supabase REST API (via HTTP, keine SDKs)    |
| **WPF / NuGet-Pakete**   | Newtonsoft.Json 13+, System.Net.Http        |
| **API-Football**         | API-Key erworben, HTTP-Zugriff              |
| **Logging**              | Python `logging`, FileHandler & StreamHandler |

---

## 2. Funktionsblöcke & Architektur

1. **WPF-Client**  
   - Login/Registrierung  
   - Anzeige und Verwaltung (CRUD) von News  
   - Liveticker & Ligenauswahl  
   - Admin-Funktionen (Benutzer- und News-Löschung)  

2. **Backend (Flask + Connexion)**  
   - **Controllers**: `default_controller.py`  
   - **Models**: `RegisterRequest`, `LoginRequest`, `LoginResponse`, `NewsItemCreate`, etc.  
   - **Data-Layer**: Supabase REST-Aufrufe via `requests`  
   - **Security**: Passworthash (PBKDF2), Rollenprüfung, Ownership-Checks  
   - **Logging**: `openapi_server/logger.py` schreibt in Log-Dateien  

3. **Supabase-Datenbank**  
   - Tabelle `users` (benutzername, passwort-hash, role_id)  
   - Tabelle `rollen` (id, name)  
   - Tabelle `news` (id, title, description, url, published_at, created_by)  

---

## 3. Detaillierte Beschreibung der Umsetzung

### 3.1 Login & Registrierung
- **Frontend**: WPF-Fenster `LoginWindow`, bindet an `AuthService.LoginAsync` / `AuthService.RegisterAsync`.
- **Backend**:  
  - `/api/benutzer/register`:  
    1. JSON → `RegisterRequest.from_dict()`  
    2. Prüfung auf bestehenden Benutzer (Supabase-Query)  
    3. Passwort-Hash mit `passlib.pbkdf2_sha256`  
    4. INSERT via Supabase REST  
  - `/api/benutzer/login`:  
    1. JSON → `LoginRequest.from_dict()`  
    2. SELECT Passwort-Hash, Rolle  
    3. Passwort-Verifikation  
    4. SELECT Rollenname, Antwort `LoginResponse`

### 3.2 News
- **Frontend**:  
  - Anzeige aller News mit `NewsService.GetAllNewsAsync()`  
  - Erstellung via `CreateNewsWindow` und `NewsService.CreateNewsAsync(dto, requester)`  
  - Löschen nur für Admin und Ersteller: Button mit `NewsService.DeleteNewsAsync(id, requester)`

- **Backend**:  
  - `/api/news` **GET**: liefert sortierte Liste aller News  
  - `/api/news` **POST**:  
    - Query-Param `requester`  
    - Body → `NewsItemCreate`  
    - Fügt `published_at` und `created_by` hinzu  
    - INSERT in Supabase  
  - `/api/news` **DELETE**:  
    1. Query-Params `id`, `requester`  
    2. Admin-Check über users.role_id  
    3. Ownership-Check (`news.created_by`)  
    4. DELETE via Supabase REST  

### 3.3 Liveticker & Ligenauswahl
- **Frontend**:  
  - `ApiFootballService.GetLeaguesAsync()`, Suche, Filter  
  - Klick öffnet `SpieleFenster` mit weiteren API-Aufrufen  
- **Backend**: keine – direkter Aufruf an externen Fußball-API

---

## 4. Mögliche Probleme & Lösungen

| Problem                                          | Ursache                                     | Lösung                                                    |
|--------------------------------------------------|---------------------------------------------|-----------------------------------------------------------|
| **Unicode-Fehler beim HTTP-Header**              | Default-Encoding `latin-1` in `requests`     | Encode Header-Values in UTF-8 oder ersetzen unzulässige Zeichen|
| **404 / 403 beim News-Löschen**                  | Fehlende `requester`-Query oder Rollencheck | Swagger-Spec & Client-Call anpassen, `?requester=` immer mitschicken |
| **`ModuleNotFoundError: No module named 'logger'`** | Falscher Import-Pfad in Models             | `from openapi_server.logger import logger` statt `from logger` |
| **WPF-Binding-Errors für Converter**             | Converter nicht in XAML-Namespace gelistet | `<local:IsAdminToVisibilityConverter/>` in `<Window.Resources>` |
| **Supabase-Timeouts / Rate-Limits**              | Viele parallele Queries                      | Batch-Requests, Caching, Query-Optimierung                   |
---

## Wie wurde die Software getestet

- **Unit-Tests**: Kernkomponenten und Logik einzeln geprüft  
- **Integrationstests**: Zusammenspiel von Frontend, Backend-API und Datenbank getestet  
- **Manuelles Testing**: Benutzerflows in der WPF-App durchgespielt (Login, News erstellen/löschen, Live-Ticker)  
- **Fehlerfälle**: Netzwerkausfälle und ungültige Eingaben simuliert
---


# Bedienungsanleitung mit Screenshots

## Login
Wenn man das Projekt starten kommt man zu Login-Fenster. Dort kann man den Benutzername und das Passwort eingeben. ![alt text](image.png)
Wenn man auf  "Noch kein Konto? Jetzt registrieren" klickt kommt man zum RegistrierFenster. Wenn man auf dem Button "Einloggen" klickt und man die richtigen Daten eingegeben hat kommt man zu der Startseite.

## Registrierung
Hier erstellt man seinen Benutzer mit entweder Admin oder User als Rolle. Beim Button "Zurück zum Login" kommt man wieder zum Login Fenster. Beim Button "Konto erstellen" erstellt man sein Konto und man kommt zu dem Login Fenster.
![alt text](image-1.png)

## Startseite
![alt text](image-2.png)
Hier Sieht man den bereich News wo man die neusten News sieht. Wenn man auf eine News doppelklickt dann kommen genauere Infos zu diesen News. ![alt text](image-3.png)
Dann sieht man die ganzen Ligen die es gibt in der Mitte. Man kann auch oben auf der Suchleiste nach einer Liga oder nach einem Land suchen und es zeigen dann diese Ligen an. ![alt text](image-4.png)
Wenn man auf eine Liga draufklickt dann kommt man zur Spielen-Fenster der jeweiligen Ligen. 
Rechts sieht man noch die Favoriten zu der aktuellen Klub-WM.
Unten rechts gibts noch den Abmelde Button. Da kommt man wieder auf die Login Seite
Wenn man sich als Admin anmeldet, werden noch alle Benutzer angezeigt die man auch löschen kann ![alt text](image-5.png). Man kann auch eigene News erstellen und löschen. ![alt text](image-6.png)

## SpieleFenster
Dort sieht man die Liga in der wir gerade sind. Man sieht die vergangenen 5 Spiele dieser Liga mit dem Endstand und wer gegeneinander gespielt hat. ![alt text](image-7.png)
Wenn man auf ein Spiel draufklickt, dann kommt ein genaueres SpielInfo Fenster mit den Liveticker.
Man sieht auch die LiveSpiele und die 10 kommenden Spiele(Falls vorhanden). ![alt text](image-8.png)

## SpielInfos
Dort sieht man dann die genauen Spielinformationen was genau im diesem Spiel passiert ist, also die Tore, Karten, Auswechlungen,... mit dem jeweiligen Namen der Spieler und des Vereins. Man sieht auch in Welcher Minute das passiert ist. ![alt text](image-9.png)

# Quellen mit denen wir gearbeitet haben:
1. https://www.api-football.com
2. https://newsapi.org
3. https://supabase.com
4. https://chatgpt.com
5. https://draw.io