import unittest

from flask import json

from openapi_server.models.login_request import LoginRequest  # noqa: E501
from openapi_server.models.login_response import LoginResponse  # noqa: E501
from openapi_server.models.register_request import RegisterRequest  # noqa: E501
from openapi_server.test import BaseTestCase


class TestDefaultController(BaseTestCase):
    """DefaultController integration test stubs"""

    def test_api_benutzer_login_post(self):
        """Test case for api_benutzer_login_post

        Login eines Benutzers
        """
        login_request = {"passwort":"passwort","benutzername":"benutzername"}
        headers = { 
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        }
        response = self.client.open(
            '/api/benutzer/login',
            method='POST',
            headers=headers,
            data=json.dumps(login_request),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))

    def test_api_benutzer_register_post(self):
        """Test case for api_benutzer_register_post

        Registrierung eines neuen Benutzers
        """
        register_request = {"passwort":"passwort","rolle":"User","benutzername":"benutzername"}
        headers = { 
            'Content-Type': 'application/json',
        }
        response = self.client.open(
            '/api/benutzer/register',
            method='POST',
            headers=headers,
            data=json.dumps(register_request),
            content_type='application/json')
        self.assert200(response,
                       'Response body is : ' + response.data.decode('utf-8'))


if __name__ == '__main__':
    unittest.main()
