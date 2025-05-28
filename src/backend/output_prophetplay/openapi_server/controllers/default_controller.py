import connexion
from typing import Dict
from typing import Tuple
from typing import Union

from openapi_server.models.login_request import LoginRequest  # noqa: E501
from openapi_server.models.login_response import LoginResponse  # noqa: E501
from openapi_server.models.register_request import RegisterRequest  # noqa: E501
from openapi_server import util


def api_benutzer_login_post(body):  # noqa: E501
    """Login eines Benutzers

     # noqa: E501

    :param login_request: 
    :type login_request: dict | bytes

    :rtype: Union[LoginResponse, Tuple[LoginResponse, int], Tuple[LoginResponse, int, Dict[str, str]]
    """
    login_request = body
    if connexion.request.is_json:
        login_request = LoginRequest.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'


def api_benutzer_register_post(body):  # noqa: E501
    """Registrierung eines neuen Benutzers

     # noqa: E501

    :param register_request: 
    :type register_request: dict | bytes

    :rtype: Union[None, Tuple[None, int], Tuple[None, int, Dict[str, str]]
    """
    register_request = body
    if connexion.request.is_json:
        register_request = RegisterRequest.from_dict(connexion.request.get_json())  # noqa: E501
    return 'do some magic!'
