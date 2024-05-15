﻿using RestSharp;
using RestSharp.Serialization.Json;
using GcmSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace De.Roslan.LexofficeDotnet {
    public class RestClient {
        public string BaseUrl { get; }

        public string ApiKey { get; }

        private readonly RestSharp.RestClient _client;

        internal RestClient(string baseUrl, string apiKey) {
            this.BaseUrl = baseUrl;
            this.ApiKey = apiKey;

            _client = CreateClient();
        }


        private RestSharp.RestClient CreateClient() {
            var client = new RestSharp.RestClient(BaseUrl);
            client.AddDefaultHeader("Authorization", $"Bearer {ApiKey}");
            client.AddDefaultHeader("Accept", "application / json");
            return client;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        public IRestResponse<T> SendGetRequest<T>(string resource) {
            var req = new RestRequest(resource);
            return _client.Get<T>(req);
        }

        public IRestResponse<T> SendPutRequest<T>(string resource, object data) {
            var req = new RestRequest(resource, Method.PUT) {
                RequestFormat = DataFormat.Json
            }.AddJsonBody(data)
                .AddHeader("Content-Type", "application/json");

            var response = _client.Put<T>(req);
            return response;
        }



        public IRestResponse<T> SendPostRequest<T>(string resource, object data) {
            var req = new RestRequest(resource, Method.POST) {
                RequestFormat = DataFormat.Json,
                JsonSerializer = NewtonsoftJsonSerializer.Default,
                DateFormat = "yyyy-MM-ddT00:00:00"
            }
                .AddBody(data)
                .AddHeader("Content-Type", "application/json");

            var response = _client.Post<T>(req);
            return response;
        }

        public IRestResponse<T> SendFileUploadRequest<T>(string resource, string filePath)
        {
            var req = new RestRequest(resource, Method.POST) {
                    RequestFormat = DataFormat.Json
                }
                .AddFile("content",filePath,"voucher")
                .AddHeader("Content-Type", "multipart/form-data");

            var str = req.ToString();

            var response = _client.Post<T>(req);
            return response;
        }
    }
    
}
