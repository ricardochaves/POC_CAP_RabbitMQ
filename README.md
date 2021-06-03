# POC CAP and RabbiMQ

- Install CAP
- Run migration

```
dotnet ef database update --project API_CAP_RabbitMQ/API_CAP_RabbitMQ.csproj --context SystemContext
```

https://github.com/dotnetcore/CAP/

https://cap.dotnetcore.xyz/user-guide/en/storage/sqlserver/


## RabbitMQ

Dash: http://localhost:8080/

## CAP

Dash: https://localhost:5001/cap

[Currently, bulk messaging is not supported.](https://cap.dotnetcore.xyz/user-guide/en/cap/messaging/#scheduling)
