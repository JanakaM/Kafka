# Kafka
Console app with Kafka Producer and Consumer

Running Kafka Producer & Consumer on local machine

https://www.youtube.com/watch?v=n_IQq3pze0s


> docker pull confluentinc/cp-zookeeper - > this imaage of Zooker enable kafka to keep track of consumers and Brokers 

> docker pull confluentinc/cp-kafka  -> this image of Kafka is community download which is good for running in locally. 


Now create a docker network which can be use both Kafka and Zookeeper 

> docker network create kafka


Now run Zookerper container 

- p -> bind the zooker container to port 2181

> docker run -d --network=kafka  --name=zooker  -e ZOOKEEPER_CLIENT_PORT=2181 -e ZOOKEEPER_TICK_TIME=2000 -p 2181:2181 confluentinc/cp-zookeeper
> docker logs zooker -> check logs of container

Now Run the Kafka container 

-p binf the port 

> docker run -d --network=kafka --name=kafka -p 9092:9092 -e KAFKA_ZOOKEEPER_CONNECT=zooker:2181 -e KAFKA_ADVERTISED_LISTNERS=PLAINTEXT://localhost:9092 confluentinc/cp-kafka    


Now it's time to create Producer service 

And Conduktor is a open source application connecting Kafka running in Docker -> https://docs.conduktor.io/
