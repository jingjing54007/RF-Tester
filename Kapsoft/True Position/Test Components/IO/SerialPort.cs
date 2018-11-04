using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;

namespace TruePosition.Test.IO
{
    /// <summary>
    /// Encapsulates functionality for interfacing with a serial (COM) port as
    /// an asynchronous stream target.
    /// </summary>
    [Serializable]
    public class SerialPort : Port, ISerialConfig
    {
        private const System.IO.Ports.Handshake DEF_HANDSHAKE = System.IO.Ports.Handshake.None;
        private const System.IO.Ports.Parity DEF_PARITY = System.IO.Ports.Parity.None;
        private const System.IO.Ports.StopBits DEF_STOP_BITS = System.IO.Ports.StopBits.One;
        private const string DEF_PORT_NAME = "COM1";
        private const int DEF_BAUD_RATE = 9600;
        private const int DEF_DATA_BITS = 8;
        private const int DEF_RECEIVE_BUFFER_SIZE = 8192;
        private const int DEF_TRANSMIT_BUFFER_SIZE = 8192;
        private const bool DEF_DISCARD_NULL = false;
        private const string DEF_HEADER = null; // Example of null as header: "\u0000";
        private const string DEF_TRAILER = "\r\n"; // Example of unicode equivalent: "\u000D" + "\u000A";
        private const bool DEF_INPUT_EVERY_CHARACTER = true;
        private const SerialPortMode DEF_MODE = SerialPortMode.Unsolicited;

        private System.IO.Ports.SerialPort m_port;
        private System.IO.Ports.Handshake m_handshake = DEF_HANDSHAKE;
        private System.IO.Ports.Parity m_parity = DEF_PARITY;
        private System.IO.Ports.StopBits m_stopBits = DEF_STOP_BITS;
        private string m_portName = DEF_PORT_NAME;
        private int m_baudRate = DEF_BAUD_RATE;
        private int m_dataBits = DEF_DATA_BITS;
        private int m_receiveBufferSize = DEF_RECEIVE_BUFFER_SIZE;
        private int m_transmitBufferSize = DEF_TRANSMIT_BUFFER_SIZE;
        private bool m_discardNull;
        private string m_inputHeader = DEF_HEADER;
        private string m_outputHeader = DEF_HEADER;
        private string m_inputTrailer = DEF_TRAILER;
        private string m_outputTrailer = DEF_TRAILER;
        private string m_inputBuffer = string.Empty;
        private SerialPortMode m_mode = DEF_MODE;
        private int m_transmitRetries = DEF_TRANSMIT_RETRIES;
        private bool m_responseReceived = false;
        private bool m_inputEveryCharacter = DEF_INPUT_EVERY_CHARACTER;

        private const string SP_HANDSHAKE = "Handshake";
        private const string SP_PARITY = "Parity";
        private const string SP_STOP_BITS = "StopBits";
        private const string SP_PORT_NAME = "PortName";
        private const string SP_BAUD_RATE = "BaudRate";
        private const string SP_DATA_BITS = "DataBits";
        private const string SP_RECEIVE_BUFFER_SIZE = "ReceiveBufferSize";
        private const string SP_TRANSMIT_BUFFER_SIZE = "TransmitBufferSize";
        private const string SP_DISCARD_NULL = "DiscardNull";
        private const string SP_INPUT_HEADER = "InputHeader";
        private const string SP_OUTPUT_HEADER = "OutputHeader";
        private const string SP_INPUT_TRAILER = "InputTrailer";
        private const string SP_OUTPUT_TRAILER = "OutputTrailer";
        private const string SP_INPUT_EVERY_CHARACTER = "InputEveryCharacter";
        private const string SP_MODE = "Mode";
        private const string SP_RECEIVE_TIMEOUT = "ReceiveTimeout";
        private const string SP_TRANSMIT_RETRIES = "TransmitRetries";

        protected override void Initialize()
        {
            m_port = new System.IO.Ports.SerialPort();
            m_responseReceived = false;
            timeoutTimer.Enabled = false;
            timeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(timeoutTimer_Elapsed);
        }

