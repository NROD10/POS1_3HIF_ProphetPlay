import requests
from openapi_server.models.register_request import RegisterRequest
from openapi_server.models.login_request import LoginRequest
from flask import jsonify, request
from passlib.hash import pbkdf2_sha256 as crypto

SUPABASE_URL = "https://qccwfrohegoiaizozyks.supabase.co"
SUPABASE_API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFjY3dmcm9oZWdvaWFpem96eWtzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkyMDMwODAsImV4cCI6MjA2NDc3OTA4MH0.czyt8tylDANbC7i1iDtk5oh3Clebh9LLleJVuSt8Pg8"

SUPABASE_HEADERS = {
    "apikey":        SUPABASE_API_KEY,
    "Authorization": f"Bearer {SUPABASE_API_KEY}",
    "Content-Type":  "application/json"
}


def api_benutzer_register_post(body):
    try:
        body = RegisterRequest.from_dict(body)

        # Existenz prüfen
        query_url  = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{body.benutzername}"
        check_resp = requests.get(query_url, headers=SUPABASE_HEADERS)
        print(">> REGISTER-EXIST-RESP:", check_resp.status_code, check_resp.text)
        if check_resp.status_code != 200:
            return "Fehler beim Überprüfen des Benutzers", 500
        if check_resp.json():
            return "Benutzername existiert bereits", 409

        # Passwort hashen
        hashed = crypto.hash(body.passwort)

        # Neuer Benutzer
        insert_url  = f"{SUPABASE_URL}/rest/v1/users"
        payload     = {
            "benutzername": body.benutzername,
            "passwort":     hashed,
            "role_id":      body.role_id
        }
        insert_resp = requests.post(insert_url, json=payload, headers=SUPABASE_HEADERS)
        print(">> REGISTER-INSERT-RESP:", insert_resp.status_code, insert_resp.text)

        if insert_resp.status_code in (200, 201):
            return "Registrierung erfolgreich", 200
        else:
            return insert_resp.text, insert_resp.status_code

    except Exception as e:
        print("Ausnahme bei Registrierung:", e)
        return "Fehler bei Registrierung", 500


def api_benutzer_login_post(body):
    try:
        body = LoginRequest.from_dict(body)
        benutzername = body.benutzername
        passwort     = body.passwort

        # 1) Benutzer + Passwort-Hash + role_id abrufen
        users_url   = (
            f"{SUPABASE_URL}/rest/v1/users"
            f"?select=benutzername,passwort,role_id"
            f"&benutzername=eq.{benutzername}"
        )
        users_resp  = requests.get(users_url, headers=SUPABASE_HEADERS)
        print(">> LOGIN-USERS-URL:", users_url)
        print(">> LOGIN-USERS-RESP:", users_resp.status_code, users_resp.text)
        if users_resp.status_code != 200:
            return "Fehler bei Anmeldung", 500

        users = users_resp.json()
        if not users:
            return "Ungültige Anmeldedaten", 401

        record      = users[0]
        stored_hash = record.get("passwort")
        role_id     = record.get("role_id")

        # Passwort prüfen
        if not stored_hash or not crypto.verify(passwort, stored_hash):
            return "Ungültige Anmeldedaten", 401

        # 2) Rollenname separat holen
        role_url  = f"{SUPABASE_URL}/rest/v1/rollen?select=name&id=eq.{role_id}"
        role_resp = requests.get(role_url, headers=SUPABASE_HEADERS)
        print(">> LOGIN-ROLE-URL:", role_url)
        print(">> LOGIN-ROLE-RESP:", role_resp.status_code, role_resp.text)
        if role_resp.status_code != 200:
            return "Fehler bei Anmeldung", 500
        roles = role_resp.json()
        rolle_name = roles[0]["name"] if roles else "User"

        # 3) Antwort bauen
        return {
            "benutzername": benutzername,
            "rolle":        rolle_name
        }, 200

    except Exception as e:
        print("Ausnahme bei Login:", e)
        return "Fehler bei Anmeldung", 500