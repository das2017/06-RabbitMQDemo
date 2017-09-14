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
                    string queue = string.Format("MQ{0}.BaseStudy", appID);

                    channel.QueueDeclare(queue, false, false, false, null);   //定义一个队列

                    while (true)
                    {
                        Console.Write("请输入要发送的消息：");
                        var message = Console.ReadLine();
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish("", queue, null, body); //发送消息

                        Console.WriteLine("已发送的消息： {0}", message);
                    }
                }
            }            
        }
    }
}
