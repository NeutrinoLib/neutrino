﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Neutrino.Entities;

namespace Neutrino.Client
{
    public class NeutrinoClient : INeutrinoClient
    {
        private readonly IHttpRequestService _httpRequestService;
        
        private const string KvPropertiesEndpointPath = "/api/key-values";
        private const string ServicesEndpointPath = "/api/services";

        public NeutrinoClient(IHttpRequestService httpRequestService)
        {
            _httpRequestService = httpRequestService;
        }

        public async Task<ActionConfirmation<KvProperty>> AddKvPropertyAsync(KvProperty kvProperty)
        {
            return await _httpRequestService.SendRequest<KvProperty>(HttpMethod.Post, KvPropertiesEndpointPath, kvProperty);
        }

        public async Task<ActionConfirmation<Service>> AddServiceAsync(Service service)
        {
            return await _httpRequestService.SendRequest<Service>(HttpMethod.Post, ServicesEndpointPath, service);
        }

        public async Task<ActionConfirmation> DeleteKvPropertyAsync(string key)
        {
            return await _httpRequestService.SendRequest(HttpMethod.Delete, KvPropertiesEndpointPath, key);
        }

        public async Task<ActionConfirmation> DeleteServiceAsync(string id)
        {
            return await _httpRequestService.SendRequest(HttpMethod.Delete, ServicesEndpointPath, id);
        }

        public async Task<KvProperty> GetKvPropertyAsync(string key)
        {
            var actionConfirmation = await _httpRequestService.SendRequest<KvProperty>(HttpMethod.Get, KvPropertiesEndpointPath, key);
            return actionConfirmation.ObjectData;
        }

        public async Task<ServiceHealth> GetLastServiceHealthAsync(string id)
        {
            var actionConfirmation = await _httpRequestService.SendRequest<ServiceHealth>(HttpMethod.Get, ServicesEndpointPath, id, "/health/current");
            return actionConfirmation.ObjectData;
        }

        public async Task<Service> GetServiceByIdAsync(string id)
        {
            var actionConfirmation = await _httpRequestService.SendRequest<Service>(HttpMethod.Get, ServicesEndpointPath, id);
            return actionConfirmation.ObjectData;
        }

        public async Task<IList<ServiceHealth>> GetServiceHealthAsync(string id)
        {
            var actionConfirmation = await _httpRequestService.SendRequest<IList<ServiceHealth>>(HttpMethod.Get, ServicesEndpointPath, id, "/health");
            return actionConfirmation.ObjectData;
        }

        public Task<PageList<ServiceHealth>> GetServiceHealthAsync(string id, int offset, int limit)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Service>> GetServicesAsync()
        {
            var actionConfirmation = await _httpRequestService.SendRequest<IList<Service>>(HttpMethod.Get, ServicesEndpointPath);
            return actionConfirmation.ObjectData;
        }

        public Task<IList<Service>> GetServicesByServiceTypeAsync(string serviceType)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Service>> GetServicesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionConfirmation> UpdateKvPropertyAsync(string key, KvProperty kvProperty)
        {
            return await _httpRequestService.SendRequest<KvProperty>(HttpMethod.Put, KvPropertiesEndpointPath, key, kvProperty);
        }

        public async Task<ActionConfirmation> UpdateServiceAsync(string id, Service service)
        {
            return await _httpRequestService.SendRequest<Service>(HttpMethod.Put, ServicesEndpointPath, id, service);
        }
    }
}
