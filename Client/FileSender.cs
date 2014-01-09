namespace Client
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using System.Net.Sockets;
	using System.Text;

	public delegate void KeyPressEventHandler(object sender, EventArgs args);
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

			//Console.CancelKeyPress += KeyPressHanlder;


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
			//Array.Reverse(tempArray);

			offset = BitConverter.ToInt32(tempArray, 0);

			Console.WriteLine(offset);

			file.Seek(offset, SeekOrigin.Begin);

			while (file.Position != file.Length)
			{
				ConsoleKeyInfo keyPress = new ConsoleKeyInfo();

				if (Console.KeyAvailable == true)
				{
					keyPress = Console.ReadKey(true);
					if (keyPress.Key == ConsoleKey.Z)
					{
						KeyPressHanlder();
						continue;
					}
				}

				byteRead = file.Read(message, 0, message.Length);

				try
				{
					if (!_tcpClient.Write(message, 0, byteRead, SocketFlags.None))
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

		private void KeyPressHanlder()//object sender, ConsoleCancelEventArgs args)
		{
			Console.WriteLine("KeyPress!");
			byte[] oobByte = new byte[1];
			byte[] buffer = new byte[1024];

			_tcpClient.Write(oobByte,0, oobByte.Length, SocketFlags.OutOfBand);

			var bytesRead = _tcpClient.Read(buffer, 0, buffer.Length);

			if (bytesRead == 0)
			{
				return;
			}


			Console.WriteLine(BitConverter.ToInt32(buffer,0));

			//args.Cancel = true;
		}
	}
}
