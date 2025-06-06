import connexion
import os

def main():
    app = connexion.App(
        __name__,
        specification_dir=os.path.join(os.path.dirname(__file__), 'openapi'),
        options={"swagger_ui": True}
    )

    app.add_api("openapi.yaml")
    app.run(port=8080, debug=True)

if __name__ == '__main__':
    main()
