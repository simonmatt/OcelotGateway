## Docker 启动 Consul
> 原文链接：[https://yq.aliyun.com/articles/696142](https://yq.aliyun.com/articles/696142)

```bash
# 单机启动
$ docker run -d -p 8500:8500 -v /data/consul:/consul/data -e CONSUL_BIND_INTERFACE='eth0' --name=consul_server_1 consul:latest agent -server -bootstrap -ui -node=1 -client='0.0.0.0'
```

### Consul 镜像提供了几个个常用环境变量

- CONSUL_CLIENT_INTERFACE ：配置 Consul 的 -client= 命令参数。
- CONSUL_BIND_INTERFACE ：配置 Consul 的 -bind= 命令参数。
- CONSUL_DATA_DIR ：配置 Consul 的数据持久化目录。
- CONSUL_CONFIG_DIR：配置 Consul 的配置文件目录。

### Consul 命令简单介绍
- agent : 表示启动 Agent 进程。
- server：表示启动 Consul Server 模式。
- client：表示启动 Consul Cilent 模式。
- bootstrap：表示这个节点是 Server-Leader ，每个数据中心只能运行一台服务器。技术角度上讲 Leader 是通过 Raft 算法选举的，但是集群第一次启动时需要一个引导 Leader，在引导群集后，建议不要使用此标志。
- ui：表示启动 Web UI 管理器，默认开放端口 8500，所以上面使用 Docker 命令把 8500 端口对外开放。
- node：节点的名称，集群中必须是唯一的。
- client：表示 Consul 将绑定客户端接口的地址，0.0.0.0 表示所有地址都可以访问。
- join：表示加入到某一个集群中去。 如：-json=192.168.1.23
### Web 管理器
上面命令已经启动了 Consul 和 Web 管理器，我们现在打开 Web 管理器来看一下是否启动成功。通过浏览器浏览 Http://{serverIp}:8500 。

```bash
# 检查当前Consul集群信息
$ docker exec consul_server_1 consul members
```