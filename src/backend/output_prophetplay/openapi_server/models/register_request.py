## @file register_request.py
#  @brief Datenmodell für Registrierungsanfragen.
#  @details Diese Datei definiert das RegisterRequest-Modell, das beim Anlegen neuer Benutzer verwendet wird.

## @package models.register_request
#  @brief Enthält die RegisterRequest-Klasse zur Verarbeitung von Registrierungsdaten.

from typing import Dict
from openapi_server.models.base_model import Model
from openapi_server import util

# Holen des Loggers
from openapi_server.logger import logger


class RegisterRequest(Model):
    """@brief Datenklasse für Registrierungsanfragen.

    @details Dieses Modell wird verwendet, um Daten für einen neuen Benutzer
    inklusive Benutzername, Passwort und Rollen-ID zu übertragen.
    """

    def __init__(self, benutzername=None, passwort=None, role_id=None):  # noqa: E501
        """@brief Konstruktor für RegisterRequest.

        @param benutzername Der gewünschte Benutzername.
        @type benutzername: str
        @param passwort Das Passwort des Benutzers.
        @type passwort: str
        @param role_id Die ID der Benutzerrolle.
        @type role_id: int
        """
        self.openapi_types = {
            'benutzername': str,
            'passwort': str,
            'role_id': int
        }

        self.attribute_map = {
            'benutzername': 'benutzername',
            'passwort': 'passwort',
            'role_id': 'role_id'
        }

        self._benutzername = benutzername
        self._passwort = passwort
        self._role_id = role_id

    @classmethod
    def from_dict(cls, dikt: Dict) -> 'RegisterRequest':
        """@brief Erstellt ein RegisterRequest-Objekt aus einem Dictionary.

        @param dikt Dictionary mit den Werten für Benutzername, Passwort und role_id.
        @type dikt: dict
        @return Ein RegisterRequest-Objekt.
        @rtype: RegisterRequest
        """
        logger.debug("Deserializing RegisterRequest from dict: %r", dikt)
        instance = util.deserialize_model(dikt, cls)
        logger.info("RegisterRequest created for user '%s' with role_id=%s",
                    instance.benutzername, instance.role_id)
        return instance

    @property
    def benutzername(self) -> str:
        """@brief Gibt den Benutzernamen zurück.

        @return Der Benutzername.
        @rtype: str
        """
        return self._benutzername

    @benutzername.setter
    def benutzername(self, benutzername: str):
        """@brief Setzt den Benutzernamen.

        @param benutzername Der zu setzende Benutzername.
        @type benutzername: str
        """
        logger.debug("Setting benutzername to '%s'", benutzername)
        if benutzername is None:
            logger.error("Attempted to set benutzername to None")
            raise ValueError("Invalid value for `benutzername`, must not be `None`")
        self._benutzername = benutzername

    @property
    def passwort(self) -> str:
        """@brief Gibt das Passwort zurück.

        @return Das Passwort (nicht empfohlen im Klartext).
        @rtype: str
        """
        return self._passwort

    @passwort.setter
    def passwort(self, passwort: str):
        """@brief Setzt das Passwort.

        @param passwort Das zu setzende Passwort.
        @type passwort: str
        """
        logger.debug("Setting passwort (hidden) for user '%s'", self._benutzername or "<unknown>")
        if passwort is None:
            logger.error("Attempted to set passwort to None")
            raise ValueError("Invalid value for `passwort`, must not be `None`")
        self._passwort = passwort

    @property
    def role_id(self) -> int:
        """@brief Gibt die Rollen-ID zurück.

        @return Die Rollen-ID.
        @rtype: int
        """
        return self._role_id

    @role_id.setter
    def role_id(self, role_id: int):
        """@brief Setzt die Rollen-ID.

        @param role_id Die zu setzende Rollen-ID.
        @type role_id: int
        """
        logger.debug("Setting role_id to %s for user '%s'", role_id, self._benutzername or "<unknown>")
        if role_id is None:
            logger.error("Attempted to set role_id to None")
            raise ValueError("Invalid value for `role_id`, must not be `None`")
        self._role_id = role_id
