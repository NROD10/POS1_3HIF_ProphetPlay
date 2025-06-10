import requests
from openapi_server.models.register_request import RegisterRequest
from flask import jsonify, request
from passlib.hash import bcrypt  # NEU: für sicheres Passwort-Hashing

SUPABASE_URL = "https://qccwfrohegoiaizozyks.supabase.co"
SUPABASE_API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFjY3dmcm9oZWdvaWFpem96eWtzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkyMDMwODAsImV4cCI6MjA2NDc3OTA4MH0.czyt8tylDANbC7i1iDtk5oh3Clebh9LLleJVuSt8Pg8"

SUPABASE_HEADERS = {
    "apikey": SUPABASE_API_KEY,
    "Authorization": f"Bearer {SUPABASE_API_KEY}",
    "Content-Type": "application/json"
}

def api_benutzer_register_post(body):
    try:
        body = RegisterRequest.from_dict(body)

        # 1. Überprüfen, ob der Benutzername schon existiert
        query_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{body.benutzername}"
        check_response = requests.get(query_url, headers=SUPABASE_HEADERS)

        if check_response.status_code != 200:
            print("Fehler beim Abrufen der Benutzerdaten:", check_response.text)
            return "Fehler beim Überprüfen des Benutzers", 500

        if check_response.json():  # Falls schon ein Eintrag existiert
            return "Benutzername existiert bereits", 409

        # 2. Passwort sicher hashen
        hashed_passwort = bcrypt.hash(body.passwort)

        # 3. Neuen Benutzer mit gehashtem Passwort einfügen
        insert_url = f"{SUPABASE_URL}/rest/v1/users"
        data = {
            "benutzername": body.benutzername,
            "passwort": hashed_passwort,
            "rolle": body.rolle
        }

        insert_response = requests.post(insert_url, json=data, headers=SUPABASE_HEADERS)
        print(">> SUPABASE INSERT", insert_response.status_code, insert_response.text)

        if insert_response.status_code in (200, 201):
            return "Registrierung erfolgreich", 200
        else:
            # Gibt den genauen Fehlertext zurück, damit WPF ihn anzeigt
            return insert_response.text, insert_response.status_code

    except Exception as e:
        print("Ausnahme bei Registrierung:", e)
        return "Fehler bei Registrierung", 500


def api_benutzer_login_post(body):
    try:
        benutzername = body.get("benutzername")
        passwort = body.get("passwort")

        # 1. Benutzer per Username abfragen (ohne Passwort-Filter)
        login_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{benutzername}"
        login_response = requests.get(login_url, headers=SUPABASE_HEADERS)

        if login_response.status_code != 200:
            print("Fehler bei Login-Abfrage:", login_response.text)
            return "Fehler bei Anmeldung", 500

        users = login_response.json()
        if not users:
            return "Ungültige Anmeldedaten", 401

        # 2. Gehashtes Passwort prüfen
        stored_hash = users[0]["passwort"]
        if bcrypt.verify(passwort, stored_hash):
            return {
                "benutzername": users[0]["benutzername"],
                "rolle": users[0]["rolle"]
            }, 200
        else:
            return "Ungültige Anmeldedaten", 401

    except Exception as e:
        print("Ausnahme bei Login:", e)
        return "Fehler bei Anmeldung", 500
