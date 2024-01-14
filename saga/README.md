Для установки приложения необходимо перейти в каталог выполнить следующие команды:
```
helm dependency update e-market
helm install e-market e-market --namespace e-market --create-namespace
```
При этом установится rabbitmq в качестве зависимости.

Далее запускаем коллекцию postman для проверки:
```
cd ..\
newman run .\pm_collection.json
```

Проект реализован по паттерну саги с хореографией. Для реализации используется библиотека masstransit, которая в свою очередь строит сагу по паттерну routingslip https://masstransit.io/documentation/patterns/routing-slip.

Сервис Order получает запрос на обработку заказа и строит маршрут саги. При этом сервис подписывается на события успешного и неуспешного выполнения саги для выставления соответствующих статусов заказа.
Каждый сервис в маршруте саги знает как выполняться в прямом направлении и проводить компенсацию в случае, если какой-либо из следующих шагов не выполнился. 
В качестве примера не успешной саги используется запрос с адресом доставки в Москве. По замыслу в Москву доставка осуществляться не может. 

Обновление образов сервисов
```
docker build . -f .\Payment\Dockerfile -t tselofan/payment:latest --platform linux/amd64
docker build . -f .\Order\Dockerfile -t tselofan/order:latest --platform linux/amd64
docker build . -f .\Delivery\Dockerfile -t tselofan/delivery:latest --platform linux/amd64
docker login
docker push tselofan/payment:latest
docker push tselofan/order:latest
docker push tselofan/payment:latest
```