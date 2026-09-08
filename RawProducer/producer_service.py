from confluent_kafka import Producer
from dotenv import load_dotenv
import os

load_dotenv()

BOOTSTRAP_SERVER = os.getenv("KAFKA_BOOTSTRAP_SERVER")

conf = {'bootstrap.servers': BOOTSTRAP_SERVER}

producer = Producer(conf)

