using System;
using System.Text;
using System.IO;

using Com.Huen.Libs;

namespace Com.Huen.Sockets
{
    public class Header
    {
        public Int32 Cmd { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }

        public void Serialize(Stream stream)
        {
            FileTransferHeader _headStruct = new FileTransferHeader()
            {
                Cmd = this.Cmd
                ,
                FileName = this.FileName
                ,
                FileSize = this.FileSize
            };

            byte[] buffer = util.GetBytes(_headStruct);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static Header Deserialize(Stream stream)
        {
            Header header = new Header();

            FileTransferHeader _headStruct = new FileTransferHeader();
            byte[] buffer = util.GetBytes(_headStruct);
            int read = stream.Read(buffer, 0, buffer.Length);

            _headStruct = util.GetObject<FileTransferHeader>(buffer);

            header.Cmd = _headStruct.Cmd;
            header.FileName = _headStruct.FileName;
            header.FileSize = _headStruct.FileSize;

            return header;
        }
    }
}
