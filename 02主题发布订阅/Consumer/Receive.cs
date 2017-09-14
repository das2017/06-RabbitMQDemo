using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// 接收消息，采用主题模式
    /// </summary>
    public class Receive
    {
        private static readonly string appID = ConfigurationManager.AppSettings["AppID"];

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { Uri = ConfigurationManager.AppSettings["RabbitMQUri"] };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    string exchange = string.Format("Ex{0}.Logs", appID);                    

                    channel.ExchangeDeclare(exchange, "topic");    //声明创建一个交换机，交换机类型设定为‘topic’
                    var queueName = channel.QueueDeclare().QueueName;   //获取连接通道所使用的队列

                    Console.Write("请输入准备监听的消息主题格式，格式如'*.rabbit'或者'info.*'或者'info.warning.error'等：");

                    while (true)
                    {
                        var bindingKey = Console.ReadLine();
                        channel.QueueBind(queueName, exchange, bindingKey);   //队列绑定到交换机

                        Console.WriteLine("准备接收消息");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (s, e) =>
                        {
                            var routingKey = e.RoutingKey;
                            var body = e.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine("接收到的消息： '{0}':'{1}'", routingKey, message);
                        };
                        channel.BasicConsume(queueName, true, consumer);    //开启消费者与通道、队列关联
                    }
                }
            }
        }
    }
}
