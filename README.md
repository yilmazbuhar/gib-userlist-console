# Gib User Elastic Search Indexer

Gelir İdaresi Başkanlığı, https://merkeztest.efatura.gov.tr adresindeki e-Fatura mükellef PK ve GB listelerini elasticsearch üzerine indexlemek için bu uygulamayı host edebilirsiniz. Uygulamaüzerindeki hangfire, belirlediğiniz aralıklarla elasticsearch indexlerinizi güncelleyecektir. Ek olarak VKN sorgulamak içinde bir uç noktası eklenmiştir. 

### İhtiyaçlar

[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)<br>
[ElasticSearch](https://www.elastic.co/downloads/elasticsearch)

To use docker composer install, [Docker](https://www.docker.com/products/docker-desktop)<br>

### Docker ile çalıştırmak için

```
docker-compose up
```

[http://localhost:5005/hangfire](http://localhost:5005/hangfire)

Enable Elastic Search Basic Authentication, add following line to appsettings.json `ElasticSearchConfig` node.

```
 "Username": "your_username",
 "Password": "your_password"
```