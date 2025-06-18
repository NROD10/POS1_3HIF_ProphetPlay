# default_controller.py

import requests
from datetime import datetime
from flask import jsonify, request
from passlib.hash import pbkdf2_sha256 as crypto

from openapi_server.models.register_request import RegisterRequest
from openapi_server.models.login_request import LoginRequest
from openapi_server.logger import logger

# Supabase-Konfiguration
SUPABASE_URL = "https://qccwfrohegoiaizozyks.supabase.co"
SUPABASE_API_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFjY3dmcm9oZWdvaWFpem96eWtzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkyMDMwODAsImV4cCI6MjA2NDc3OTA4MH0.czyt8tylDANbC7i1iDtk5oh3Clebh9LLleJVuSt8Pg8"
SUPABASE_HEADERS = {
    "apikey":        SUPABASE_API_KEY,
    "Authorization": f"Bearer {SUPABASE_API_KEY}",
    "Content-Type":  "application/json"
}


def api_benutzer_register_post(body):
    logger.info("api_benutzer_register_post called with body=%s", body)
    try:
        body = RegisterRequest.from_dict(body)
        # Existenz prüfen
        query_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{body.benutzername}"
        logger.debug("Checking existing user via %s", query_url)
        check_resp = requests.get(query_url, headers=SUPABASE_HEADERS)
        logger.debug("Existence check status=%s, body=%s", check_resp.status_code, check_resp.text)
        if check_resp.status_code != 200:
            logger.error("Error checking user existence: %s", check_resp.status_code)
            return "Fehler beim Überprüfen des Benutzers", 500
        if check_resp.json():
            logger.warning("Register failed: username %s already exists", body.benutzername)
            return "Benutzername existiert bereits", 409

        # Passwort hashen
        hashed = crypto.hash(body.passwort)
        logger.debug("Password hashed for user %s", body.benutzername)

        # Neuer Benutzer anlegen
        insert_url = f"{SUPABASE_URL}/rest/v1/users"
        payload = {
            "benutzername": body.benutzername,
            "passwort":     hashed,
            "role_id":      body.role_id
        }
        logger.debug("Inserting new user via %s, payload=%s", insert_url, payload)
        insert_resp = requests.post(insert_url, json=payload, headers=SUPABASE_HEADERS)
        logger.debug("Insert status=%s, body=%s", insert_resp.status_code, insert_resp.text)

        if insert_resp.status_code in (200, 201):
            logger.info("User %s registered successfully", body.benutzername)
            return "Registrierung erfolgreich", 200
        else:
            logger.error("Registration failed: %s", insert_resp.text)
            return insert_resp.text, insert_resp.status_code

    except Exception:
        logger.exception("Exception in api_benutzer_register_post")
        return "Fehler bei Registrierung", 500


def api_benutzer_login_post(body):
    logger.info("api_benutzer_login_post called with body=%s", body)
    try:
        body = LoginRequest.from_dict(body)
        benutzername, passwort = body.benutzername, body.passwort

        # 1) Benutzerdaten abrufen
        users_url = (
            f"{SUPABASE_URL}/rest/v1/users"
            f"?select=benutzername,passwort,role_id"
            f"&benutzername=eq.{benutzername}"
        )
        logger.debug("Fetching user for login via %s", users_url)
        users_resp = requests.get(users_url, headers=SUPABASE_HEADERS)
        logger.debug("Login users status=%s, body=%s", users_resp.status_code, users_resp.text)
        if users_resp.status_code != 200:
            logger.error("Error fetching user for login")
            return "Fehler bei Anmeldung", 500

        users = users_resp.json()
        if not users:
            logger.warning("Login failed: user %s not found", benutzername)
            return "Ungültige Anmeldedaten", 401

        record = users[0]
        stored_hash, role_id = record.get("passwort"), record.get("role_id")
        if not stored_hash or not crypto.verify(passwort, stored_hash):
            logger.warning("Login failed: invalid password for user %s", benutzername)
            return "Ungültige Anmeldedaten", 401

        # 2) Rollennamen abrufen
        role_url = f"{SUPABASE_URL}/rest/v1/rollen?select=name&id=eq.{role_id}"
        logger.debug("Fetching role name via %s", role_url)
        role_resp = requests.get(role_url, headers=SUPABASE_HEADERS)
        logger.debug("Role fetch status=%s, body=%s", role_resp.status_code, role_resp.text)
        if role_resp.status_code != 200:
            logger.error("Error fetching role for user %s", benutzername)
            return "Fehler bei Anmeldung", 500

        roles = role_resp.json()
        rolle_name = roles[0]["name"] if roles else "User"
        logger.info("User %s logged in with role %s", benutzername, rolle_name)

        return {"benutzername": benutzername, "rolle": rolle_name}, 200

    except Exception:
        logger.exception("Exception in api_benutzer_login_post")
        return "Fehler bei Anmeldung", 500