        /// <summary>
        /// Constructs a new serial port class with default settings
        /// </summary>
        public SerialPort() 
            : base()
        {
            Initialize();
            this.State = PortState.Created;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="obj">object to copy</param>
        public SerialPort(SerialPort obj)
            : base()
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            this.BaudRate = obj.BaudRate;
            this.DataBits = obj.DataBits;
            this.DiscardNull = obj.DiscardNull;
            this.Handshake = obj.Handshake;
            this.Parity = obj.Parity;
            this.PortName = obj.PortName;
            this.ReceiveBufferSize = obj.ReceiveBufferSize;
            this.StopBits = obj.StopBits;
            this.TransmitBufferSize = obj.TransmitBufferSize;
            this.InputHeader = obj.InputHeader;
            this.InputTrailer = obj.InputTrailer;
            this.OutputHeader = obj.OutputHeader;
            this.OutputTrailer = obj.OutputTrailer;
            this.InputEveryCharacter = obj.InputEveryCharacter;
            this.Mode = obj.Mode;
            this.ReceiveTimeout = obj.ReceiveTimeout;
            this.TransmitRetries = obj.TransmitRetries;
            this.State = PortState.Created;
        }

        /// <summary>
        /// Constructs object using the parameters supplied
        /// </summary>
        /// <param name="portName">Descriptive name of resource</param>
        public SerialPort(string name)
            : base()
        {
            Initialize();
            this.Name = name;
            this.State = PortState.Created;
        }

        /// <summary>
        /// Constructor for object deserialization
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected SerialPort(SerializationInfo info, StreamingContext context)
            : base()
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Initialize();

            this.BaudRate = info.GetInt32(SP_BAUD_RATE);
            this.DataBits = info.GetInt32(SP_DATA_BITS);
            this.Parity = (System.IO.Ports.Parity)info.GetInt32(SP_PARITY);
            this.Handshake = (System.IO.Ports.Handshake)info.GetInt32(SP_HANDSHAKE);
            this.DiscardNull = info.GetBoolean(SP_DISCARD_NULL);
            this.InputHeader = info.GetString(SP_INPUT_HEADER);
            this.InputTrailer = info.GetString(SP_INPUT_TRAILER);
            this.OutputHeader = info.GetString(SP_OUTPUT_HEADER);
            this.OutputTrailer = info.GetString(SP_OUTPUT_TRAILER);
            this.InputEveryCharacter = info.GetBoolean(SP_INPUT_EVERY_CHARACTER);
            this.Mode = (SerialPortMode)info.GetInt32(SP_MODE);
            this.ReceiveTimeout = info.GetInt32(SP_RECEIVE_TIMEOUT);
            this.TransmitRetries = info.GetInt32(SP_TRANSMIT_RETRIES);

            try
            {
                this.PortName = info.GetString(SP_PORT_NAME);
            }
            catch (ArgumentNullException)
            {
                this.PortName = DEF_PORT_NAME;
            }

            this.ReceiveBufferSize = info.GetInt32(SP_RECEIVE_BUFFER_SIZE);
            this.TransmitBufferSize = info.GetInt32(SP_TRANSMIT_BUFFER_SIZE);
            this.StopBits = (System.IO.Ports.StopBits)info.GetInt32(SP_STOP_BITS);

            this.State = PortState.Created;
        }

        /// <summary>
        /// Gets object information for serialization
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Streaming context</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            CheckDisposed();

            if (info == null)
            {
                return;
            }

