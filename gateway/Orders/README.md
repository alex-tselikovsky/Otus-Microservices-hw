Клиент должен генерировать заголовок X-Request-Idempotency. На сервере используется хранилище с уже использованными ключами идемпотентности. Если приходит запрос с ключом в хранилище, запрос отклоняется с кодом 400.
 
Хранилище реализовано в памяти приложения, поэтому не поддерживается 
```
helm install auth  ..\Auth\chart\ --namespace auth --create-namespace
helm install orders  .\chart\ --namespace idempotency --create-namespace
```
Запуск тестов
```
newman run .\postman_collection.json
```