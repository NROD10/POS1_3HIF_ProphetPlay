# API-Kommunikation
import requests
# Datenmodelle, die aus dem Swagger-YAML automatisch erzeugt wurden
from openapi_server.models.register_request import RegisterRequest
from openapi_server.models.login_request import LoginRequest
# Flask-Zugriff auf HTTP-Anfragen
from flask import jsonify, request
# Sicheres Passwort-Hashing
from passlib.hash import pbkdf2_sha256 as crypto
# Für Fehlerausgabe im Terminal
import traceback

# Supabase-Zugangsdaten
SUPABASE_URL = "https://qccwfrohegoiaizozyks.supabase.co"
SUPABASE_API_KEY = "eyJhbGciOi..."  # ← API-Key (nicht öffentlich teilen!)

# HTTP-Header für jede Supabase-Anfrage
SUPABASE_HEADERS = {
    "apikey":        SUPABASE_API_KEY,
    "Authorization": f"Bearer {SUPABASE_API_KEY}",
    "Content-Type":  "application/json"
}


# Registrierung neuer Benutzer
def api_benutzer_register_post(body):
    try:
        # Aus JSON wird ein Python-Objekt gemacht
        body = RegisterRequest.from_dict(body)

        # Prüfen, ob der Benutzername schon existiert
        query_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{body.benutzername}"
        check_resp = requests.get(query_url, headers=SUPABASE_HEADERS)
        print(">> REGISTER-EXIST-RESP:", check_resp.status_code, check_resp.text)
        if check_resp.status_code != 200:
            return "Fehler beim Überprüfen des Benutzers", 500
        if check_resp.json():
            return "Benutzername existiert bereits", 409

        # Passwort sicher hashen
        hashed = crypto.hash(body.passwort)

        # Benutzer wird in Supabase gespeichert
        insert_url = f"{SUPABASE_URL}/rest/v1/users"
        payload = {
            "benutzername": body.benutzername,
            "passwort":     hashed,
            "role_id":      body.role_id
        }
        insert_resp = requests.post(insert_url, json=payload, headers=SUPABASE_HEADERS)
        print(">> REGISTER-INSERT-RESP:", insert_resp.status_code, insert_resp.text)

        # Erfolg oder Fehler zurückgeben
        if insert_resp.status_code in (200, 201):
            return "Registrierung erfolgreich", 200
        else:
            return insert_resp.text, insert_resp.status_code

    except Exception as e:
        print("Ausnahme bei Registrierung:", e)
        return "Fehler bei Registrierung", 500


# Benutzer-Login
def api_benutzer_login_post(body):
    try:
        body = LoginRequest.from_dict(body)
        benutzername = body.benutzername
        passwort     = body.passwort

        # Benutzer aus DB abrufen
        users_url = (
            f"{SUPABASE_URL}/rest/v1/users"
            f"?select=benutzername,passwort,role_id"
            f"&benutzername=eq.{benutzername}"
        )
        users_resp = requests.get(users_url, headers=SUPABASE_HEADERS)
        print(">> LOGIN-USERS-URL:", users_url)
        print(">> LOGIN-USERS-RESP:", users_resp.status_code, users_resp.text)
        if users_resp.status_code != 200:
            return "Fehler bei Anmeldung", 500

        users = users_resp.json()
        if not users:
            return "Ungültige Anmeldedaten", 401

        record = users[0]
        stored_hash = record.get("passwort")
        role_id     = record.get("role_id")

        # Passwort prüfen
        if not stored_hash or not crypto.verify(passwort, stored_hash):
            return "Ungültige Anmeldedaten", 401

        # Rolle holen (Admin/User)
        role_url = f"{SUPABASE_URL}/rest/v1/rollen?select=name&id=eq.{role_id}"
        role_resp = requests.get(role_url, headers=SUPABASE_HEADERS)
        print(">> LOGIN-ROLE-URL:", role_url)
        print(">> LOGIN-ROLE-RESP:", role_resp.status_code, role_resp.text)
        if role_resp.status_code != 200:
            return "Fehler bei Anmeldung", 500
        roles = role_resp.json()
        rolle_name = roles[0]["name"] if roles else "User"

        # Login erfolgreich → Daten zurückgeben
        return {
            "benutzername": benutzername,
            "rolle": rolle_name
        }, 200

    except Exception as e:
        print("Ausnahme bei Login:", e)
        return "Fehler bei Anmeldung", 500


# Benutzerliste abrufen (nur Admins)
def api_benutzer_liste_get():
    try:
        requester = request.args.get("requester")  # der Anfragende Benutzername

        # Prüfen, ob requester ein Admin ist
        user_url = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        user_resp = requests.get(user_url, headers=SUPABASE_HEADERS)

        if user_resp.status_code != 200:
            return "Fehler beim Abrufen der Anfrageperson", 500

        users = user_resp.json()
        if not users:
            return "Benutzer nicht gefunden", 404

        role_id = users[0]["role_id"]
        if role_id != 1:
            return "Nicht autorisiert", 403  # kein Admin

        # Alle Benutzer abrufen
        all_users_url = f"{SUPABASE_URL}/rest/v1/users?select=benutzername,role_id"
        all_users_resp = requests.get(all_users_url, headers=SUPABASE_HEADERS)

        if all_users_resp.status_code != 200:
            return "Fehler beim Abrufen der Benutzer", 500

        return all_users_resp.json(), 200

    except Exception as e:
        print("Fehler beim Benutzerlisten-Abruf:", e)
        return "Serverfehler", 500


# Benutzer löschen (nur Admins)
def api_benutzer_loeschen_delete():
    try:
        requester = request.args.get("requester")  # wer löscht
        target    = request.args.get("target")     # wer soll gelöscht werden
        print(f">> DELETE request: requester={requester}, target={target}")

        if not requester or not target:
            return "Fehlende Parameter", 400

        # Admin-Prüfung
        check_url = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        resp = requests.get(check_url, headers=SUPABASE_HEADERS)
        print(">> ROLLE-Ausgabe:", resp.status_code, resp.text)
        data = resp.json() or []
        role_id = data[0].get("role_id") if data else None

        if role_id != 1:
            return "Nicht autorisiert", 403

        # DELETE-Anfrage an Supabase
        delete_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{target}"
        headers = {**SUPABASE_HEADERS, "Prefer": "return=minimal"}
        d_resp = requests.delete(delete_url, headers=headers)

        print(">> Supabase DELETE:", d_resp.status_code, d_resp.text)
        if d_resp.status_code not in (200, 204):
            return f"Fehler beim Löschen: {d_resp.text}", d_resp.status_code

        return "Benutzer gelöscht", 200

    except Exception as e:
        traceback.print_exc()
        return "Serverfehler", 500