def api_benutzer_liste_get():
    requester = request.args.get("requester")
    logger.info("api_benutzer_liste_get called, requester=%s", requester)
    try:
        # Admin-Check
        user_url = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        logger.debug("Checking admin rights via %s", user_url)
        user_resp = requests.get(user_url, headers=SUPABASE_HEADERS)
        logger.debug("Admin check status=%s, body=%s", user_resp.status_code, user_resp.text)
        if user_resp.status_code != 200:
            logger.error("Error checking admin rights")
            return "Fehler beim Abrufen der Anfrageperson", 500

        users = user_resp.json()
        if not users or users[0]["role_id"] != 1:
            logger.warning("Unauthorized access to user list by %s", requester)
            return "Nicht autorisiert", 403

        # Alle Benutzer abrufen
        all_users_url = f"{SUPABASE_URL}/rest/v1/users?select=benutzername,role_id"
        logger.debug("Fetching all users via %s", all_users_url)
        all_users_resp = requests.get(all_users_url, headers=SUPABASE_HEADERS)
        logger.debug("All users status=%s, body=%s", all_users_resp.status_code, all_users_resp.text)
        if all_users_resp.status_code != 200:
            logger.error("Error fetching all users")
            return "Fehler beim Abrufen der Benutzer", 500

        logger.info("User list returned to %s", requester)
        return all_users_resp.json(), 200

    except Exception:
        logger.exception("Exception in api_benutzer_liste_get")
        return "Serverfehler", 500


def api_benutzer_loeschen_delete():
    requester, target = request.args.get("requester"), request.args.get("target")
    logger.info("api_benutzer_loeschen_delete called, requester=%s, target=%s", requester, target)
    try:
        # Admin-Check
        check_url = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        logger.debug("Checking admin rights via %s", check_url)
        check_resp = requests.get(check_url, headers=SUPABASE_HEADERS)
        logger.debug("Admin rights status=%s, body=%s", check_resp.status_code, check_resp.text)
        if check_resp.status_code != 200:
            logger.error("Error checking admin rights")
            return "Fehler beim Rollencheck", 500

        if not check_resp.json() or check_resp.json()[0]["role_id"] != 1:
            logger.warning("Unauthorized delete user attempt by %s", requester)
            return "Nicht autorisiert", 403

        # Löschen
        delete_url = f"{SUPABASE_URL}/rest/v1/users?benutzername=eq.{target}"
        logger.debug("Deleting user via %s", delete_url)
        delete_headers = {**SUPABASE_HEADERS, "Prefer": "return=minimal"}
        d_resp = requests.delete(delete_url, headers=delete_headers)
        logger.debug("Delete user status=%s, body=%s", d_resp.status_code, d_resp.text)
        if d_resp.status_code not in (200, 204):
            logger.error("Error deleting user: %s", d_resp.text)
            return f"Fehler beim Löschen: {d_resp.text}", d_resp.status_code

        logger.info("User %s deleted by %s", target, requester)
        return "Benutzer gelöscht", 200

    except Exception:
        logger.exception("Exception in api_benutzer_loeschen_delete")
        return "Serverfehler", 500


