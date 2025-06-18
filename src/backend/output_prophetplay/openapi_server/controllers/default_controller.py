# default_controller.py

import requests
import traceback
from datetime import datetime
from openapi_server.models.register_request import RegisterRequest
from openapi_server.models.login_request import LoginRequest
from flask import jsonify, request
from passlib.hash import pbkdf2_sha256 as crypto

# Supabase-Konfiguration
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
        if insert_resp.status_code in (200, 201):
            return "Registrierung erfolgreich", 200
        return insert_resp.text, insert_resp.status_code
    except Exception:
        traceback.print_exc()
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
        # 2) Rollenname holen
        role_url  = f"{SUPABASE_URL}/rest/v1/rollen?select=name&id=eq.{role_id}"
        role_resp = requests.get(role_url, headers=SUPABASE_HEADERS)
        if role_resp.status_code != 200:
            return "Fehler bei Anmeldung", 500
        roles = role_resp.json()
        rolle_name = roles[0]["name"] if roles else "User"
        # 3) Antwort
        return {"benutzername": benutzername, "rolle": rolle_name}, 200
    except Exception:
        traceback.print_exc()
        return "Fehler bei Anmeldung", 500


def api_benutzer_liste_get():
    try:
        requester = request.args.get("requester")
        # Admin-Check
        user_url  = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        user_resp = requests.get(user_url, headers=SUPABASE_HEADERS)
        if user_resp.status_code != 200:
            return "Fehler beim Abrufen der Anfrageperson", 500
        users = user_resp.json()
        if not users or users[0]["role_id"] != 1:
            return "Nicht autorisiert", 403
        # Alle Benutzer abrufen
        all_users_url  = f"{SUPABASE_URL}/rest/v1/users?select=benutzername,role_id"
        all_users_resp = requests.get(all_users_url, headers=SUPABASE_HEADERS)
        if all_users_resp.status_code != 200:
            return "Fehler beim Abrufen der Benutzer", 500
        return all_users_resp.json(), 200
    except Exception:
        traceback.print_exc()
        return "Serverfehler", 500


def api_benutzer_loeschen_delete():
    try:
        requester = request.args.get("requester")
        target    = request.args.get("target")
        # Admin-Check
        check_url  = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        check_resp = requests.get(check_url, headers=SUPABASE_HEADERS)
        if check_resp.status_code != 200:
            return "Fehler beim Rollencheck", 500
        if not check_resp.json() or check_resp.json()[0]["role_id"] != 1:
            return "Nicht autorisiert", 403
        # Löschen
        delete_url     = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{target}"
        delete_headers = {**SUPABASE_HEADERS, "Prefer": "return=minimal"}
        d_resp         = requests.delete(delete_url, headers=delete_headers)
        if d_resp.status_code not in (200, 204):
            return f"Fehler beim Löschen: {d_resp.text}", d_resp.status_code
        return "Benutzer gelöscht", 200
    except Exception:
        traceback.print_exc()
        return "Serverfehler", 500


def api_news_get():
    try:
        url  = f"{SUPABASE_URL}/rest/v1/news?select=*&order=published_at.desc"
        resp = requests.get(url, headers=SUPABASE_HEADERS)
        if resp.status_code != 200:
            return "Fehler beim Laden der News", 500
        return resp.json(), 200
    except Exception:
        traceback.print_exc()
        return "Serverfehler", 500


def api_news_post(body):
    try:
        # Body enthält title, description, url
        item = dict(body)
        # Timestamp setzen
        item["published_at"] = datetime.utcnow().isoformat()
        # Wer erstellt die News?
        requester = request.args.get("requester", "")
        item["created_by"] = requester
        # Insert
        url  = f"{SUPABASE_URL}/rest/v1/news"
        resp = requests.post(url, json=item, headers=SUPABASE_HEADERS)
        if resp.status_code not in (200, 201):
            return f"Fehler beim Anlegen: {resp.text}", resp.status_code
        return "News angelegt", 201
    except Exception:
        traceback.print_exc()
        return "Serverfehler", 500


def api_news_delete():
    try:
        news_id   = request.args.get("id")
        requester = request.args.get("requester")
        if not news_id or not requester:
            return "Fehlende Parameter", 400

        # 1) Admin-Check
        user_url  = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        user_resp = requests.get(user_url, headers=SUPABASE_HEADERS)
        if user_resp.status_code != 200:
            return "Fehler beim Rollencheck", 500
        users = user_resp.json()
        if not users or users[0]["role_id"] != 1:
            return "Nicht autorisiert", 403

        # 2) Ownership-Check (Spalte 'created_by' vorausgesetzt)
        check_url  = (
            f"{SUPABASE_URL}/rest/v1/news"
            f"?select=created_by"
            f"&id=eq.{news_id}"
        )
        check_resp = requests.get(check_url, headers=SUPABASE_HEADERS)
        if check_resp.status_code != 200:
            return "Fehler beim Abrufen der News", 500
        items = check_resp.json()
        if not items:
            return "News nicht gefunden", 404
        if items[0].get("created_by") != requester:
            return "Nicht autorisiert", 403

        # 3) Löschen
        delete_url     = f"{SUPABASE_URL}/rest/v1/news?id=eq.{news_id}"
        delete_headers = {**SUPABASE_HEADERS, "Prefer": "return=minimal"}
        d_resp         = requests.delete(delete_url, headers=delete_headers)
        if d_resp.status_code not in (200, 204):
            return f"Fehler beim Löschen: {d_resp.text}", d_resp.status_code

        return "News gelöscht", 200
    except Exception:
        traceback.print_exc()
        return "Serverfehler", 500
