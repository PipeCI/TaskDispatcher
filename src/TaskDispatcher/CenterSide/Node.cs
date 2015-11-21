using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class Node : Abstractions.Node
    {
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

        public int Ping { get; set; }

        public ulong LostConnectionCount { get; set; } = 1;
        #endregion

        #region Methods
        /// <summary>
        /// Abort the task which the identifier equals to id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool Abort(string id)
        {
            var client = Client;
            client.Timeout = new TimeSpan(0, 0, 5);
            var task = client.PostAsync($"/api/abort/{id}", new FormUrlEncodedContent(new Dictionary<string, string> {}));
            task.Wait();
            var result = task.Result;
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        public Task<bool> AbortAsync(string id)
        {
            return Task.Factory.StartNew(() => Abort(id));
        }

        /// <summary>
        /// Test the node is online or offline
        /// </summary>
        /// <returns></returns>
        public bool HeartBeat()
        {
            try
            {
                var client = Client;
                client.Timeout = new TimeSpan(0, 0, 5);
                var begin = DateTime.Now;
                var task = client.PostAsync($"/api/heart-beat", new FormUrlEncodedContent(new Dictionary<string, string> { }));
                task.Wait();
                var result = task.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var task2 = result.Content.ReadAsStringAsync();
                    task2.Wait();
                    var json = JsonConvert.DeserializeObject<Abstractions.NodeInfo>(task2.Result);
                    this.OS = json.OS;
                    this.CurrentTaskCount = json.CurrentTaskCount;
                    this.QueuedTaskCount = json.CurrentTaskCount;
                    this.Ping = Convert.ToInt32((DateTime.Now - begin).TotalMilliseconds);
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> HeartBeatAsync()
        {
            return Task.Factory.StartNew(() => HeartBeat());
        }

        /// <summary>
        /// Check the task which is in building, is in this node or not.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool IsInBuilding(string id)
        {
            var client = Client;
            client.Timeout = new TimeSpan(0, 0, 5);
            var task = client.PostAsync($"/api/is-in-building/{id}", new FormUrlEncodedContent(new Dictionary<string, string> { }));
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

        public Task<bool> IsInBuildingAsync(string id)
        {
            return Task.Factory.StartNew(()=> IsInBuilding(id));
        }

        /// <summary>
        ///  Check the task which is in the queue of this node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool IsInQueue(string id)
        {
            var client = Client;
            client.Timeout = new TimeSpan(0, 0, 5);
            var task = client.PostAsync($"/api/is-in-queue/{id}", new FormUrlEncodedContent(new Dictionary<string, string> { }));
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

        public Task<bool> IsInQueueAsync(string id)
        {
            return Task.Factory.StartNew(() => IsInQueue(id));
        }

        public Task<bool> IsInNodeAsync(string id)
        {
            return Task.Factory.StartNew(() => IsInNode(id));
        }

        /// <summary>
        /// Send the task into this node
        /// </summary>
        /// <param name="citask"></param>
        /// <returns></returns>
        public bool SendTask(Abstractions.CITask citask)
        {
            var client = Client;
            using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss")))
            {
                if (citask.ZipArchive != null && citask.ZipArchive.Length > 0)
                    content.Add(new StreamContent(new MemoryStream(citask.ZipArchive)), "ZipArchive", "archive.zip");
                content.Add(new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", citask.Id },
                    { "Version", citask.Version },
                    { "Uri", citask.Uri },
                    { "Branch", citask.Branch },
                    { "RestoreMethod", citask.RestoreMethod.ToString() },
                    { "Dependency", citask.Dependency },
                    { "LastYamlHash", citask.LastYmlHash }
                }));
                var task = client.PostAsync("/api/run-task", content);
                task.Wait();
                var result = task.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
        }

        public Task<bool> SendTaskAsync(CITask citask)
        {
            return Task.Factory.StartNew(() => SendTask(citask));
        }


        #endregion

        #region Events
        #endregion
    }
}
