using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Xml.Serialization;

#if ! NI_MISSING
using NationalInstruments.NI4882;
#endif

namespace TruePosition.Test.IO
{
#if NI_MISSING
    enum GpibStatusFlags
    {
        IOComplete
    }

    class Device
    {
        public Device(int boardNumber, byte primary, byte secondary) { }
        public GpibStatusFlags GetCurrentStatus() { return GpibStatusFlags.IOComplete; }
        public void Dispose() { }
        public string ReadString() { return null; }
        public void Write(string message) { }
    }
#endif

    public class GpibPort : Port, IGpibConfig
    {
        private const int DEF_BOARD_ID = 0;
        private const byte DEF_PRIMARY_ADDRESS = 2;
        private const byte DEF_SECONDARY_ADDRESS = 0;

        private Device device;

        public int BoardId { get; set; }
        public byte PrimaryAddress { get; set; }
        public byte SecondaryAddress { get; set; }

        protected override void Initialize()
        {
            BoardId = DEF_BOARD_ID;
            PrimaryAddress = DEF_PRIMARY_ADDRESS;
            SecondaryAddress = DEF_SECONDARY_ADDRESS;
        }
        public GpibPort()
            : base()
        {
            Initialize();
            State = PortState.Created;
        }
        public GpibPort(string name)
            : base()
        {
            Initialize();
            Name = name;
            State = PortState.Created;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected GpibPort(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public bool Connected 
        { 
            get 
            {
                if (device == null)
                    return false;
                else
                    return device.GetCurrentStatus() == GpibStatusFlags.IOComplete; 
            } 
        }
        public override void Open()
        {
            try
            {
                State = PortState.Opening;
                device = new Device(BoardId, PrimaryAddress, SecondaryAddress);
            }
            catch (Exception ex)
            {
                SignalError("Connection error", ex.Message);
            }
            finally
            {
                State = Connected ? PortState.Opened : PortState.Faulted;
            }
        }
        public override void Close()
        {
            if (timeoutTimer != null)
                timeoutTimer.Enabled = false;

            if (device == null)
            {
                State = PortState.Closed;
                return;
            }

            State = PortState.Closing;

            if (Connected)
            {
                try
                {
                    device.Dispose();
                    device = null;
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
            if ((device == null) || (!Connected) || (string.IsNullOrEmpty(message)))
                return false;

            try
            {
                Write(message);
                OnDataTransmitted(message);
                return true;
            }
            catch (Exception ex)
            {
                SignalError("GPIB Error during Transmit: " + ex.Message);
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
        protected void Write(string command)
        {
            if (!Connected) 
                return;

            device.Write(ReplaceCommonEscapeSequences(command));

            if (command.Contains("?"))
                ProcessResponse();
        }
        protected void ProcessResponse()
        {
            string response = Read();
            if (!string.IsNullOrEmpty(response))
                OnDataReceived(response);
        }
        protected string Read()
        {
            if (!Connected) 
                return null;

            timeoutTimer.Interval = ReceiveTimeout;
            timeoutTimer.Enabled = true;

            StringBuilder sb = new StringBuilder();
            // DESIGN NOTE:
            // ReadString() throws timeout exceptions in early NI API versions if no read data waiting...
            try
            {
                string input = device.ReadString();
                while ((!string.IsNullOrEmpty(input)) && (timeoutTimer.Enabled))
                {
                    sb.Append(InsertCommonEscapeSequences(input));
                    System.Threading.Thread.Sleep(100);
                    input = device.ReadString();
                }
            }
            catch { }

            if (!timeoutTimer.Enabled)
                sb.Length = 0;
            else
                timeoutTimer.Enabled = false;

            return sb.ToString();
        }

        private string ReplaceCommonEscapeSequences(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r");
        }
        private string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}