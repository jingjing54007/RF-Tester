using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using TruePosition.Test.DataLayer;
using TruePosition.Test.IO;
using TruePosition.Test.Process;

namespace TruePosition.Test.UI
{
    /// <summary>
    /// Helper for simplifying global and device configuration
    /// </summary>
    public sealed class ConfigManager
    {
        private static string Location = Environment.CurrentDirectory + @"\Config";

        /// <summary>
        /// Provide an alternate configuration location. Default='Executable Path\Config'
        /// </summary>
        /// <param name="path">full path for configuration</param>
        public static void SetLocation(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("The path '" + path + "' could not be found.");

            Location = path;
        }

        public static bool Exists(string name)
        {
            return File.Exists(Location + @"\" + name + ".config");
        }

        private static void Save(string name, object config)
        {
            if (!Directory.Exists(Location))
                Directory.CreateDirectory(Location);

            XmlSerializer serializer = new XmlSerializer(config.GetType());
            using (FileStream stream = WriteStream(name))
            {
                serializer.Serialize(stream, config);
                stream.Flush();
                stream.Close();
            }
        }
        internal static FileStream ReadStream(string name)
        {
            return new FileStream(Location + @"\" + name + ".config", FileMode.Open, FileAccess.Read);
        }
        internal static FileStream WriteStream(string name)
        {
            return new FileStream(Location + @"\" + name + ".config", FileMode.Create, FileAccess.Write);
        }

        /// <summary>
        /// Create a new serial port configuration for a given device filled with default values.
        /// </summary>
        /// <param name="name">name of device attached to serial port</param>
        /// <param name="config">serial port configuration</param>
        public static void Create(string name, out ISerialConfig config)
        {
            if (VBHelpers.Match(name.ToLower(), "com?"))
                throw new ArgumentException("A valid device name must be provided. Devices cannot be named using COM device names (i.e. COM1, COM2, etc)", name);

            config = (ISerialConfig)PortFactory.Create(PortType.Serial, name); 
        }
        /// <summary>
        /// Load an existing serial port configuration from disk.
        /// </summary>
        /// <param name="name">name of device attached to serial port</param>
        /// <param name="config">serial port configuration</param>
        public static void Load(string name, out ISerialConfig config)
        {
            config = null;
            using (FileStream stream = ReadStream(name))
            {
                config = (ISerialConfig)PortFactory.Create(PortType.Serial, stream);
            }
        }
        /// <summary>
        /// Save the serial port configuration for the given device.
        /// </summary>
        /// <param name="name">name of device attached to serial port</param>
        /// <param name="config">serial port configuration</param>
        public static void Save(string name, ISerialConfig config)
        {
            Save(name, (object)config);
        }

        /// <summary>
        /// Create a new telnet port configuration for a given device filled with default values.
        /// </summary>
        /// <param name="name">name of device attached to telnet port</param>
        /// <param name="config">telnet port configuration</param>
        public static void Create(string name, out ITelnetConfig config)
        {
            config = (ITelnetConfig)PortFactory.Create(PortType.Telnet, name);
        }
        /// <summary>
        /// Load an existing telnet port configuration from disk.
        /// </summary>
        /// <param name="name">name of device attached to telnet port</param>
        /// <param name="config">telnet port configuration</param>
        public static void Load(string name, out ITelnetConfig config)
        {
            config = null;
            using (FileStream stream = ReadStream(name))
            {
                config = (ITelnetConfig)PortFactory.Create(PortType.Telnet, stream);
            }
        }
        /// <summary>
        /// Save the telnet port configuration for the given device.
        /// </summary>
        /// <param name="name">name of device attached to telnet port</param>
        /// <param name="config">telnet port configuration</param>
        public static void Save(string name, ITelnetConfig config)
        {
            Save(name, (object)config);
        }

        /// <summary>
        /// Create a new GPIB port configuration for a given device filled with default values.
        /// </summary>
        /// <param name="name">name of device attached to GPIB port</param>
        /// <param name="config">GPIB port configuration</param>
        public static void Create(string name, out IGpibConfig config)
        {
            config = (IGpibConfig)PortFactory.Create(PortType.Gpib, name);
        }
        /// <summary>
        /// Load an existing GPIB port configuration from disk.
        /// </summary>
        /// <param name="name">name of device attached to GPIB port</param>
        /// <param name="config">GPIB port configuration</param>
        public static void Load(string name, out IGpibConfig config)
        {
            config = null;
            using (FileStream stream = ReadStream(name))
            {
                config = (IGpibConfig)PortFactory.Create(PortType.Gpib, stream);
            }
        }
        /// <summary>
        /// Save the GPIB port configuration for the given device.
        /// </summary>
        /// <param name="name">name of device attached to GPIB port</param>
        /// <param name="config">GPIB port configuration</param>
        public static void Save(string name, IGpibConfig config)
        {
            Save(name, (object)config);
        }

        /// <summary>
        /// Create a new windows process configuration filled with default values.
        /// </summary>
        /// <param name="name">display name for process</param>
        /// <param name="config">windows process configuration</param>
        public static void Create(string name, out IProcessConfig config)
        {
            config = (IProcessConfig)ProcessFactory.Create(name);
        }
        /// <summary>
        /// Load an existing windows process configuration from disk.
        /// </summary>
        /// <param name="name">display name for process</param>
        /// <param name="config">windows process configuration</param>
        public static void Load(string name, out IProcessConfig config)
        {
            config = null;
            using (FileStream stream = ReadStream(name))
            {
                config = (IProcessConfig)ProcessFactory.Create(stream);
            }
        }
        /// <summary>
        /// Save the windows process configuration for the given process.
        /// </summary>
        /// <param name="name">display name for process</param>
        /// <param name="config">windows process configuration</param>
        public static void Save(string name, IProcessConfig config)
        {
            Save(name, (object)config);
        }

        /// <summary>
        /// Create a new global settings configuration filled with default values.
        /// </summary>
        /// <param name="config">global settings configuration</param>
        public static void Create(out IGlobalConfig config)
        {
            config = new GlobalSettings();
        }
        /// <summary>
        /// Load an existing global settings configuration from disk.
        /// </summary>
        /// <param name="config">global settings configuration</param>
        public static void Load(out IGlobalConfig config)
        {
            config = null;
            using (FileStream stream = ReadStream("GlobalSettings"))
            {
                if (stream == null)
                    throw new ArgumentNullException("stream", "A valid stream must be provided.");

                XmlSerializer serializer = new XmlSerializer(typeof(GlobalSettings));
                config = (IGlobalConfig)serializer.Deserialize(stream);
                stream.Dispose();
            }
        }
        /// <summary>
        /// Save the global settings configuration for the given process.
        /// </summary>
        /// <param name="config">global settings configuration</param>
        public static void Save(IGlobalConfig config)
        {
            Save("GlobalSettings", (object)config);
        }
    }
}
