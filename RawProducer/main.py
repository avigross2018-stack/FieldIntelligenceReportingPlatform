import json
import logging
from dotenv import load_dotenv
from producer_service import producer
import os
import sys

def get_logger():
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s | %(levelname)s | %(message)s",
        handlers=[logging.FileHandler("logger.log", "a"),
                  logging.StreamHandler(sys.stdout)],
    )
    logger = logging.getLogger(__name__)
    return logger

log = get_logger()
load_dotenv()

RAW_TOPIC = os.getenv("RAW_TOPIC_NAME")

def main():
    counter = 1
    try:
        log.info("Loading raw JSON file")
        with open("field_reports.json", "r") as f:
            load = json.load(f)
        for line in load:
            string = json.dumps(line)
            producer.produce(RAW_TOPIC, string)
            # producer.poll(1)
            log.info("Produce raw data successfully %s", string)
            print(counter)
            counter += 1
        producer.flush()

    except(FileNotFoundError):
        log.error("Failed to load JSON File, File does not exist")
    except Exception as ex:
        log.error("Unexpected ERROR: %s", ex)


if __name__ == "__main__":
    main()
