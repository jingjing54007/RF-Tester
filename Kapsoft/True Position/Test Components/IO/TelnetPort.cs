using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TruePosition.Test.IO
{
    public class TelnetPort : Port, ITelnetConfig
    {
        private const int DEF_PORT = 2222;
        private const string DEF_HOST_NAME = "localhost";
        private const string DEF_INPUT_TRAILER = ">";
        private const string DEF_OUTPUT_TRAILER = "\n";

        private TcpClient tcpSocket = null;

        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [XmlAttribute]
        public string InputTrailer { get; set; }
        [XmlAttribute]
        public string OutputTrailer { get; set; }

        protected override void Initialize()
        {
            HostName = DEF_HOST_NAME;
            Port = DEF_PORT;
            InputTrailer = DEF_INPUT_TRAILER;
            OutputTrailer = DEF_OUTPUT_TRAILER;

            tcpSocket = new TcpClient();
        }
        public TelnetPort()
            : base()
        {
            Initialize();
            State = PortState.Created;
        }
        public TelnetPort(string name)
            : base()
        {
            Initialize();
            Name = name;
            State = PortState.Created;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected TelnetPort(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override void Open()
        {
            try
            {
                State = PortState.Opening;
                tcpSocket.Connect(HostName, Port);
                Login(Username, Password);
                // DESIGN NOTE: This is a SMLC specific detail. Place SMLC in 'engineering mode'.
                Transmit("eng", true);
            }
            catch (Exception ex)
            {
                SignalError("Connection error", ex.Message);
            }
            finally
            {
                State = tcpSocket.Connected ? PortState.Opened : PortState.Faulted;
            }
        }
        public override void Close()
        {
            if (timeoutTimer != null)
                timeoutTimer.Enabled = false;

            if (tcpSocket == null)
            {
                State = PortState.Closed;
                return;
            }

            State = PortState.Closing;

            if (tcpSocket.Connected)
            {
                try
                {
                    tcpSocket.GetStream().Close();
                    tcpSocket.Close();
                    tcpSocket = null;
                    State = PortState.Closed;
                }
                catch
                {
                    State = PortState.Faulted;
                }
            }
        }
        public override bool Transmit(string message, bool pack)
        {
            if ((tcpSocket == null) || (!tcpSocket.Connected))
                return false;

            try
            {
                if (!pack)
                    Write(message);
                else
                    Write(message + OutputTrailer);

                OnDataTransmitted(message);
                return true;
            }
            catch (Exception ex)
            {
                SignalError("Telnet Error during Transmit: " + ex.Message);
                return false;
            }
        }

        protected override void OnDisposePort()
        {
            Close();
        }
        protected override void timeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            timeoutTimer.Enabled = false;
            OnReceiveTimeoutExpired();
        }
        protected string Login(string username, string password)
        {
            string s = Read();

            // DESIGN NOTE: This is an SMLC detail. Currently does not have security.
            if (String.IsNullOrEmpty(username))
                return s;

            if (!s.TrimEnd().EndsWith(InputTrailer))
                SignalError("Failed to connect : no login prompt");
            Transmit(username, true);

            s += Read();
            if (!s.TrimEnd().EndsWith(InputTrailer))
                SignalError("Failed to connect : no password prompt");
            Transmit(password, true);

            s += Read();
            return s;
        }
        protected void Write(string cmd)
        {
            if (!tcpSocket.Connected) 
                return;

            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);

            ProcessResponse();
        }
        protected void ProcessResponse()
        {
            string response = Read();
            while (!string.IsNullOrEmpty(response))
            {
                int index = response.IndexOf(InputTrailer);
                if (index < 0)
                    break;
                else
                {
                    string input = response.Substring(0, index);
                    response = response.Substring(index + InputTrailer.Length);

                    if (!string.IsNullOrEmpty(input))
                        OnDataReceived(input);
                }
            }
        }
        protected string Read()
        {
            if (!tcpSocket.Connected) 
                return null;

            timeoutTimer.Interval = ReceiveTimeout;
            timeoutTimer.Enabled = true;

            StringBuilder sb = new StringBuilder();
            do
            {
                ParseTelnet(sb);
                System.Threading.Thread.Sleep(100);
            } while ((tcpSocket.Available > 0) && (timeoutTimer.Enabled));

            if (!timeoutTimer.Enabled)
                sb.Length = 0;
            else
                timeoutTimer.Enabled = false;

            return sb.ToString();
        }
        protected void ParseTelnet(StringBuilder sb)
        {
            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)TelnetVerbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)TelnetVerbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)TelnetVerbs.DO:
                            case (int)TelnetVerbs.DONT:
                            case (int)TelnetVerbs.WILL:
                            case (int)TelnetVerbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)TelnetVerbs.IAC);
                                if (inputoption == (int)TelnetOptions.SGA)
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)TelnetVerbs.DO ? (byte)TelnetVerbs.WILL : (byte)TelnetVerbs.DO);
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)TelnetVerbs.DO ? (byte)TelnetVerbs.WONT : (byte)TelnetVerbs.DONT);
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }
    }
}