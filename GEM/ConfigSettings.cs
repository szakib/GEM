using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Configuration;
using System.Reflection;

namespace GEM
{
    /// <summary>
    /// This whole class is based on
    /// http://ryanfarley.com/blog/archive/2004/07/13/879.aspx
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigSettings"/> class from being created
        /// </summary>
        private ConfigSettings() { }

        /// <summary>
        /// Reads the setting.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns></returns>
        private static string ReadSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Writes the setting.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public static void WriteSetting(string key, string value)
        {
            // load config document for current assembly
            XmlDocument doc = loadConfigDocument();

            // retrieve appSettings node
            XmlNode node = doc.SelectSingleNode("//appSettings");

            if (node == null)
                throw new InvalidOperationException(
                    "appSettings section not found in config file.");

            try
            {
                // select the 'add' element that contains the key
                XmlElement elem = (XmlElement)node.SelectSingleNode(
                    string.Format("//add[@key='{0}']", key));

                if (elem != null)
                {
                    // add value for key
                    elem.SetAttribute("value", value);
                }
                else
                {
                    // key was not found so create the 'add' element 
                    // and set it's key/value attributes 
                    elem = doc.CreateElement("add");
                    elem.SetAttribute("key", key);
                    elem.SetAttribute("value", value);
                    node.AppendChild(elem);
                }
                doc.Save(getConfigFilePath());
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Removes the setting.
        /// </summary>
        /// <param name="key">The key</param>
        public static void RemoveSetting(string key)
        {
            // load config document for current assembly
            XmlDocument doc = loadConfigDocument();

            // retrieve appSettings node
            XmlNode node = doc.SelectSingleNode("//appSettings");

            try
            {
                if (node == null)
                    throw new InvalidOperationException(
                        "appSettings section not found in config file.");
                else
                {
                    // remove 'add' element with coresponding key
                    node.RemoveChild(node.SelectSingleNode(
                        string.Format("//add[@key='{0}']", key)));
                    doc.Save(getConfigFilePath());
                }
            }
            catch (NullReferenceException e)
            {
                throw new Exception(string.Format("The key {0} does not exist.", key), e);
            }
        }

        /// <summary>
        /// Loads the config document.
        /// </summary>
        /// <returns></returns>
        private static XmlDocument loadConfigDocument()
        {
            XmlDocument doc = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(getConfigFilePath());
                return doc;
            }
            catch (System.IO.FileNotFoundException e)
            {
                throw new Exception("No configuration file found.", e);
            }
        }

        /// <summary>
        /// Gets the config file path.
        /// </summary>
        /// <returns></returns>
        private static string getConfigFilePath()
        {
            return Assembly.GetExecutingAssembly().Location + ".config";
        }

        /// <summary>
        /// Reads an int value
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        public static int ReadInt(string key)
        {
            try
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings[key]);
            }
            catch (FormatException)
            {
                throw new Exception(key
                    + " specified in the config file is not a sequence of digits.");
            }
            catch (OverflowException)
            {
                throw new Exception(key
                    + " specified in the config file cannot fit in an Int32.");
            }
            catch (Exception e)
            {
                throw new Exception("Unknown error while reading "
                    + key + " from config file. Exception text:" + e.Message);
            }
        }

        /// <summary>
        /// Reads a double value
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        public static double ReadDouble(string key)
        {
            try
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings[key]);
            }
            catch (FormatException)
            {
                throw new Exception(key + 
                    " specified in the config file is not a well-formed double.");
            }
            catch (OverflowException)
            {
                throw new Exception(key +
                    " specified in the config file cannot fit in a double.");
            }
            catch (Exception e)
            {
                throw new Exception("Unknown error while reading "
                    + key + " from config file. Exception text:" + e.Message);
            }
        }

        /// <summary>
        /// Reads a string value
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        public static string ReadString(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception e)
            {
                throw new Exception("Unknown error while reading "
                    + key + " from config file. Exception text:" + e.Message);
            }
        }

        public static bool ReadBool(string key)
        {
            try
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings[key]);
            }
            catch (FormatException)
            {
                throw new Exception(key
                    + " value specified in the config file is not a proper boolean value.");
            }
            catch (Exception e)
            {
                throw new Exception("Unknown error while reading "
                    + key + " from config file. Exception text:" + e.Message);
            }
        }
    }
}
