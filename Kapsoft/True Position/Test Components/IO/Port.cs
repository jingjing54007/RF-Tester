using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TruePosition.Test.IO
{
    public abstract class Port : IPort
    {
        protected const int DEF_TIMEOUT = 5000;
        protected const int DEF_TRANSMIT_RETRIES = 1;

        protected bool disposed = false;
        protected System.Timers.Timer timeoutTimer = new System.Timers.Timer(DEF_TIMEOUT);

        protected abstract void Initialize();
        public Port()
        {
            ReceiveTimeout = DEF_TIMEOUT;
            TransmitRetries = DEF_TRANSMIT_RETRIES;
        }

        /// <summary>
        /// Fires when data is received from the connection target
        /// </summary>
        public event EventHandler<PortMessageEventArgs> DataReceived;
        /// <summary>
        /// Fires when data is transmitted to the connection target
        /// </summary>
        public event EventHandler<PortMessageEventArgs> DataTransmitted;
        /// <summary>
        /// Fires when an error occurs during the operation of a connection
        /// </summary>
        public event EventHandler<PortErrorEventArgs> Error;
        /// <summary>
        /// Fires when the status of the connection changes
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChanged;
        /// <summary>
        /// Fires in PortMode.Solicited when a response has not been received within the timeout.
        /// </summary>
        public event EventHandler<PortTimeoutExpiredEventArgs> ReceiveTimeoutExpired;

        public string Name { get; set; }
        public int ReceiveTimeout { get; set; }
        public int TransmitRetries { get; set; }

        private PortState m_State = PortState.Closed;
        /// <summary>
        /// Returns the connection state of the stream with regards to the target
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when an unsupported state value is passed</exception>
        [XmlIgnore]
        public PortState State
        {
            get { return m_State; }
            protected set
            {
                PortState saved = m_State;

                m_State = value;
                if (saved != value)
                    OnStateChanged();
            }
        }

        protected void OnDataReceived(string data)
        {
            if (DataReceived != null)
                DataReceived(this, new PortMessageEventArgs(Name, data));
        }
        protected void OnDataTransmitted(string data)
        {
            if (DataTransmitted != null)
                DataTransmitted(this, new PortMessageEventArgs(Name, data));
        }
        protected void OnError(string error, string additionalDescription)
        {
            if (Error != null)
                Error(this, new PortErrorEventArgs(Name, error, additionalDescription));
        }
        protected void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged(this, new StateChangeEventArgs(m_State));
        }
        protected void OnReceiveTimeoutExpired()
        {
            if (ReceiveTimeoutExpired != null)
                ReceiveTimeoutExpired(this, new PortTimeoutExpiredEventArgs(Name));
        }

        public abstract void Open();
        public abstract void Close();
        protected abstract void timeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs args);

        public bool Transmit(string message)
        {
            return Transmit(message, true);
        }
        public abstract bool Transmit(string message, bool pack);

        public abstract void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);

        protected abstract void OnDisposePort();
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Disposes of resources held by the object
        /// </summary>
        /// <param name="disposing">If true, disposes of all resources, otherwise disposes of the unmanaged resources only.</param>
        protected void Dispose(bool disposing)
        {
            try
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        // Just to be thorough, unhook all events...
                        if (DataReceived != null)
                            Delegate.RemoveAll(DataReceived, DataReceived);

                        if (DataTransmitted != null)
                            Delegate.RemoveAll(DataTransmitted, DataTransmitted);

                        if (ReceiveTimeoutExpired != null)
                            Delegate.RemoveAll(ReceiveTimeoutExpired, ReceiveTimeoutExpired);

                        if (Error != null)
                            Delegate.RemoveAll(Error, Error);

                        if (StateChanged != null)
                            Delegate.RemoveAll(StateChanged, StateChanged);

                        if (timeoutTimer != null)
                        {
                            timeoutTimer.Elapsed -= timeoutTimer_Elapsed;
                            timeoutTimer.Enabled = false;
                            timeoutTimer.Dispose();

                            timeoutTimer = null;
                        }

                        OnDisposePort();
                    }

                    // Dispose of all unmanaged resources here.

                    disposed = true;
                }
            }
            finally
            {
            }
        }

        /// <summary>
        /// Causes an error of a particular code and description to be reported to
        /// objects that have registered with the Error event.
        /// </summary>
        /// <param name="error">The error struct associated with the particular error</param>
        protected void SignalError(string error)
        {
            SignalError(error, string.Empty);
        }
        /// <summary>
        /// Causes an error of a particular code, description, and additional information to
        /// be reported to objects that have registered with the Error event.
        /// </summary>
        /// <param name="error">Error message</param>
        /// <param name="additionalDescription">Additional error description</param>
        protected void SignalError(string error, string additionalDescription)
        {
            timeoutTimer.Enabled = false;
            OnError(error, additionalDescription);
        }
    }
}
