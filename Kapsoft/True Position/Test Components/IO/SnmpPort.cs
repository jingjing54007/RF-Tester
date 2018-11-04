using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Lextm.SharpSnmpLib;
using Mono.Options;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;

namespace TruePosition.Test.IO
{
    public enum Command
    {
        Get,
        Set
    }

    public class SnmpPort : Port
    {
        private const string DEF_COMMUNITY = "public";
        private const VersionCode DEF_VERSION_CODE = VersionCode.V1;
        private const Levels DEF_LEVELS = Levels.None | Levels.Reportable;

        protected class Arguments
        {
            public Arguments(IPort port)
            {
                Community = DEF_COMMUNITY;
                Version = DEF_VERSION_CODE;
                Timeout = port.ReceiveTimeout;
                Level = DEF_LEVELS;
                UserName = string.Empty;
                Authentication = string.Empty;
                AuthPhrase = string.Empty;
                Privacy = string.Empty;
                PrivPhrase = string.Empty;
                Dump = false;
            }

            private IPort Port { get; set; }
            public Command Command { get; set; }
            public string Community { get; set; }
            public VersionCode Version { get; set; }
            public int Timeout { get; set; }
            public Levels Level { get; set; }
            public string UserName { get; set; }
            public string Authentication { get; set; }
            public string AuthPhrase { get; set; }
            public string Privacy { get; set; }
            public string PrivPhrase { get; set; }
            public bool Dump { get; set; }
            public int Retries 
            {
                get { return Port.TransmitRetries; }
                set { Port.TransmitRetries = value; }
            }
        }

