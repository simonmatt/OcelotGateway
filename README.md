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

# 输出
Node  Address          Status  Type    Build  Protocol  DC   Segment
1     172.17.0.4:8301  alive   server  1.8.0  2         dc1  <all>
```

### Server加入集群
Server模式在集群中建议是三个以上，这样更好地避免因为Server的宕机导致整个集群挂掉的风险。

```bash
# 加入节点2
$ docker run -d -e CONSUL_BIND_INTERFACE='eth0' --name=consul_server_2 consul:latest agent -server -node=2 -join='172.17.0.4'

# 加入节点3
$ docker run -d -e CONSUL_BIND_INTERFACE='eth0' --name=consul_server_3 consul:latest agent -server -node=3 -join='172.17.0.4'
```

#### Client加入集群
Client在Consul集群中起到代理Server的作用，Client模式不持久化数据。一般情况每台应用服务器都会安装一个或多个Client，这样可以减轻跨服务器访问带来的性能损耗，也可以减轻Server的请求压力。

```bash
# 加入client 1
$ docker run -d -e CONSUL_BIND_INTERFACE='eth0' --name=consul_server_4 consul:latest agent -client -node=4 -join='172.17.0.4' -client='0.0.0.0'

# 加入Client 2
$ docker run -d -e CONSUL_BIND_INTERFACE='eth0' --name=consul_server_5 consul:latest agent -client -node=5 -join='172.17.0.4' -client='0.0.0.0'
```

验证结果

```bash
$ docker exec consul_server_1 consul members

# 输出
Node          Address          Status  Type    Build  Protocol  DC   Segment
1             172.17.0.4:8301  alive   server  1.8.0  2         dc1  <all>
2             172.17.0.5:8301  alive   server  1.8.0  2         dc1  <all>
3             172.17.0.6:8301  alive   server  1.8.0  2         dc1  <all>
2c2946dfcec7  172.17.0.7:8301  alive   client  1.8.0  2         dc1  <default>
6b19975fe06e  172.17.0.8:8301  alive   client  1.8.0  2         dc1  <default>
```

> 参考资料
> 
> Consul 原理和使用简介 ： https://blog.coding.net/blog/intro-consul?type=hot
> 
> Consul 镜像仓库地址 ：https://hub.docker.com/_/consul
> 
> Consul 镜像使用文档：https://github.com/docker-library/docs/tree/master/consul
> 
> Consul 官方文档 ：https://www.consul.io/docs/agent/basics.html
> 
> 使用Consul和Registration对Docker容器进行服务发现： https://livewyer.io/blog/2015/02/05/service-discovery-docker-containers-using-consul-and-registrator
> 基于Consul+Registrator+Nginx实现容器服务自动发现的集群框架：http://www.mamicode.com/info-detail-2222200.html
> .NET Core微服务之基于Consul实现服务治理：https://www.cnblogs.com/edisonchou/p/9124985.html
