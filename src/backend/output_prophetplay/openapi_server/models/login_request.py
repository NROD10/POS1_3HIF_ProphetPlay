## @file login_request.py
#  @brief Datenmodell für Login-Anfragen.
#  @details Dieses Modell wird verwendet, um Benutzeranmeldedaten (Benutzername und Passwort) zu kapseln
#  und innerhalb der Anwendung weiterzugeben oder zu verarbeiten.

## @package models.login_request
#  @brief Enthält die LoginRequest-Klasse zur Verarbeitung von Login-Daten.

from datetime import date, datetime  # noqa: F401
from typing import List, Dict  # noqa: F401

from openapi_server.models.base_model import Model
from openapi_server import util
from openapi_server.logger import logger


class LoginRequest(Model):
    """@brief Datenklasse für Login-Anfragen.

    @details Diese Klasse enthält die Struktur für Login-Daten, wie sie vom Client
    gesendet werden. Sie umfasst den Benutzernamen und das Passwort.
    
    @note Diese Klasse wurde automatisch durch den OpenAPI Generator erzeugt.
    Änderungen sollten mit Vorsicht vorgenommen werden.
    """

    def __init__(self, benutzername=None, passwort=None):  # noqa: E501
        """@brief Konstruktor für LoginRequest.

        @param benutzername Der Benutzername, der für den Login verwendet wird.
        @type benutzername: str
        @param passwort Das zugehörige Passwort.
        @type passwort: str
        """
        logger.info(f"Initializing LoginRequest with benutzername={benutzername}")
        self.openapi_types = {
            'benutzername': str,
            'passwort': str
        }

        self.attribute_map = {
            'benutzername': 'benutzername',
            'passwort': 'passwort'
        }

        self._benutzername = benutzername
        self._passwort = passwort

    @classmethod
    def from_dict(cls, dikt) -> 'LoginRequest':
        """@brief Erstellt ein LoginRequest-Objekt aus einem Dictionary.

        @param dikt Ein Dictionary mit den Schlüsseln 'benutzername' und 'passwort'.
        @type dikt: dict
        @return Ein LoginRequest-Objekt mit den übergebenen Werten.
        @rtype: LoginRequest
        """
        logger.info(f"Deserializing LoginRequest from dict: {dikt}")
        return util.deserialize_model(dikt, cls)

    @property
    def benutzername(self) -> str:
        """@brief Gibt den Benutzernamen zurück.

        @return Der Benutzername dieses LoginRequest-Objekts.
        @rtype: str
        """
        return self._benutzername

    @benutzername.setter
    def benutzername(self, benutzername: str):
        """@brief Setzt den Benutzernamen.

        @param benutzername Der zu setzende Benutzername.
        @type benutzername: str
        @raise ValueError Wenn der Benutzername None ist.
        """
        logger.info(f"Setting benutzername to: {benutzername}")
        if benutzername is None:
            logger.error("Attempted to set benutzername to None")
            raise ValueError("Invalid value for `benutzername`, must not be `None`")  # noqa: E501

        self._benutzername = benutzername

    @property
    def passwort(self) -> str:
        """@brief Gibt das Passwort zurück.

        @return Das Passwort dieses LoginRequest-Objekts.
        @rtype: str
        """
        return self._passwort

    @passwort.setter
    def passwort(self, passwort: str):
        """@brief Setzt das Passwort.

        @param passwort Das zu setzende Passwort.
        @type passwort: str
        @raise ValueError Wenn das Passwort None ist.
        """
        logger.info(f"Setting passwort for user {self._benutzername}")
        if passwort is None:
            logger.error("Attempted to set passwort to None")
            raise ValueError("Invalid value for `passwort`, must not be `None`")  # noqa: E501

        self._passwort = passwort
