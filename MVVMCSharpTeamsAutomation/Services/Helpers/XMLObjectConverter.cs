using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MVVMCSharpTeamsAutomation.Model.Helpers
{
    /// <summary>
    /// Static class for conversion of model objects into XML and vice versa.
    /// Stored XML elements are encrypted after saving and decrypted before loading.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class XMLObjectConverter<T>
    {
        /// <summary>
        /// Converts a generic object into an XML representation.
        /// </summary>
        /// <param name="input">Object to convert.</param>
        /// <param name="filename">Filename of created/overwritten .xml file containing results.</param>
        /// <param name="elementToEncrypt">Name of an element to encrypt after the conversion.</param>
        public static void ConvertToXML(T input, string filename, string elementToEncrypt)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (Stream reader = new FileStream(filename, FileMode.Create))
            {
                xmlSerializer.Serialize(reader, input);
            }

            XMLCrypt.Encrypt(filename, elementToEncrypt, elementToEncrypt, elementToEncrypt);

        }

        /// <summary>
        /// Converts encrypted XML document into corresponding object.
        /// </summary>
        /// <param name="filename">Filename of an .xml file to decrypt and deserialize.</param>
        /// <param name="elementToDecrypt">Name of an element to decrypt.</param>
        /// <returns></returns>
        /// <exception cref="XmlException">Exception is thrown when any kind of problem happens in the proccess of conversion e.g. file or searched element is not found</exception>
        public static T ConvertFromXML(string filename, string elementToDecrypt)
        {

           XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            try
            {
                XMLCrypt.Decrypt(filename, elementToDecrypt);
                using (Stream reader = new FileStream(filename, FileMode.Open))
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
            catch(Exception)
            {
                throw new XmlException("XML Operation failed");
            }
       
        }

    }
}
