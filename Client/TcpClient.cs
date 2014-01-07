namespace Client
{
	using System.Net.Sockets;

	public class TcpClient
	{
		private readonly Socket _clientSocket;

		public TcpClient(Socket socket)
		{
			_clientSocket = socket;
		}

		public void Connect(string ipAddress, int port)
		{
			_clientSocket.Connect(ipAddress, port);
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			try
			{
				return _clientSocket.Receive(buffer, offset,
						count, SocketFlags.None);
			}
			catch
			{
				return 0;
			}
		}

		public bool Write(byte[] buffer, int offset, int count)
		{
			try
			{
				_clientSocket.Send(buffer, offset, count, SocketFlags.None);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void EndConnection()
		{
			if (_clientSocket != null && _clientSocket.Connected)
			{
				_clientSocket.Shutdown(SocketShutdown.Both);
				_clientSocket.Close();
			}
		}
	}
}
