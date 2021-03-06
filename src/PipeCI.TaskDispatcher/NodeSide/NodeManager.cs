﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using PipeCI.TaskDispatcher.Abstractions;
using PipeCI.TaskDispatcher.NodeSide.EventArgs;

namespace PipeCI.TaskDispatcher.NodeSide
{
    public class NodeManager : Node
    {
        #region Constructor
        public NodeManager(IConfiguration config)
        {
            this.Address = config["Address"];
            this.Port = Convert.ToInt32(config["Port"]);
            this.MaxThreadsCount = Convert.ToInt32(config["MaxThreadsCount"]);
            this.PrivateKey = config["PrivateKey"];
            OnTaskPushed += NodeManager_OnTaskPushed;
            CITask.OnTaskProcessOutputed += (sender, e) => Task.Factory.StartNew(() =>
            {
                Output(e);
            });
            CITask.OnTaskProcessExecuteFailed += (sender, e) => Task.Factory.StartNew(() =>
            {
                UpdateStatus(e.Id, CITaskStatus.Failing, e.Time);
                var task = sender as CITask;
                task.Dispose();
                Building.Remove(task);
                HandleResourceFree();
            });
            CITask.OnTaskFinished += (sender, e) => Task.Factory.StartNew(() =>
            {
                UpdateStatus(e.Id, CITaskStatus.Passing, e.Time);
                var task = sender as CITask;
                task.Dispose();
                Building.Remove(task);
                HandleResourceFree();
            });
        }
        #endregion

        #region Properties
        protected virtual HttpClient Client
        {
            get
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri($"http://{Address}:{Port}");
                client.DefaultRequestHeaders.Add("private-key", PrivateKey);
                return client;
            }
        }
        public virtual CITaskQueue Queued { get; protected set; } = new CITaskQueue();
        public virtual CITaskQueue Building { get; protected set; } = new CITaskQueue();
        public override int CurrentTaskCount
        {
            get
            {
                return Building.Count;
            }
            set
            {
                throw new NotSupportedException("Building task count is a read-only variable.");
            }
        }
        public override int QueuedTaskCount
        {
            get
            {
                return Queued.Count;
            }
            set
            {
                throw new NotSupportedException("Queued task count is a read-only variable.");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Detecting the task dependence relation.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool IsAbleToRun(CITask task)
        {
            if (string.IsNullOrEmpty(task.Dependency))
                return true;
            var status = CheckStatus(task.Id);
            if (status == CITaskStatus.Passing)
            {
                return true;
            }
            else if (status == CITaskStatus.Building || status == CITaskStatus.Pending)
            {
                task.WaitingCount++;
                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    Queued.Enqueue(task);
                    OnTaskPushed(this, new TaskPushedEventArgs { Id = task.Id });
                });
                return false;
            }
            else
            {
                Output(new Output { TaskId = task.Id, Text = "Due to the task on which depended by this task is failing, this task has been aborted.", Type = OutputType.Error, Time = DateTime.Now });
                UpdateStatus(task.Id, CITaskStatus.Failing, DateTime.Now);
                return false;
            }
            
        }

        /// <summary>
        /// Handle the task pushed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeManager_OnTaskPushed(object sender, TaskPushedEventArgs e)
        {
            HandleResourceFree();
        }

        /// <summary>
        /// Handle the queue when there are some free resources.
        /// </summary>
        private void HandleResourceFree()
        {
            if (Building.Count >= MaxThreadsCount || Queued.Count == 0)
                return;
            var task = Queued.Dequeue();
            if (IsAbleToRun(task))
            {
                task.Execute();
                Building.Add(task);
            }
        }

        /// <summary>
        /// Push a CI task into the tasks queue.
        /// </summary>
        /// <param name="task"></param>
        public void PushTask(CITask task)
        {
            Queued.Enqueue(task);
            OnTaskPushed(this, new TaskPushedEventArgs { Id = task.Id });
        }

        /// <summary>
        /// Abort the task which identifier equals to id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Abort(string id)
        {
            try
            {
                var task = Queued.Where(x => x.Id == id).SingleOrDefault();
                if (task != null)
                {
                    Queued.Remove(task);
                }
                task = Building.Where(x => x.Id == id).SingleOrDefault();
                if (task != null)
                {
                    Building.Remove(task);
                    task.Dispose();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check the task which is in building, is in this node or not.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool IsInBuilding(string id)
        {
            if (Building.Any(x => x.Id == id))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check the task which is in the queue of this node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool IsInQueue(string id)
        {
            if (Queued.Any(x => x.Id == id))
                return true;
            else
                return false;
        }

        public bool SendYmlHash(string id, string hash)
        {
            var client = Client;
            var task = client.PostAsync($"/api-node/hash-yml/{id}", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "hash", hash }
            }));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                ErrorCount++;
                return false;
            }
        }

        /// <summary>
        /// Send the outputs of process to the center server.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool Output(Output output)
        {
            var client = Client;
            var task = client.PostAsync($"/api-node/output/{output.TaskId}", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "text", output.Text },
                { "time", output.Time.ToString("yyyy-MM-dd HH:mm:ss.ffffff") },
                { "type", output.Type.ToString() },
                { "os", OS.ToString() }
            }));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                ErrorCount++;
                return false;
            }
        }

        public Task<bool> OutputAsync(Output output)
        {
            return Task.Factory.StartNew(() => Output(output));
        }

        /// <summary>
        /// Update the ci task status to the center server.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateStatus(string id, CITaskStatus status, DateTime time)
        {
            var client = Client;
            var task = client.PostAsync($"/api-node/status/{id}", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "status", status.ToString() },
                { "time", time.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}
            }));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                ErrorCount++;
                return false;
            }
        }

        public Task<bool> UpdateStatusAsync(string id, CITaskStatus status, DateTime time)
        {
            return Task.Factory.StartNew(() => UpdateStatus(id, status, time));
        }

        /// <summary>
        /// Get the status of the specific task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CITaskStatus CheckStatus(string id)
        {
            var client = Client;
            var task = client.PostAsync($"/api-node/check-status/{id}", new FormUrlEncodedContent(new Dictionary<string, string> { }));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var task2 = result.Content.ReadAsStringAsync();
                task2.Wait();
                return (CITaskStatus)Enum.Parse(typeof(CITaskStatus),task2.Result);
            }
            else
            {
                ErrorCount++;
                return CITaskStatus.Error;
            }
        }

        public Task<CITaskStatus> CheckStatusAsync(string id)
        {
            return Task.Factory.StartNew(() => CheckStatus(id));
        }
        #endregion

        #region Events
        public static event Action<object, TaskPushedEventArgs> OnTaskPushed;
        #endregion
    }
}
