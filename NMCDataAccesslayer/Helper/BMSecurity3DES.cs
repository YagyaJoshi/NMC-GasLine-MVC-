using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace NMCDataAccesslayer.Helper
{
    public class BMSecurity3DES
    {
        private byte[] TripleDesEncKey = ASCIIEncoding.ASCII.GetBytes("729269BZME228482"); //16bit key

        public BMSecurity3DES(string p_16BitKey = "")
        {
            if (p_16BitKey.Trim().Length == 16)//16bit key
            {
                TripleDesEncKey = ASCIIEncoding.ASCII.GetBytes(p_16BitKey);
            }
        }//end of constructor
        public string Encrypt_(string p_InputStr)
        {
            string ret = p_InputStr;
            if (String.IsNullOrEmpty(p_InputStr))
            {
                ret = "The string which needs to be encrypted can not be null.";
                return ret;
            }
            if (TryTripleDESDecrypt(p_InputStr) == false)
            {
                TripleDESCryptoServiceProvider l_cryptoProvider = new TripleDESCryptoServiceProvider();
                MemoryStream l_memoryStream = new MemoryStream();
                try
                {
                    CryptoStream l_cryptoStream = new CryptoStream(l_memoryStream, l_cryptoProvider.CreateEncryptor(TripleDesEncKey, TripleDesEncKey), CryptoStreamMode.Write);
                    StreamWriter l_writer = new StreamWriter(l_cryptoStream);
                    l_writer.Write(p_InputStr);
                    l_writer.Flush();
                    l_cryptoStream.FlushFinalBlock();
                    l_writer.Flush();
                    ret = Convert.ToBase64String(l_memoryStream.GetBuffer(), 0, (int)l_memoryStream.Length);
                    l_cryptoStream.Dispose();
                    l_cryptoStream = null;
                    l_writer.Dispose();
                    l_writer = null;
                }
                catch (System.Exception ex)
                {
                    ret = string.Empty;
                    throw new Exception("TDEZ: " + ex.GetType() + ": " + ex.Message, ex);
                }
                finally
                {
                    l_cryptoProvider.Dispose();
                    l_cryptoProvider = null;
                    l_memoryStream.Dispose();
                    l_memoryStream = null;
                }
            }
            else
            {
                ret = string.Empty;
            }
            return ret;

        }//end of function

        public string Decrypt_(string p_InputStr)
        {
            string ret = string.Empty;
            if (String.IsNullOrEmpty(p_InputStr))
            {
                ret = "The string which needs to be encrypted can not be null.";
                return ret;
            }
            if (TryTripleDESDecrypt(p_InputStr) == false)
            {
                return string.Empty;
            }
            TripleDESCryptoServiceProvider l_cryptoProvider = new TripleDESCryptoServiceProvider();
            MemoryStream l_memoryStream = new MemoryStream(Convert.FromBase64String(p_InputStr));
            try
            {
                CryptoStream l_cryptoStream = new CryptoStream(l_memoryStream, l_cryptoProvider.CreateDecryptor(TripleDesEncKey, TripleDesEncKey), CryptoStreamMode.Read);
                StreamReader l_reader = new StreamReader(l_cryptoStream);
                ret = l_reader.ReadToEnd();
                l_cryptoStream.Dispose(); l_cryptoStream = null;
                l_reader.Dispose(); l_reader = null;
            }
            catch (System.Exception)
            {
                return string.Empty; //throw new Exception("TDEZ: " + ex.GetType() + ": " + ex.Message, ex); 
            }
            finally
            {
                l_cryptoProvider.Dispose();
                l_cryptoProvider = null;
                l_memoryStream.Dispose();
                l_memoryStream = null;
            }
            return ret;
        }//end of function

        private bool TryTripleDESDecrypt(string p_InputStr)
        {
            bool ret = true;
            try
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(p_InputStr));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(TripleDesEncKey, TripleDesEncKey), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);
                string strRet = reader.ReadToEnd();
            }
            catch (System.Exception)
            {
                ret = false;
            }
            return ret;
        }//end of function

        public void Dispose()
        { }//end of function
    }
}
