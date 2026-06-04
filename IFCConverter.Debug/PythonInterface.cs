using System.Net.Sockets;
using System.Text;
using IFCConverter.Debug.Interfaces;
using Newtonsoft.Json;

namespace IFCConverter.Debug
{
    public class PythonInterface : IPythonInterface
    {
        private readonly string _host;
        private readonly int _port;
        
        public PythonInterface(string host =  "127.0.0.1", int port = 5000)
        {
            _host = host;
            _port = port;
        }

        public void Send(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            
            using TcpClient client = new TcpClient(_host, _port);
            if (!client.Connected)
                client.Connect(_host, _port);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            
            using NetworkStream stream = client.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
}