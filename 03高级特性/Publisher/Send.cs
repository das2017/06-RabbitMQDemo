using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public class Send
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
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2;    //1表示不持久，2表示持久化

                    Console.WriteLine("请注意：演示耗时较长的消息时，可通过发送带有‘.’的内容去模拟，每个‘.’加1秒！");

                    while (true)
                    {
                        Console.Write("请输入要发送的消息：");
                        var message = Console.ReadLine();
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish("", queue, properties, body);   //发送消息

                        Console.WriteLine("已发送的消息： {0}", message);
                    }
                }
            }
        }
    }
}
