using System.IO;
using TagLib;
using static TagLib.File;

public class StreamFileAbstraction : IFileAbstraction
{
    public string Name { get; }
    public Stream ReadStream { get; }
    public Stream WriteStream { get; }

    public StreamFileAbstraction(string name, Stream stream, Stream writeStream)
    {
        Name = name;
        ReadStream = stream;
        WriteStream = writeStream ?? stream;
    }

    public void CloseStream(Stream stream) => stream.Close();
}
