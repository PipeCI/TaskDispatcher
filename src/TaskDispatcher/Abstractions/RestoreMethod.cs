using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
