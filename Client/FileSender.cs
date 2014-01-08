namespace Client
{
	using System;
	using System.IO;
	using System.Net.Sockets;
	using System.Text;

	public class FileSender
	{
		private TcpClient _tcpClient;
		private string _fileName;

		public FileSender(string ipAddress, int port, string fileName)
		{
			_fileName = fileName;

			_tcpClient = new TcpClient(new Socket(AddressFamily.InterNetwork, 
				SocketType.Stream, ProtocolType.Tcp));

			_tcpClient.Connect(ipAddress, port);
			
			SendingFileToServer();

			_tcpClient.EndConnection();
		}

		private void SendingFileToServer()
		{
			var file = new FileStream(_fileName, FileMode.Open, FileAccess.Read);

			int offset;
			var message = Encoding.UTF8.GetBytes(_fileName);

			if (!_tcpClient.Write(message, 0, message.Length))
			{
				return;
			}

			message = new Byte[4096];
			var byteRead = 0;

			try
			{
				byteRead = _tcpClient.Read(message, 0, message.Length);
			}
			catch
			{
				return;
			}

			if (byteRead == 0)
			{
				return;
			}

			byte[] tempArray = new byte[byteRead];
			Array.Copy(message, tempArray, byteRead);
			Array.Reverse(tempArray);

			offset = BitConverter.ToInt32(tempArray, 0);

			Console.WriteLine(offset);

			file.Seek(offset, SeekOrigin.Begin);

			while (file.Position != file.Length)
			{
				byteRead = file.Read(message, 0, message.Length);

				try
				{
					if (!_tcpClient.Write(message, 0, byteRead))
					{
						break;
					}
				}
				catch
				{
					break;
				}
			}
		}
	}
}
