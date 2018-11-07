## pull elastic image
docker pull docker.elastic.co/elasticsearch/elasticsearch:6.4.3

## run the container for test
docker run -d -p 9200:9200 -p 9300:9300 --name elasticsearch -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:6.4.3


## pull kubana image
 docker pull docker.elastic.co/kibana/kibana:6.4.3

## run kabana container
docker run -d -p 5601:5601 --name kibana --link elasticsearch:elasticsearch docker.elastic.co/kibana/kibana:6.4.3


#3 pull logstash image
docker pull docker.elastic.co/logstash/logstash:6.4.3

## run container with current really simple config
cd /e/GitHub/Cloud-Playground/ELK

## run in deamon with bash in cmder
 docker run -d --name logstash1 -v /${PWD}:/config-dir --link elasticsearch:elasticsearch docker.elastic.co/logstash/logstash:6.4.3

docker exec -it logstash1 //bin//bash
