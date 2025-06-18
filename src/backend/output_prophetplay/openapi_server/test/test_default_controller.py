# Datei: openapi_server/test/test_default_controller_unit.py
import unittest
from unittest.mock import patch, MagicMock

from openapi_server.controllers.default_controller import (
    api_benutzer_login_post,
    api_benutzer_register_post
)


class TestDefaultControllerUnit(unittest.TestCase):
    """Unit-Tests für DefaultController (Login/Register)."""

    @patch('openapi_server.controllers.default_controller.crypto.verify', return_value=True)
    @patch('openapi_server.controllers.default_controller.requests.get')
    def test_api_benutzer_login_post_success(self, mock_get, mock_verify):
        # 1) Mock für Benutzer-Lookup (Passwort-Hash + role_id)
        resp_user = MagicMock(status_code=200)
        resp_user.json.return_value = [{
            'benutzername': 'alice',
            'passwort': 'irrelevanter_hash',
            'role_id': 2
        }]
        # 2) Mock für Rollen-Lookup
        resp_role = MagicMock(status_code=200)
        resp_role.json.return_value = [{'name': 'User'}]
        mock_get.side_effect = [resp_user, resp_role]

        body = {'benutzername': 'alice', 'passwort': 'secret'}
        result, status = api_benutzer_login_post(body)

        self.assertEqual(status, 200)
        self.assertIsInstance(result, dict)
        self.assertEqual(result['benutzername'], 'alice')
        self.assertEqual(result['rolle'], 'User')

    @patch('openapi_server.controllers.default_controller.requests.get')
    @patch('openapi_server.controllers.default_controller.requests.post')
    def test_api_benutzer_register_post_success(self, mock_post, mock_get):
        # 1) Mock für Existenz-Check → kein Eintrag gefunden
        resp_exist = MagicMock(status_code=200)
        resp_exist.json.return_value = []
        mock_get.return_value = resp_exist

        # 2) Mock für INSERT → 201 Created
        resp_insert = MagicMock(status_code=201, text='OK')
        mock_post.return_value = resp_insert

        body = {'benutzername': 'bob', 'passwort': 'pw', 'role_id': 2}
        message, status = api_benutzer_register_post(body)

        self.assertEqual(status, 200)
        self.assertIn('erfolgreich', message.lower())

    @patch('openapi_server.controllers.default_controller.requests.get')
    def test_api_benutzer_register_post_conflict(self, mock_get):
        # Existenz-Check findet schon einen Benutzer → 409 Conflict
        resp_exist = MagicMock(status_code=200)
        resp_exist.json.return_value = [{'benutzername': 'bob'}]
        mock_get.return_value = resp_exist

        body = {'benutzername': 'bob', 'passwort': 'pw', 'role_id': 2}
        message, status = api_benutzer_register_post(body)

        self.assertEqual(status, 409)
        self.assertIn('existiert', message.lower())


if __name__ == '__main__':
    unittest.main()