            info.AddValue(SP_BAUD_RATE, m_baudRate);
            info.AddValue(SP_DATA_BITS, m_dataBits);
            info.AddValue(SP_HANDSHAKE, m_handshake);
            info.AddValue(SP_PARITY, m_parity);
            info.AddValue(SP_PORT_NAME, m_portName);
            info.AddValue(SP_RECEIVE_BUFFER_SIZE, m_receiveBufferSize);
            info.AddValue(SP_STOP_BITS, m_stopBits);
            info.AddValue(SP_TRANSMIT_BUFFER_SIZE, m_transmitBufferSize);
            info.AddValue(SP_DISCARD_NULL, m_discardNull);
            info.AddValue(SP_INPUT_HEADER, m_inputHeader);
            info.AddValue(SP_INPUT_TRAILER, m_inputTrailer);
            info.AddValue(SP_OUTPUT_HEADER, m_outputHeader);
            info.AddValue(SP_OUTPUT_TRAILER, m_outputTrailer);
            info.AddValue(SP_INPUT_EVERY_CHARACTER, m_inputEveryCharacter);
            info.AddValue(SP_MODE, m_mode);
            info.AddValue(SP_RECEIVE_TIMEOUT, ReceiveTimeout);
            info.AddValue(SP_TRANSMIT_RETRIES, m_transmitRetries);
        }

        /// <summary>
        /// Returns/sets the size of the receive buffer
        /// </summary>
        public int ReceiveBufferSize
        {
            get
            {
                CheckDisposed();
                return m_receiveBufferSize;
            }
            set
            {
                CheckDisposed();
                m_receiveBufferSize = value;
            }
        }

        /// <summary>
        /// Returns/sets the size of the transmit buffer
        /// </summary>
        public int TransmitBufferSize
        {
            get
            {
                CheckDisposed();
                return m_transmitBufferSize;
            }
            set
            {
                CheckDisposed();
                m_transmitBufferSize = value;
            }
        }

        /// <summary>
        /// Returns/sets the type of handshaking to be used for this port
        /// </summary>
        public System.IO.Ports.Handshake Handshake
        {
            get
            {
                CheckDisposed();
                return m_handshake;
            }

            set
            {
                CheckDisposed();
                m_handshake = value;
            }
        }

        /// <summary>
        /// Returns/sets the parity style to be used for this port
        /// </summary>
        public System.IO.Ports.Parity Parity
        {
            get
            {
                CheckDisposed();
                return m_parity;
            }

            set
            {
                CheckDisposed();
                m_parity = value;
            }
        }

        /// <summary>
        /// Returns/sets the stop bits to be used for this port
        /// </summary>
        public System.IO.Ports.StopBits StopBits
        {
            get
            {
                CheckDisposed();
                return m_stopBits;
            }

            set
            {
                CheckDisposed();
                m_stopBits = value;
            }
        }

        /// <summary>
        /// Returns/sets the port assignment for this port
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if passed a null reference</exception>
        public string PortName
        {
            get
            {
                CheckDisposed();
                return m_portName;
            }

            set
            {
                CheckDisposed();
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                m_portName = value;
            }
        }
        /// <summary>
        /// Returns/sets the baud rate for this port
        /// </summary>
        public int BaudRate
        {
            get
            {
                CheckDisposed();
                return m_baudRate;
            }

            set
            {
                CheckDisposed();
                m_baudRate = value;
            }
        }

        /// <summary>
        /// Returns/sets the data bits for this port
        /// </summary>
        public int DataBits
        {
            get
            {
                CheckDisposed();
                return m_dataBits;
            }

            set
            {
                CheckDisposed();
                m_dataBits = value;
            }
        }

        /// <summary>
        /// Returns/sets whether nulls are discarded when received
        /// </summary>
        public bool DiscardNull
        {
            get
            {
                CheckDisposed();
                return m_discardNull;
            }
            set
            {
                CheckDisposed();
                m_discardNull = value;
            }
        }

        /// <summary>
        /// Returns/sets the input text header packing
        /// </summary>
        [XmlAttribute]
        public string InputHeader
        {
            get
            {
                return m_inputHeader;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else
                {
                    m_inputHeader = value;
                }
            }
        }

        /// <summary>
        /// Returns/sets the output text header packing
        /// </summary>
        [XmlAttribute]
        public string OutputHeader
        {
            get
            {
                return m_outputHeader;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else
                {
                    m_outputHeader = value;
                }
            }
        }

        /// <summary>
        /// Returns/sets the input text packing
        /// </summary>
        [XmlAttribute]
        public string InputTrailer
        {
            get
            {
                return m_inputTrailer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else
                {
                    m_inputTrailer = value;
                }
            }
        }

        /// <summary>
        /// Returns/sets whether unpacked input protocols (input header and trailer are
        /// both empty) generate an input event for each character read, or whether
        /// a single input event is generated.
        /// </summary>
        public bool InputEveryCharacter
        {
            get
            {
                return m_inputEveryCharacter;
            }
            set
            {
                m_inputEveryCharacter = value;
            }
        }

        /// <summary>
        /// Returns/sets the output text trailer packing
        /// </summary>
        [XmlAttribute]
        public string OutputTrailer
        {
            get
            {
                return m_outputTrailer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else
                {
                    m_outputTrailer = value;
                }
            }
        }

        public SerialPortMode Mode
        {
            get
            {
                return m_mode;
            }
            set
            {
                m_mode = value;
            }
        }

        /// <summary>
        /// Attempts to connect to the target serial port using the parameters as configured.
        /// </summary>
        public override void Open()
        {
            CheckDisposed();

            try
            {
                this.State = PortState.Opening;

                m_port.BaudRate = m_baudRate;
                m_port.DataBits = m_dataBits;
                m_port.DiscardNull = m_discardNull;
                m_port.Handshake = m_handshake;
                m_port.Parity = m_parity;
                m_port.PortName = m_portName;
                m_port.ReadBufferSize = m_receiveBufferSize;
                m_port.ReceivedBytesThreshold = 1;
                m_port.StopBits = m_stopBits;
                m_port.WriteBufferSize = m_transmitBufferSize;
                m_port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(m_port_ErrorReceived);
                m_port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(m_port_DataReceived);
                m_port.Open();
            }
            catch (InvalidOperationException e)
            {
                this.SignalError("Connection Error", e.Message);
            }
            catch (System.IO.IOException e)
            {
                this.SignalError("Connection Error", e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                this.SignalError("Connection Error", e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                this.SignalError("Connection Error", e.Message);
            }
            finally
            {
                this.State = (m_port.IsOpen) ? PortState.Opened : PortState.Faulted;
            }
        }

        /// <summary>
        /// Disconnects from the target resource.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the port is already closed</exception>
        public override void Close()
        {
            CheckDisposed();

            if (timeoutTimer != null)
                timeoutTimer.Enabled = false;

            if (m_port == null)
            {
                State = PortState.Closed;
                return;
            }
            
            this.State = PortState.Closing;

            if (m_port.IsOpen)
            {
                m_port.Close();
            }

            this.State = (m_port.IsOpen) ? PortState.Faulted : PortState.Closed;
        }

        /// <summary>
        /// Constructs an output byte stream with the selected output encoding and packing
        /// </summary>
        /// <param name="message">Text to encode</param>
        public byte[] ConstructOutput(string message)
        {
            return ConstructOutput(message, true);
        }

        /// <summary>
        /// Constructs an output byte stream with the selected output encoding and,
        /// if desired, the output packing
        /// </summary>
        /// <param name="message">Text to encode</param>
        /// <param name="pack">Set to true to apply packing before constructing output, false to construct output as is</param>
        public byte[] ConstructOutput(string message, bool pack)
        {
            string outputMessage;

            if (message == null)
            {
                throw new ArgumentNullException("input");
            }

            outputMessage = message;

            if (pack)
            {
                outputMessage = m_outputHeader + outputMessage + m_outputTrailer;
            }

            return Encoding.UTF8.GetBytes(outputMessage);
        }

        /// <summary>
        /// Transmits the required message with or without defined message packing
        /// </summary>
        /// <param name="message">Message to transmit</param>
        /// <param name="pack">Set to true to apply message packing before transmission, false to transmit as is</param>
        /// <param name="lingerTimeout">When set to a non-zero value, the transmit will block until all data has been transmitted, or the timeout is reached, whichever comes first</param>
        /// <returns>True if the transmit was successful</returns>
        public override bool Transmit(string message, bool pack)
        {
            CheckDisposed();
            byte[] bytes;
            bool ret = false;

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            // If in solicited mode and the receive timeout is still active, transmit is not allowed...
            if ((m_mode == SerialPortMode.Solicited) && (timeoutTimer.Enabled == true))
            {
                throw new InvalidOperationException("In solicited mode, a transmit cannot occur before a receive or timeout event.");
            }

            if (!m_port.IsOpen)
            {
                return ret;
            }

            bytes = ConstructOutput(message, pack);

            if (OutputBytes(bytes))
            {
                ret = true;
                OnDataTransmitted(message);
            }

            return ret;
        }

        /// <summary>
        /// Outputs the byte stream to the serial port
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="lingerTimeout">Time to wait for completion of transmit</param>
        /// <returns>True if the transmit was successful</returns>
        protected bool OutputBytes(byte[] data)
        {
            int lengthToWrite = data.Length;
            int writeBufferFree = (m_port.WriteBufferSize - m_port.BytesToWrite);
            bool ret = false;

            if (writeBufferFree < 0)
            {
                writeBufferFree = 0;
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (lengthToWrite > writeBufferFree)
            {
                SignalError("Transmit buffer overflow error");
                return ret;
            }

            try
            {
                int retries = 0;
                bool done = false;
                do
                {
                    m_responseReceived = false;
                    m_port.BaseStream.Write(data, 0, lengthToWrite);
                    ret = true;

                    if (m_mode != SerialPortMode.Solicited)
                    {
                        done = true;
                    }
                    else
                    {
                        timeoutTimer.Interval = ReceiveTimeout;
                        timeoutTimer.Enabled = true;
                        while ((m_mode == SerialPortMode.Solicited) && (timeoutTimer.Enabled)) { System.Threading.Thread.Sleep(1); }

                        if (m_responseReceived)
                        {
                            done = true;
                        }
                        else if (m_transmitRetries == retries++)
                        {
                            done = true;
                            ret = false;
                            SignalError("Transmit failed. Maximum retries attempted without response.");
                        }
                    }
                } while (! done);
            }
            catch (System.IO.IOException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return ret;
        }

        /// <summary>
        /// Appends data to the input buffer stream then immediately processes the current stream
        /// firing any input events
        /// </summary>
        /// <param name="data">Data to append to the input buffer stream</param>
        private void AppendInput(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            else
            {
                m_inputBuffer += Encoding.UTF8.GetString(data);
                ProcessInputData();
            }
        }

        /// <summary>
        /// Method processes the current input buffer and, if applicable, fires one or more
        /// Input events
        /// </summary>
        private void ProcessInputData()
        {
            string input;
            string inputHeader;
            string inputTrailer;
            int index;

            if (string.IsNullOrEmpty(m_inputHeader) && string.IsNullOrEmpty(m_inputTrailer))
            {
                input = m_inputBuffer;
                m_inputBuffer = String.Empty;

                if (m_inputEveryCharacter)
                {
                    for (index = 0; index < input.Length; index++)
                    {
                        OnDataReceived(input);
                    }
                }
                else
                {
                    OnDataReceived(input);
                }
            }
            else if (string.IsNullOrEmpty(m_inputHeader) || string.IsNullOrEmpty(m_inputTrailer))
            {
                inputTrailer = string.Empty;

                if (string.IsNullOrEmpty(m_inputHeader))
                {
                    inputTrailer = m_inputTrailer;
                }
                else
                {
                    inputTrailer = m_inputHeader;
                }

                while (!string.IsNullOrEmpty(m_inputBuffer))
                {
                    index = m_inputBuffer.IndexOf(inputTrailer);

                    if (index < 0)
                    {
                        break;
                    }
                    else
                    {
                        input = m_inputBuffer.Substring(0, index);
                        m_inputBuffer = m_inputBuffer.Substring(index + inputTrailer.Length);

                        if (!string.IsNullOrEmpty(input))
                            OnDataReceived(input);
                    }
                }
            }
            else
            {
                inputTrailer = m_inputTrailer;
                inputHeader = m_inputHeader;

                while (!string.IsNullOrEmpty(m_inputBuffer))
                {
                    index = m_inputBuffer.IndexOf(inputTrailer);

                    if (index < 0)
                    {
                        break;
                    }
                    else
                    {
                        input = m_inputBuffer.Substring(0, index);
                        m_inputBuffer = m_inputBuffer.Substring(index + inputTrailer.Length);

                        index = input.LastIndexOf(inputHeader);

                        if (index >= 0)
                        {
                            input = input.Substring(index + inputHeader.Length);
                            OnDataReceived(input);
                        }
                    }
                }
            }
        }

        private void m_port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int length = m_port.BytesToRead;
            byte[] bytes = new byte[length];

            m_port.BaseStream.Read(bytes, 0, length);
            this.AppendInput(bytes);

            m_responseReceived = true;
            timeoutTimer.Enabled = false;
        }

        private void m_port_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            string error = string.Empty;

            switch (e.EventType)
            {
                case System.IO.Ports.SerialError.Frame:
                    error = "Framing Error";
                    break;
                case System.IO.Ports.SerialError.Overrun:
                    error = "Character Buffer Overrun";
                    break;
                case System.IO.Ports.SerialError.RXOver:
                    error = "Receive Buffer Overflow";
                    break;
                case System.IO.Ports.SerialError.RXParity:
                    error = "Parity Error";
                    break;
                case System.IO.Ports.SerialError.TXFull:
                    error = "Transmit Buffer Overflow";
                    break;
                default:
                    error = "Unknown Error";
                    break;
            }

            this.SignalError(error);
        }

        protected override void timeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args)
        {
            m_responseReceived = false;
            timeoutTimer.Enabled = false;
            OnReceiveTimeoutExpired();
        }

        protected override void OnDisposePort()
        {
            this.Close();
            // Dispose of all managed resources here.
            if (m_port != null)
                m_port.Dispose();
        }
        private void CheckDisposed()
        {
            if (disposed)
            {
                this.State = PortState.Faulted;
                throw new ObjectDisposedException(this.GetType().AssemblyQualifiedName);
            }
        }
    }
}
