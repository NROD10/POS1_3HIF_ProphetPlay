# Entwicklertagebuch
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