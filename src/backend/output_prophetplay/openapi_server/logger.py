# logger.py

import os
import logging
from datetime import datetime

# Verzeichnis für Logs (wird automatisch angelegt)
log_dir = os.path.join(os.path.dirname(__file__), "logs")
os.makedirs(log_dir, exist_ok=True)

# Logdatei pro Tag
today = datetime.now().strftime("%Y-%m-%d")
log_path = os.path.join(log_dir, f"app_{today}.log")

# Logging-Konfiguration
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s [%(levelname)s] %(name)s: %(message)s',
    handlers=[
        logging.FileHandler(log_path, encoding="utf-8"),
        logging.StreamHandler()
    ]
)

logger = logging.getLogger("ProphetPlayLogger")
