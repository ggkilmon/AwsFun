using System;

namespace AWSFun
{
    public class MessagePump
    {
        public static void Main(string[] args)
        {
            MessagePumpService service = new MessagePumpService();

            for (var i = 0; i < 30; i++)
            {
                MessagePumpModel model = new MessagePumpModel()
                {
                    Id = Guid.NewGuid(),
                    Message = string.Format("Hello World {0}; {1}", i, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                };
                service.SendMessage(model);
            }
            
            Console.ReadLine();
        }
    }
}
