using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Spar
{
    public class Aes
    {
        private KeyParameter _key = null;

        public Aes(string hexKey)
        {
            var byteKey = HexStringToBytes(hexKey);
            _key = new KeyParameter(byteKey);
        }

        private static byte[] HexStringToBytes(string hex)
        {
            byte[] returnBytes = new byte[hex.Length / 2];
            for (int hexByteSize = 0; hexByteSize < returnBytes.Length; hexByteSize++)
            {
                returnBytes[hexByteSize] = Convert.ToByte(hex.Substring(hexByteSize * 2, 2), 16);
            }
            return returnBytes;
        }

        private static byte[] processCipher(PaddedBufferedBlockCipher cipher, byte[] message)
        {
            var result = new byte[cipher.GetOutputSize(message.Length)];
            var resultLength = cipher.ProcessBytes(message, 0, message.Length, result, 0);
            cipher.DoFinal(result, resultLength);
            return result;
        }

        private byte[] encode(byte[] message)
        {
            var cipher = new PaddedBufferedBlockCipher(
                new AesEngine(),
                new Pkcs7Padding());
            cipher.Init(true, _key);
            return processCipher(cipher, message);
        }

        private byte[] decode(byte[] message)
        {
            var cipher = new PaddedBufferedBlockCipher(
                new AesEngine(),
                new ZeroBytePadding());
            cipher.Init(false, _key);
            return /*removePaddingBytes(*/processCipher(cipher, message);//);
        }

        public byte[] Encode(string message)
        {
            return encode(Encoding.ASCII.GetBytes(message));
        }

        public string Decode(byte[] message)
        {
            return Encoding.ASCII.GetString(decode(message));
        }
    }
}
