using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace MediaTypeFormatters
{
    public class JpegMediaTypeFormatter : MediaTypeFormatter

    {
        private const int MAXXFER = 4*1024*1024;
        public JpegMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpeg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));

        }
        public override bool CanReadType(Type type)
        {
            return typeof (byte[]).IsAssignableFrom(type);
        }

        public override bool CanWriteType(Type type)
        {
            return typeof (byte[]).IsAssignableFrom(type);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {

            return ReadFromStreamAsync(type, readStream, content, formatterLogger, CancellationToken.None);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger,
            CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var contentLength = readStream.Length;
                var fileBytes = new byte[contentLength];
                for (int ix = 0; ix < contentLength; )
                {
                    if (cancellationToken.IsCancellationRequested) return (object) null;
                    var xferLength = contentLength - ix;
                    if (xferLength > MAXXFER) xferLength = MAXXFER;
                    ix += readStream.Read(fileBytes, ix, (int) xferLength);
                }
                return (object)fileBytes;
            }

                );

        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return WriteToStreamAsync(type, value, writeStream, content, transportContext, CancellationToken.None);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var fileBytes = value as byte[];
                if (fileBytes == null) return ;
                var contentLength = fileBytes.Length;
                for (int ix = 0; ix < contentLength;)
                {
                    if (cancellationToken.IsCancellationRequested) return;
                    var xferLength = contentLength - ix;
                    if (xferLength > MAXXFER) xferLength = MAXXFER;
                    writeStream.Write(fileBytes, ix, xferLength);
                    ix += xferLength;
                }
            }
                );
        }
    }
}
