using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// 接收消息
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
                    string queue = string.Format("MQ{0}.TaskQueue", appID);

                    channel.QueueDeclare(queue, true, false, false, null);   //定义一个支持持久化的消息队列

                    channel.BasicQos(0, 1, false);  //在一个消费者还在处理消息且没响应消息之前，不要给他分发新的消息，而是将这条新的消息发送给下一个不那么忙碌的消费者

                    Console.WriteLine("准备接收消息：");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (s, e) =>
                    {
                        var message = Encoding.UTF8.GetString(e.Body);
                        SimulationTask(message);

                        channel.BasicAck(e.DeliveryTag, false); //手动Ack：用来确认消息已经被消费完成了
                    };
                    channel.BasicConsume(queue, false, consumer);    //开启消费者与通道、队列关联
                    //channel.BasicConsume(queue, true, consumer);    //开启消费者与通道、队列关联；自动Ack

                    Console.ReadLine();
                }
            }            
        }

        /// <summary>
        /// 模拟消息任务的处理过程
        /// </summary>
        /// <param name="message">消息</param>
        private static void SimulationTask(string message)
        {
            Console.WriteLine("接收的消息： {0}", message);
            int dots = message.Split('.').Length - 1;
            Thread.Sleep(dots * 1000);  
            Console.WriteLine("接收的消息处理完成，现在时间为{0}！", DateTime.Now);            
        }
    }
}