def api_news_get():
    logger.info("api_news_get called")
    try:
        url = f"{SUPABASE_URL}/rest/v1/news?select=*&order=published_at.desc"
        logger.debug("Fetching news via %s", url)
        resp = requests.get(url, headers=SUPABASE_HEADERS)
        logger.debug("News fetch status=%s", resp.status_code)
        if resp.status_code != 200:
            logger.error("Error fetching news")
            return "Fehler beim Laden der News", 500
        logger.info("News returned successfully")
        return resp.json(), 200

    except Exception:
        logger.exception("Exception in api_news_get")
        return "Serverfehler", 500


def api_news_post(body):
    logger.info("api_news_post called with body=%s", body)
    try:
        item = dict(body)
        item["published_at"] = datetime.utcnow().isoformat()
        requester = request.args.get("requester", "")
        item["created_by"] = requester
        logger.debug("Creating news with payload=%s", item)

        url = f"{SUPABASE_URL}/rest/v1/news"
        resp = requests.post(url, json=item, headers=SUPABASE_HEADERS)
        logger.debug("News create status=%s, body=%s", resp.status_code, resp.text)
        if resp.status_code not in (200, 201):
            logger.error("Error creating news: %s", resp.text)
            return f"Fehler beim Anlegen: {resp.text}", resp.status_code

        logger.info("News created by %s successfully", requester)
        return "News angelegt", 201

    except Exception:
        logger.exception("Exception in api_news_post")
        return "Serverfehler", 500


def api_news_delete():
    news_id = request.args.get("id")
    requester = request.args.get("requester")
    logger.info("api_news_delete called, id=%s, requester=%s", news_id, requester)
    try:
        if not news_id or not requester:
            logger.warning("Missing parameters for news delete")
            return "Fehlende Parameter", 400

        # 1) Admin-Check
        user_url = f"{SUPABASE_URL}/rest/v1/users?select=role_id&benutzername=eq.{requester}"
        logger.debug("Checking admin rights via %s", user_url)
        user_resp = requests.get(user_url, headers=SUPABASE_HEADERS)
        logger.debug("Admin rights status=%s", user_resp.status_code)
        if user_resp.status_code != 200:
            logger.error("Error checking admin rights for news delete")
            return "Fehler beim Rollencheck", 500

        users = user_resp.json()
        if not users or users[0]["role_id"] != 1:
            logger.warning("Unauthorized news delete attempt by %s", requester)
            return "Nicht autorisiert", 403

        # 2) Ownership-Check
        check_url = (
            f"{SUPABASE_URL}/rest/v1/news"
            f"?select=created_by"
            f"&id=eq.{news_id}"
        )
        logger.debug("Checking ownership via %s", check_url)
        check_resp = requests.get(check_url, headers=SUPABASE_HEADERS)
        logger.debug("Ownership check status=%s", check_resp.status_code)
        if check_resp.status_code != 200:
            logger.error("Error fetching news for ownership check")
            return "Fehler beim Abrufen der News", 500

        items = check_resp.json()
        if not items:
            logger.warning("News %s not found for deletion", news_id)
            return "News nicht gefunden", 404
        if items[0].get("created_by") != requester:
            logger.warning("User %s is not owner of news %s", requester, news_id)
            return "Nicht autorisiert", 403

        # 3) Löschen
        delete_url = f"{SUPABASE_URL}/rest/v1/news?id=eq.{news_id}"
        logger.debug("Deleting news via %s", delete_url)
        delete_headers = {**SUPABASE_HEADERS, "Prefer": "return=minimal"}
        d_resp = requests.delete(delete_url, headers=delete_headers)
        logger.debug("News delete status=%s, body=%s", d_resp.status_code, d_resp.text)
        if d_resp.status_code not in (200, 204):
            logger.error("Error deleting news: %s", d_resp.text)
            return f"Fehler beim Löschen: {d_resp.text}", d_resp.status_code

        logger.info("News %s deleted by %s", news_id, requester)
        return "News gelöscht", 200

    except Exception:
        logger.exception("Exception in api_news_delete")
        return "Serverfehler", 500
