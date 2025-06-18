## @file login_response.py
#  @brief Datenmodell für Login-Antworten.
#  @details Dieses Modell wird verwendet, um die Antwort einer erfolgreichen Login-Anfrage
#  mit Benutzername und Rolle zurückzugeben.

## @package models.login_response
#  @brief Enthält die LoginResponse-Klasse zur Repräsentation von Login-Antwortdaten.

from datetime import date, datetime  # noqa: F401
from typing import List, Dict  # noqa: F401

from openapi_server.models.base_model import Model
from openapi_server import util


class LoginResponse(Model):
    """@brief Datenklasse für Login-Antworten.

    @details Diese Klasse beschreibt die Antwortstruktur nach einem erfolgreichen Login.
    Sie enthält den Benutzernamen und die zugewiesene Rolle des Nutzers.

    @note Diese Klasse wurde automatisch durch den OpenAPI Generator erzeugt.
    Änderungen sollten mit Vorsicht vorgenommen werden.
    """

    def __init__(self, benutzername=None, rolle=None):  # noqa: E501
        """@brief Konstruktor für LoginResponse.

        @param benutzername Der Benutzername des eingeloggten Nutzers.
        @type benutzername: str
        @param rolle Die Rolle, die dem Benutzer im System zugewiesen ist.
        @type rolle: str
        """
        self.openapi_types = {
            'benutzername': str,
            'rolle': str
        }

        self.attribute_map = {
            'benutzername': 'benutzername',
            'rolle': 'rolle'
        }

        self._benutzername = benutzername
        self._rolle = rolle

    @classmethod
    def from_dict(cls, dikt) -> 'LoginResponse':
        """@brief Erstellt ein LoginResponse-Objekt aus einem Dictionary.

        @param dikt Ein Dictionary mit den Schlüsseln 'benutzername' und 'rolle'.
        @type dikt: dict
        @return Ein LoginResponse-Objekt mit den übergebenen Werten.
        @rtype: LoginResponse
        """
        return util.deserialize_model(dikt, cls)

    @property
    def benutzername(self) -> str:
        """@brief Gibt den Benutzernamen zurück.

        @return Der Benutzername dieses LoginResponse-Objekts.
        @rtype: str
        """
        return self._benutzername

    @benutzername.setter
    def benutzername(self, benutzername: str):
        """@brief Setzt den Benutzernamen.

        @param benutzername Der zu setzende Benutzername.
        @type benutzername: str
        """
        self._benutzername = benutzername

    @property
    def rolle(self) -> str:
        """@brief Gibt die Rolle des Benutzers zurück.

        @return Die Rolle des Benutzers.
        @rtype: str
        """
        return self._rolle

    @rolle.setter
    def rolle(self, rolle: str):
        """@brief Setzt die Rolle des Benutzers.

        @param rolle Die zu setzende Rolle.
        @type rolle: str
        """
        self._rolle = rolle
