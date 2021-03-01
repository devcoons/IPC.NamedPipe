using System;
using System.Collections.Generic;
using System.Text;

namespace IPC.NamedPipe
{
    public enum PipeMessageType
    {
        PMTString = 0x01,
        PMTByte = 0x02,
        PMTUnknown = 0x03
    }


    public class PipeMessage
    {
        private int size = 0;
        private byte[] payload = null;
        private int type = -1;

        public void SetPayload(Type dataType, object data)
        {
            if(dataType == typeof(System.String))
            {
                payload = Encoding.UTF8.GetBytes((string)data);
                type = 1;
            }
            else if (dataType == typeof(System.Byte[]))
            {
                payload = (byte[])Convert.ChangeType(data, typeof(System.Byte[]));
                type = 2;
            }
            else
            {
                throw new NotSupportedException("This datatype is not supported");
            }

            size = payload.Length;
        }


        public object GetPayload()
        {
            if (type == 1)
                return (string)Encoding.UTF8.GetString(payload, 0, payload.Length);
            if (type == 2)
                return (byte[])payload;
            throw new NotSupportedException("No Payload Available");
        }

        public void ConvertToString()
        {
            type = 1;
        }
        public void ConvertToBytes()
        {
            type = 2;
        }

        public byte[] GetHeader()
        {
            if(type != 1 && type != 2)
                throw new NotSupportedException("No Payload Available");

            byte[] result = new byte[6];

            result[0] = (byte)((size & 0x0000FF00) >> 8);
            result[1] = (byte)((size & 0x000000FF) >> 0);
            result[2] = (byte)(type);

            result[3] = (byte)(result[2] + ((result[0] ^ result[1])));

            return result;
        }

        public byte[] GetSerialized()
        {
            byte[] header = GetHeader();
            byte[] rv = new byte[header.Length + payload.Length];
            System.Buffer.BlockCopy(header, 0, rv, 0, header.Length);
            System.Buffer.BlockCopy(payload, 0, rv, header.Length, payload.Length);
            return rv;
        }

        public int GetSize()
        {
            return size;
        }

        public PipeMessageType GetPayloadType()
        {
            return type == 1 ? PipeMessageType.PMTString : (type == 2 ? PipeMessageType.PMTByte : PipeMessageType.PMTUnknown);
        }

    }
}
