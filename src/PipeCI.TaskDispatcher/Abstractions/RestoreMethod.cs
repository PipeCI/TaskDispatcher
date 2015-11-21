namespace PipeCI.TaskDispatcher.Abstractions
{
    public enum RestoreMethod
    {
        ZipArchive,
        ZipDownload,
        GitClone,
        SvnCheckOut,
        CvsCheckOut,
        TFSCheckOut
    }
}
