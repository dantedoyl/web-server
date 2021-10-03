### Запуск
```
dotnet build
dotnet run ./bin/Debug/net5.0/web-server.dll
```
### Запрос
`wrk -t12 -c400 -d30s http://127.0.0.1/httptest/wikipedia_russia.html`
### web-server
```
Running 30s test @ http://127.0.0.1/httptest/wikipedia_russia.html
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    97.49ms   69.79ms 485.48ms   64.39%
    Req/Sec    75.86     48.61   410.00     76.24%
  16061 requests in 30.10s, 14.28GB read
  Socket errors: connect 0, read 29804, write 102, timeout 0
Requests/sec:    533.61
Transfer/sec:    485.99MB
```

### nginx
```
Running 30s test @ http://127.0.0.1/httptest/wikipedia_russia.html
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    95.21ms   88.91ms 857.91ms   82.64%
    Req/Sec   350.90    248.22     1.34k    73.32%
  114387 requests in 30.10s, 101.75GB read
  Socket errors: connect 0, read 396, write 0, timeout 0
Requests/sec:   3800.70
Transfer/sec:      3.38GB
```