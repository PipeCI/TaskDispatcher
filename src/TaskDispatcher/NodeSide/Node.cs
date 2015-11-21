using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using PipeCI.TaskDispatcher.Abstractions;
using PipeCI.TaskDispatcher.NodeSide.EventArgs;

namespace PipeCI.TaskDispatcher.NodeSide
{
    public class Node : Abstractions.Node
    {
        #region Properties
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        protected virtual HttpClient Client
        {
            get
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri($"http://{ServerAddress}:{ServerPort}");
                client.DefaultRequestHeaders.Add("private-key", PrivateKey);
                return client;
            }
        }
        public virtual CITaskQueue Queued { get; protected set; } = new CITaskQueue();
        public virtual CITaskQueue Building { get; protected set; } = new CITaskQueue();
        #endregion

        #region Methods
        /// <summary>
        /// Push a CI task into the tasks queue.
        /// </summary>
        /// <param name="task"></param>
        public void PushTask(CITask task)
        {
            task.Node = this;
            Queued.Enqueue(task);
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
                { "type", output.Type.ToString() }
            }));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
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
        #endregion

        #region Events
        public static event Action<object, TaskPushedEventArgs> OnTaskPushed;
        #endregion
    }
}
