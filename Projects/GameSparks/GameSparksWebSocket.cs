using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameSparks.Core;
//using WebSocket4Net;
using NativeWebSocket;

namespace GameSparks
{
	/// <summary>
	/// Default WebSocket implementation used in the sdk. 
	/// </summary>
	public class GameSparksWebSocket : IGameSparksWebSocket
	{
		private Action<String> onMessage;
		private Action<byte[]> onBinaryMessage;
		private Action onClose;
		private Action onOpen;
		private Action<String> onError;

		protected WebSocket ws;

		public static System.Net.EndPoint Proxy{get; set;}

        private void Initialize(String url, 
			Action onClose,
			Action onOpen,
			Action<String> onError){

			this.onOpen = onOpen;
			this.onError = onError;
			this.onClose = onClose;

			GameSparks.Core.GameSparksUtil.Log("connecting to socket " + url);

			ws = new WebSocket (url);

			//if (Proxy != null)
			//{
			//	ws.Proxy = new SuperSocket.ClientEngine.Proxy.HttpConnectProxy(Proxy);
			//}

			//#if !__IOS__
			//ws.NoDelay = true;
			//#endif

			//ws.Opened += new EventHandler(webSocketClient_Opened);
			//ws.Closed += new EventHandler(webSocketClient_Closed);
			//ws.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(webSocketClient_Error);
			//ws.EnableAutoSendPing = true;
			//ws.AutoSendPingInterval = 30;

			ws.OnOpen += Ws_OnOpen;
            ws.OnClose += Ws_OnClose;
            ws.OnError += Ws_OnError;
		}

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(String url, 
			Action<String> onMessage,
			Action onClose,
			Action onOpen,
			Action<String> onError){

			Initialize (url, onClose, onOpen, onError);

			this.onMessage = onMessage;

			//ws.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            ws.OnMessage += Ws_OnMessage;
		}

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(String url, 
			Action<byte[]> onBinaryMessage,
			Action onClose,
			Action onOpen,
			Action<String> onError){

			Initialize (url, onClose, onOpen, onError);			

			this.onBinaryMessage = onBinaryMessage;

            //ws.DataReceived += new EventHandler<DataReceivedEventArgs>(webSocketClient_BinaryMessageReceived);
            ws.OnMessageBinary += Ws_OnMessageBinary;
		}
   

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            GameSparks.Core.GameSparksUtil.Log("Opening Websocket");
            try
            {
                //ws.Open();
                ws.Connect();
            }
            catch (Exception e)
            {
                GameSparks.Core.GameSparksUtil.LogException(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
			Terminate ();
            GameSparks.Core.GameSparksUtil.Log("Closing Websocket");
            try
            {
                ws.Close();
            }
            catch (Exception e)
            {
                GameSparks.Core.GameSparksUtil.LogException(e);
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public void Terminate()
		{
			GameSparks.Core.GameSparksUtil.Log("Closing Websocket");
			try
			{
				//ws.Opened -= webSocketClient_Opened;
				//ws.Closed -= webSocketClient_Closed;
				//ws.Error -= webSocketClient_Error;
				//ws.MessageReceived -= webSocketClient_MessageReceived;
				//ws.DataReceived -= webSocketClient_BinaryMessageReceived;
				//ws.CloseWithoutHandshake();

				ws.OnOpen -= Ws_OnOpen;
				ws.OnClose -= Ws_OnClose;
				ws.OnError -= Ws_OnError;
				ws.OnMessage -= Ws_OnMessage;
				ws.OnMessageBinary -= Ws_OnMessageBinary;
                ws.Close();
                
                if (onClose != null)
				{
					onClose();
				}
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public void Send(String request)
        {
            try
            {
                //ws.Send(request);
                ws.SendText(request);
            }
            catch (Exception e)
            {
                GameSparks.Core.GameSparksUtil.LogException(e);
            }
        }

		public void SendBinary(byte[] request, int offset, int length)
		{
			try
			{
				//List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();

				//list.Add(new ArraySegment<byte>(request, offset, length));
                
                //ws.Send(list);
                ws.Send(new ArraySegment<byte>(request, offset, length).Array);
            }
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

        /// <summary>
        /// 
        /// </summary>
		public GameSparksWebSocketState State{
			
            get{
			
                try{
                    //            switch (ws.State) {
                    //    case WebSocket4Net.WebSocketState.Closed:
                    //     return GameSparksWebSocketState.Closed;
                    //    case WebSocket4Net.WebSocketState.Closing:
                    //     return GameSparksWebSocketState.Closing;
                    //    case WebSocket4Net.WebSocketState.Connecting:
                    //     return GameSparksWebSocketState.Connecting;
                    //    case WebSocket4Net.WebSocketState.Open:
                    //     return GameSparksWebSocketState.Open;
                    //}

                    switch (ws.State)
                    {
                        case NativeWebSocket.WebSocketState.Closed:
                            return GameSparksWebSocketState.Closed;
                        case NativeWebSocket.WebSocketState.Closing:
                            return GameSparksWebSocketState.Closing;
                        case NativeWebSocket.WebSocketState.Connecting:
                            return GameSparksWebSocketState.Connecting;
                        case NativeWebSocket.WebSocketState.Open:
                            return GameSparksWebSocketState.Open;
                    }
                }
                catch{}
                return GameSparksWebSocketState.None;
			}
		}

        //private void webSocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e){
        //	if (onError != null) {
        //		onError (e.Exception.Message);
        //	}
        //}

        //private void webSocketClient_Opened(object sender, EventArgs e){
        //	if (onOpen != null) {
        //		onOpen ();
        //	}
        //}

        //private void webSocketClient_Closed(object sender, EventArgs e){
        //	if (onClose != null) {
        //		onClose ();
        //	}
        //}

        //private void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e){
        //	if (onMessage != null) {
        //		onMessage (e.Message);
        //	}
        //}

        //private void webSocketClient_BinaryMessageReceived(object sender, DataReceivedEventArgs e){
        //	if (onBinaryMessage != null) {
        //		onBinaryMessage (e.Data);
        //	}
        //}

        private void Ws_OnMessage(byte[] data)
        {
            GameSparks.Core.GameSparksUtil.Log("got message for socket" + System.Text.Encoding.UTF8.GetString(data));
            onMessage?.Invoke(System.Text.Encoding.UTF8.GetString(data));
        }

        private void Ws_OnMessageBinary(byte[] data)
        {
            GameSparks.Core.GameSparksUtil.Log("got binary message for socket");
            onBinaryMessage?.Invoke(data);
        }
        private void Ws_OnError(string errorMsg)
        {
            GameSparks.Core.GameSparksUtil.Log("socket error " + errorMsg);
            onError(errorMsg);
        }

        private void Ws_OnClose(WebSocketCloseCode closeCode)
        {
            GameSparks.Core.GameSparksUtil.Log("socket closed ");
            onClose();
        }

        private void Ws_OnOpen()
        {
            GameSparks.Core.GameSparksUtil.Log("socket opened ");
            onOpen();
            DispatchLoop();
        }

        private async void DispatchLoop()
        {
            while (ws.State == WebSocketState.Open)
            {
                //GameSparks.Core.GameSparksUtil.Log("Dispatch Message");
                ws.DispatchMessageQueue();
                await Task.Delay(100);
            }
        }
    }
}
