Архитектура решения

![image-20200512154011363](./README.assets/image-20200512154011363.png)

Создаем и делаем дефолтным неймспейс auth
```
kubectl create ns auth
```
Устанавливаем приложение для аутентификации 
```
cd auth
helm install auth --namespace auth chart
```
Устанавливаем приложение для хранения профиля пользователя
```
cd ..\UserProfile\
helm install user-profile --namespace auth chart
```
Запускаем коллекцию postman для проверки

```
cd ..\
newman run .\gateway.postman_collection.json
```



