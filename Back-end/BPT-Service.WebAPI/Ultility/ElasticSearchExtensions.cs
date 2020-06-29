using BPT_Service.Application.PostService.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace BPT_Service.WebAPI.Ultility
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticsearch(
            this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings
                .DefaultMappingFor<PostServiceViewModel>(m => m
                    .Ignore(p=>p.TagList)
                    .Ignore(p=>p.tagofServices)
                    .Ignore(p=>p.userofServices)
                    .Ignore(p=>p.UserId)
                    .Ignore(p=>p.serviceofProvider)
                    .Ignore(p=>p.Reason)
                    .Ignore(p=>p.ProviderId)
                    .Ignore(p=>p.listImages)
                    .Ignore(p=>p.Email)
                );
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                index => index.Map<PostServiceViewModel>(x => x.AutoMap())
            );
        }
    }
}