using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace PipeCI.TaskDispatcher.CenterSide
{
    public class Node : Abstractions.Node
    {
        #region Properties
        protected HttpClient Client
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
        public override bool HeartBeat()
        {
            try
            {
                var client = Client;
                client.Timeout = new TimeSpan(0, 0, 5);
                var begin = DateTime.Now;
                var task = client.PostAsync($"/api/heartbeat", new FormUrlEncodedContent(new Dictionary<string, string> { }));
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

        public override bool IsInBuilding(string id)
        {
            throw new NotImplementedException();
        }

        public override bool IsInQueue(string id)
        {
            throw new NotImplementedException();
        }

        public override bool SendTask(Abstractions.CITask task)
        {
            throw new NotImplementedException();
        }

        public override void UpdateNodeInfo()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
