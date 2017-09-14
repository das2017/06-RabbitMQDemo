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
    /// 发送消息，采用主题模式
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
                    string exchange = string.Format("Ex{0}.Logs", appID);

                    channel.ExchangeDeclare(exchange, "topic");    //声明创建一个交换机，交换机类型设定为‘topic’
                
                    while (true)
                    {
                        Console.Write("请输入要发送的消息，输入格式如'RoutingKey_Message'：");
                        var keyWithMsg = Console.ReadLine();

                        args = keyWithMsg.Split('_');
                        var routingKey = args.Length > 1 ? args[0] : "*.rabbit";
                        var message = args.Length > 1 ? args[1] : "Hello World";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange, routingKey, null, body);    //发布消息

                        Console.WriteLine("已发送的消息： '{0}':'{1}'", routingKey, message);
                    }
                }
            }            
        }
    }
}