        private OptionSet CreateOptionSet(Arguments args)
        {
            return new OptionSet()
                .Add("Get|GET", "Get to issue SNMP Get command, (default is Get)", delegate(string v) 
                                                                                { 
                                                                                    if (v != null)
                                                                                        args.Command = Command.Get; 
                                                                                })
                .Add("Set|SET", "Set to issue SNMP Set command, (default is Get)", delegate(string v)
                                                                                {
                                                                                    if (v != null)
                                                                                        args.Command = Command.Set;
                                                                                })
                .Add("c:", "-c for community name, (default is public)", delegate(string v) { if (v != null) args.Community = v; })
                .Add("l:", "-l for security level, (default is noAuthNoPriv)", delegate(string v)
                                                                                {
                                                                                    if (v.ToUpperInvariant() == "NOAUTHNOPRIV")
                                                                                    {
                                                                                        args.Level = Levels.None | Levels.Reportable;
                                                                                    }
                                                                                    else if (v.ToUpperInvariant() == "AUTHNOPRIV")
                                                                                    {
                                                                                        args.Level = Levels.Authentication | Levels.Reportable;
                                                                                    }
                                                                                    else if (v.ToUpperInvariant() == "AUTHPRIV")
                                                                                    {
                                                                                        args.Level = Levels.Authentication | Levels.Privacy | Levels.Reportable;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                       throw new InvalidOperationException("SNMP Errpr: No such security mode: " + v);
                                                                                    }
                                                                                })
                .Add("a:", "-a for authentication method (MD5 or SHA)", delegate(string v) { args.Authentication = v; })
                .Add("A:", "-A for authentication passphrase", delegate(string v) { args.AuthPhrase = v; })
                .Add("x:", "-x for privacy method", delegate(string v) { args.Privacy = v; })
                .Add("X:", "-X for privacy passphrase", delegate(string v) { args.PrivPhrase = v; })
                .Add("u:", "-u for security name", delegate(string v) { args.UserName = v; })
                .Add("d", "-d to display message dump", delegate(string v) { args.Dump = true; })
                .Add("t:", "-t for timeout value (unit is second).", delegate(string v) { args.Timeout = int.Parse(v) * 1000; })
                .Add("r:", "-r for retry count (default is 0)", delegate(string v) { args.Retries = int.Parse(v); })
                .Add("v|V|version:", "-v for SNMP version (v1, v2 and v3 are currently supported)", delegate(string v)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            args.Version = (VersionCode)Enum.Parse(typeof(VersionCode), v.ToUpper());
                                                                                                        }
                                                                                                        catch
                                                                                                        {
                                                                                                            throw new InvalidOperationException("SNMP Error: No such version: " + v);
                                                                                                        }
                                                                                                    });
        }
        private List<string> ParseCommand(OptionSet optionSet, string command)
        {
            return optionSet.Parse(command.Split(' '));
        }
        private IPAddress ParseAddress(string value)
        {
            IPAddress ip;
            bool parsed = IPAddress.TryParse(value, out ip);
            if (!parsed)
            {
                foreach (IPAddress address in Dns.GetHostAddresses(value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    ip = address;
                    break;
                }

                if (ip == null)
                    throw new InvalidOperationException("SNMP Error: Invalid host or wrong IP address found: " + value);
            }

            return ip;
        }
        private bool SetParameterCheck(List<string> values)
        {
            if ((values.Count - 1) % 3 != 0)
                throw new InvalidOperationException("SNMP Error: Invalid number of variables in SNMP Set Command. Count=" + (values.Count - 1));
            return true;
        }
        private List<Variable> BuildVariableList(Command command, List<string> values)
        {
            List<Variable> variables = new List<Variable>();
            if (command == Command.Get)
            {
                for (int i = 1; i < values.Count; i++)
                {
                    Variable test = new Variable(new ObjectIdentifier(values[i]));
                    variables.Add(test);
                }
            }
            else
            {
                for (int i = 1; i < values.Count; i = i + 3)
                {
                    string type = values[i + 1];
                    if (type.Length != 1)
                        throw new InvalidOperationException("SNMP Error: Invalid type string: " + type);

                    ISnmpData data;
                    switch (type[0])
                    {
                        case 'i':
                            data = new Integer32(int.Parse(values[i + 2]));
                            break;
                        case 'u':
                            data = new Gauge32(uint.Parse(values[i + 2]));
                            break;
                        case 't':
                            data = new TimeTicks(uint.Parse(values[i + 2]));
                            break;
                        case 'a':
                            data = new IP(IPAddress.Parse(values[i + 2]));
                            break;
                        case 'o':
                            data = new ObjectIdentifier(values[i + 2]);
                            break;
                        case 'x':
                            data = new OctetString(ByteTool.Convert(values[i + 2]));
                            break;
                        case 's':
                            data = new OctetString(values[i + 2]);
                            break;
                        case 'd':
                            data = new OctetString(ByteTool.ConvertDecimal(values[i + 2]));
                            break;
                        case 'n':
                            data = new Null();
                            break;
                        default:
                            throw new InvalidOperationException("SNMP Error: Unknown type string: " + type[0]);
                    }

                    Variable test = new Variable(new ObjectIdentifier(values[i]), data);
                    variables.Add(test);
                }
            }

            return variables;
        }
        private IPEndPoint CreateReceiver(IPAddress ip)
        {
            return new IPEndPoint(ip, 161);
        }
        private void ProcessOldVersion(Arguments args, IPEndPoint receiver, List<Variable> variables)
        {
            if (args.Version != VersionCode.V3)
            {
                timeoutTimer.Interval = args.Timeout;
                timeoutTimer.Enabled = true;

                IList<Variable> response = null;
                if (args.Command == Command.Get)
                {
                    response = Messenger.Get(args.Version, receiver, new OctetString(args.Community), variables, args.Timeout);
                }
                else
                {
                    response = Messenger.Set(args.Version, receiver, new OctetString(args.Community), variables, args.Timeout);
                }

                if (!timeoutTimer.Enabled)
                    throw ErrorException.Create("SNMP Error: Timeout while waiting for response.", receiver.Address);
                else
                    timeoutTimer.Enabled = false;

                // TODO: What are failure responses?
                if ((response == null) || (response.Count == 0))
                    throw ErrorException.Create("SNMP Error: No response", receiver.Address);
                else
                    OnDataReceived(response[0].Data.ToString());
            }
        }
        private static IAuthenticationProvider GetAuthenticationProviderByName(string authentication, string phrase)
        {
            if (authentication.ToUpperInvariant() == "MD5")
                return new MD5AuthenticationProvider(new OctetString(phrase));

            if (authentication.ToUpperInvariant() == "SHA")
                return new SHA1AuthenticationProvider(new OctetString(phrase));

            throw new ArgumentException("Unknown authentication name");
        }
        private ProviderPair SecurityCheck(Arguments args)
        {
            if (string.IsNullOrEmpty(args.UserName))
                throw new InvalidOperationException("User name needs to be specified for v3.");

            IAuthenticationProvider auth = (args.Level & Levels.Authentication) == Levels.Authentication
                                               ? GetAuthenticationProviderByName(args.Authentication, args.AuthPhrase)
                                               : DefaultAuthenticationProvider.Instance;

            IPrivacyProvider priv = (args.Level & Levels.Privacy) == Levels.Privacy
                                        ? new DESPrivacyProvider(new OctetString(args.PrivPhrase), auth)
                                        : DefaultPrivacyProvider.Instance;

            return new ProviderPair(auth, priv);
        }
        private ISnmpMessage IssueCommand(Arguments args, IPEndPoint receiver, ProviderPair record, List<Variable> variables)
        {
            Discovery discovery = new Discovery(1, 101);
            ReportMessage report = discovery.GetResponse(args.Timeout, receiver);

            if (args.Command == Command.Get)
                return new GetRequestMessage(VersionCode.V3, 100, 0, new OctetString(args.UserName), variables, record, report);
            else
                return new SetRequestMessage(VersionCode.V3, 100, 0, new OctetString(args.UserName), variables, record, report);
        }

        protected override void Initialize() { }
        public SnmpPort()
            : base()
        {
            Initialize();
            State = PortState.Created;
        }
        public SnmpPort(string name)
            : base()
        {
            Initialize();
            Name = name;
            State = PortState.Created;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
        protected SnmpPort(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
        public override void Open()
        {
            State = PortState.Opened;
        }
        public override void Close()
        {
            if (timeoutTimer != null)
                timeoutTimer.Enabled = false;

            State = PortState.Closed;
        }
        public override bool Transmit(string message, bool pack)
        {
            try
            {
                Write(message);
                OnDataTransmitted(message);
                return true;
            }
            catch (Exception ex)
            {
                SignalError("SNMP Error during Transmit: " + ex.Message);
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
            Arguments args = new Arguments(this);
            OptionSet optionSet = CreateOptionSet(args);
            List<string> values = ParseCommand(optionSet, command);

            IPAddress address = ParseAddress(values[0]);
            if (args.Command == Command.Set)
                SetParameterCheck(values);
            List<Variable> variables = BuildVariableList(args.Command, values);
            IPEndPoint receiver = CreateReceiver(address);
            if (args.Version != VersionCode.V3)
            {
                ProcessOldVersion(args, receiver, variables);
            }
            else
            {
                ProviderPair record = SecurityCheck(args);
                ISnmpMessage request = IssueCommand(args, receiver, record, variables);
                ProcessResponse(args, request, receiver);
            }
        }
        protected void ProcessResponse(Arguments args, ISnmpMessage request, IPEndPoint receiver)
        {
            ISnmpMessage response = Read(args, request, receiver);
            if (response == null)
                throw ErrorException.Create("SNMP Error: No response", receiver.Address);
            else if (response.Pdu.ErrorStatus.ToInt32() != 0)
                throw ErrorException.Create("SNMP Error: Invalid response", receiver.Address, response);
            else if ((args.Command == Command.Get) && (response.Pdu.Variables.Count > 0))
                OnDataReceived(response.Pdu.Variables[0].Data.ToString());
        }
        protected ISnmpMessage Read(Arguments args, ISnmpMessage request, IPEndPoint receiver)
        {
            timeoutTimer.Interval = args.Timeout;
            timeoutTimer.Enabled = true;

            ISnmpMessage response = null;
            if (args.Command == Command.Get)
                response = ((GetRequestMessage)request).GetResponse(args.Timeout, receiver);
            else
                response = ((SetRequestMessage)request).GetResponse(args.Timeout, receiver);

            if (!timeoutTimer.Enabled)
                throw ErrorException.Create("SNMP Error: Timeout while waiting for response.", receiver.Address);
            else
                timeoutTimer.Enabled = false;

            return response;
        }
    }
}