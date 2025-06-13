#from datetime import date, datetime  # noqa: F401
from typing import List, Dict  # noqa: F401
from openapi_server.models.base_model import Model
from openapi_server import util

class RegisterRequest(Model):
    """RegisterRequest - ein Modell für die Registrierung (angepasst auf role_id)."""

    def __init__(self, benutzername=None, passwort=None, role_id=None):  # noqa: E501
        """RegisterRequest - ein Modell definiert in OpenAPI

        :param benutzername: Der Benutzername dieses Requests
        :type benutzername: str
        :param passwort: Das Passwort dieses Requests
        :type passwort: str
        :param role_id: Die Rollen-ID dieses Requests
        :type role_id: int
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
    def from_dict(cls, dikt) -> 'RegisterRequest':
        """Deserialisiert ein Dict in ein RegisterRequest."""
        return util.deserialize_model(dikt, cls)

    @property
    def benutzername(self) -> str:
        return self._benutzername

    @benutzername.setter
    def benutzername(self, benutzername: str):
        if benutzername is None:
            raise ValueError("Invalid value for `benutzername`, must not be `None`")
        self._benutzername = benutzername

    @property
    def passwort(self) -> str:
        return self._passwort

    @passwort.setter
    def passwort(self, passwort: str):
        if passwort is None:
            raise ValueError("Invalid value for `passwort`, must not be `None`")
        self._passwort = passwort

    @property
    def role_id(self) -> int:
        return self._role_id

    @role_id.setter
    def role_id(self, role_id: int):
        if role_id is None:
            raise ValueError("Invalid value for `role_id`, must not be `None`")
        self._role_id = role_id
