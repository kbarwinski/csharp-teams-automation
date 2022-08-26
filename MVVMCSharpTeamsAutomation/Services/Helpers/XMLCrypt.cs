using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MVVMCSharpTeamsAutomation.Model.Helpers
{
    /// <summary>
    /// Class instantiated by XMLObjectConverter methods, after saving and before loading XML files containing program's data.
    /// It's purpose is  to encrypt and decrypt XML elements with the use of AES cipher, as info provided to the program is sensitive.
    /// </summary>
    public class XMLCrypt
    {
        /// <summary>
        /// Nested class containing opened XML document and private AES key.
        /// Serves as an entry point of every encryption or decryption.
        /// </summary>
        private class XMLInitObject
        {
            internal XmlDocument xmlDoc;
            internal RSACryptoServiceProvider rsaKey;

            /// <summary>
            /// Constructor opens a XML file and loads it to XMLDocument object.
            /// After loading the XML private AES key is loaded from the safe storage for further use.
            /// </summary>
            /// <param name="filename">Filename of .xml file to load</param>
            internal XMLInitObject(string filename)
            {
                this.xmlDoc = new XmlDocument();

                try
                {
                    this.xmlDoc.PreserveWhitespace = true;
                    this.xmlDoc.Load(filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                CspParameters cspParams = new CspParameters();
                cspParams.KeyContainerName = "XML_GENERATED_KEY";

                this.rsaKey = new RSACryptoServiceProvider(cspParams);
            }
        }

        /// <summary>
        /// Encrypts an XML element and saves the results.
        /// </summary>
        /// <param name="filename">Filename of .xml file to load into XMLInitObject</param>
        /// <param name="ElementToEncrypt">name of XML element to encrypt.</param>
        /// <param name="EncryptionElementID">ID of an element after encryption</param>
        /// <param name="KeyName"></param>
        /// <exception cref="ArgumentNullException">Is thrown when null is passed to any of method's parameters or XML file does not exist.</exception>
        /// <exception cref="XmlException">Is thrown specified element is not found in XML file.</exception>
        public static void Encrypt(string filename, string ElementToEncrypt, string EncryptionElementID, string KeyName)
        {
            XMLInitObject xmlInfo = new XMLInitObject(filename);
            XmlDocument Doc = xmlInfo.xmlDoc;
            RSA Alg = xmlInfo.rsaKey;

            // Check the arguments.
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementToEncrypt == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (EncryptionElementID == null)
                throw new ArgumentNullException("EncryptionElementID");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            ////////////////////////////////////////////////
            // Find the specified element in the XmlDocument
            // object and create a new XmlElement object.
            ////////////////////////////////////////////////
            XmlElement elementToEncrypt = Doc.GetElementsByTagName(ElementToEncrypt)[0] as XmlElement;

            // Throw an XmlException if the element was not found.
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }
            Aes sessionKey = null;

            try
            {
                //////////////////////////////////////////////////
                // Create a new instance of the EncryptedXml class
                // and use it to encrypt the XmlElement with the
                // a new random symmetric key.
                //////////////////////////////////////////////////

                // Create an AES key.
                sessionKey = Aes.Create();

                EncryptedXml eXml = new EncryptedXml();

                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);
                ////////////////////////////////////////////////
                // Construct an EncryptedData object and populate
                // it with the desired encryption information.
                ////////////////////////////////////////////////

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = EncryptionElementID;
                // Create an EncryptionMethod element so that the
                // receiver knows which algorithm to use for decryption.

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                // Encrypt the session key and add it to an EncryptedKey element.
                EncryptedKey ek = new EncryptedKey();

                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, Alg, false);

                ek.CipherData = new CipherData(encryptedKey);

                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

                // Create a new DataReference element
                // for the KeyInfo element.  This optional
                // element specifies which EncryptedData
                // uses this key.  An XML document can have
                // multiple EncryptedData elements that use
                // different keys.
                DataReference dRef = new DataReference();

                // Specify the EncryptedData URI.
                dRef.Uri = "#" + EncryptionElementID;

                // Add the DataReference to the EncryptedKey.
                ek.AddReference(dRef);
                // Add the encrypted key to the
                // EncryptedData object.

                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                // Set the KeyInfo element to specify the
                // name of the RSA key.


                // Create a new KeyInfoName element.
                KeyInfoName kin = new KeyInfoName();

                // Specify a name for the key.
                kin.Value = KeyName;

                // Add the KeyInfoName element to the
                // EncryptedKey object.
                ek.KeyInfo.AddClause(kin);
                // Add the encrypted element data to the
                // EncryptedData object.
                edElement.CipherData.CipherValue = encryptedElement;
                ////////////////////////////////////////////////////
                // Replace the element from the original XmlDocument
                // object with the EncryptedData element.
                ////////////////////////////////////////////////////
                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            }
            catch (Exception e)
            {
                // re-throw the exception.
                throw e;
            }
            finally
            {
                if (sessionKey != null)
                {
                    sessionKey.Clear();
                }
                Alg.Clear();
                Doc.Save(filename);
            }
        }

        /// <summary>
        /// Decrypts an XML element and saves the results.
        /// </summary>
        /// <param name="filename">Filename of .xml file to load into XMLInitObject</param>
        /// <param name="KeyName">Keyname of an element to decrypt</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Decrypt(string filename, string KeyName)
        {
            XMLInitObject xmlInfo = new XMLInitObject(filename);
            XmlDocument Doc = xmlInfo.xmlDoc;
            RSA Alg = xmlInfo.rsaKey;

            // Check the arguments.
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(Doc);

            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping(KeyName, Alg);

            // Decrypt the element.
            exml.DecryptDocument();

            Alg.Clear();
            Doc.Save(filename);
        }
    }
 
}
