using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using PipeCI.TaskDispatcher.Abstractions;
using PipeCI.TaskDispatcher.NodeSide.EventArgs;

namespace PipeCI.TaskDispatcher.NodeSide
{
    public class CITask : Abstractions.CITask, IDisposable
    {
        #region Properties
        public string StandardOutput { get; set; } = "";
        public string StandardError { get; set; } = "";
        public string StandardMixed { get; set; } = "";
        public List<Process> Processes { get; set; }
        public ulong WaitingCount { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Begin execute this task.
        /// </summary>
        /// <param name="execute"></param>
        public void Execute(Action execute = null)
        {
            if (execute != null)
                execute();
            OnTaskFinished(this, new TaskFinishedEventArgs { Id = this.Id });
        }

        public void Dispose()
        {
            foreach (var x in Processes)
            {
                try
                {
                    x.Kill();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Create a process and run it.
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Arguments"></param>
        /// <param name="TimeLimit"></param>
        /// <param name="WorkingDirectory"></param>
        /// <param name="AdditionalEnvironmentVariables"></param>
        /// <returns></returns>
        public Task<bool> RunAsync(string Filename, string Arguments = null, int TimeLimit = 0, string WorkingDirectory = null, IDictionary<string, string> AdditionalEnvironmentVariables = null)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var process = new Process())
                    {
                        if (OnTaskStartBuilding != null)
                            OnTaskStartBuilding(this, new TaskStartBuildingEventArgs { Id = this.Id, Time = DateTime.Now });
                        process.StartInfo.FileName = Filename;
                        process.StartInfo.Arguments = Arguments;
                        process.StartInfo.WorkingDirectory = WorkingDirectory;
                        if (AdditionalEnvironmentVariables != null)
                        {
#if NET451
                            foreach (var x in AdditionalEnvironmentVariables)
                            {
                                if (process.StartInfo.EnvironmentVariables.ContainsKey(x.Key))
                                    process.StartInfo.EnvironmentVariables[x.Key] = process.StartInfo.EnvironmentVariables[x.Key].TrimEnd(' ').TrimEnd(';') + ';' + x.Value;
                                else
                                    process.StartInfo.EnvironmentVariables.Add(x.Key, x.Value);
                            }
#else
                    foreach (var x in AdditionalEnvironmentVariables)
                    {
                        if (process.StartInfo.Environment.ContainsKey(x.Key))
                            process.StartInfo.Environment[x.Key] = process.StartInfo.Environment[x.Key].TrimEnd(' ').TrimEnd(';') + ';' + x.Value;
                        else
                            process.StartInfo.Environment.Add(x.Key, x.Value);
                    }
#endif
                        }
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardInput = true;
                        process.OutputDataReceived += (sender, e) =>
                        {
                            StandardMixed += e.Data + Environment.NewLine;
                            StandardOutput += e.Data + Environment.NewLine;
                            if (OnTaskProcessOutputed != null)
                                OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Text = e.Data, Time = DateTime.Now, Type = OutputType.Output });
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            StandardMixed += e.Data + Environment.NewLine;
                            StandardError += e.Data + Environment.NewLine;
                            if (OnTaskProcessOutputed != null)
                                OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Text = e.Data, Time = DateTime.Now, Type = OutputType.Error });
                        };
                        Processes.Add(process);
                        process.Start();
                        if (TimeLimit == 0)
                            process.WaitForExit();
                        else
                        {
                            if (!process.WaitForExit(TimeLimit))
                            {
                                process.Kill();
                                OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Time = DateTime.Now, Type = OutputType.Error, Text = $"The process has been killed, because the time which cost by the process is so long. The server limited time of this process to be {TimeLimit} ms." });
                                OnTaskProcessExecuteFailed(this, new TaskProcessExecuteFailed { Error = $"The process has been killed, because the time which cost by the process is so long. The server limited time of this process to be {TimeLimit} ms.", Id = this.Id, Time = DateTime.Now });
                                Processes.Remove(process);
                                return false;
                            }
                        }
                        if (process.ExitCode == 0)
                        {
                            OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Time = DateTime.Now, Type = OutputType.Successful, Text = $"The process has exited with the code 0." });
                            OnTaskFinished(this, new TaskFinishedEventArgs { Id = this.Id, Time = DateTime.Now });
                            Processes.Remove(process);
                            return true;
                        }
                        else
                        {
                            OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Time = DateTime.Now, Type = OutputType.Successful, Text = $"The process has exited with the code {process.ExitCode}." });
                            OnTaskProcessExecuteFailed(this, new TaskProcessExecuteFailed { Error = $"{StandardError}\r\nThe process has exited with the code {process.ExitCode}.", Id = this.Id, Time = DateTime.Now });
                            Processes.Remove(process);
                            return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    OnTaskProcessOutputed(this, new TaskProcessOutputedEventArgs { TaskId = this.Id, Time = DateTime.Now, Type = OutputType.Successful, Text = $"The process has crashed with the following messages: \r\n" + e.ToString() });
                    OnTaskProcessExecuteFailed(this, new TaskProcessExecuteFailed { Error = e.ToString(), Id = this.Id, Time = DateTime.Now });
                    return false;
                }
            });
        }
        #endregion

        #region Events
        public static event Action<object, TaskStartBuildingEventArgs> OnTaskStartBuilding;
        public static event Action<object, TaskFinishedEventArgs> OnTaskFinished;
        public static event Action<object, TaskProcessOutputedEventArgs> OnTaskProcessOutputed;
        public static event Action<object, TaskProcessExecuteFailed> OnTaskProcessExecuteFailed;
        #endregion
    }
}
