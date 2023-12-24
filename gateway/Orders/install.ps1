helm install auth  ..\Auth\chart\ --namespace auth --create-namespace
helm install orders  .\chart\ --namespace idempotency --create-namespace