import sqlite3
import connexion
from openapi_server.models.login_request import LoginRequest
from openapi_server.models.register_request import RegisterRequest
from openapi_server.models.login_response import LoginResponse

DB = "database.db"

def init_db():
    with sqlite3.connect(DB) as conn:
        c = conn.cursor()
        c.execute("""
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                benutzername TEXT UNIQUE NOT NULL,
                passwort TEXT NOT NULL,
                rolle TEXT NOT NULL
            )
        """)
        conn.commit()

init_db()

def api_benutzer_login_post(body: LoginRequest):
    with sqlite3.connect(DB) as conn:
        c = conn.cursor()
        c.execute("SELECT benutzername, rolle FROM users WHERE benutzername=? AND passwort=?",
                  (body.benutzername, body.passwort))
        result = c.fetchone()
        if result:
            return LoginResponse(benutzername=result[0], rolle=result[1]), 200
        return "Ungültige Anmeldedaten", 401

def api_benutzer_register_post(body: RegisterRequest):
    with sqlite3.connect(DB) as conn:
        c = conn.cursor()
        c.execute("SELECT id FROM users WHERE benutzername=?", (body.benutzername,))
        if c.fetchone():
            return "Benutzername existiert bereits", 409
        c.execute("INSERT INTO users (benutzername, passwort, rolle) VALUES (?, ?, ?)",
                  (body.benutzername, body.passwort, body.rolle))
        conn.commit()
        return "Registrierung erfolgreich", 200
