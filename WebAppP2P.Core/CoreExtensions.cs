using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WebAppP2P.Core.Blockchain;
using WebAppP2P.Core.Blockchain.Queries;
using WebAppP2P.Core.Database;
using WebAppP2P.Core.Database.Queries;
using WebAppP2P.Core.Keys;
using WebAppP2P.Core.Messages;
using WebAppP2P.Core.Messages.Queries;
using WebAppP2P.Core.Nodes;
using WebAppP2P.Core.Nodes.Queries;

namespace WebAppP2P.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IKeysValidator, KeysValidator>();
            services.AddTransient<IKeysFactory, KeysFactory>();
            services.AddTransient<IMessageValidator, MessageValidator>();
            services.AddTransient<IMessageStore, MessageStore>();
            services.AddTransient<IMessageDecryptor, MessageDecryptor>();
            services.AddTransient<IEncryptedMessageBuilder, EncryptedMessageBuilder>();
            services.AddTransient<IHashCash, HashCash>();
            services.AddDbContext<ApplicationDatabase>(options =>
            {
                options.UseSqlite("Data Source=Application.db");
            });
            services.AddTransient<INodesRepository, NodesRepository>();
            services.AddTransient<DatabaseInitializer>();
            services.AddTransient<IQueryHandler<CalculateNodeStatisticsQuery>, CalculateNodeStatisticsQueryHandler>();
            services.AddTransient<IQueryHandler<ClearNodeStatisticsQuery>, ClearNodeStatisticsQueryHandler>();
            services.AddTransient<IQueryHandler<GetBlockQuery,Database.Block>, GetBlockQueryHandler>();
            services.AddTransient<IQueryHandler<GetHeadBlockQuery, Database.Block>, GetHeadBlockQueryHandler>();
            services.AddTransient<IQueryHandler<GetMessagesQuery, IEnumerable<EncryptedMessageStore>>, GetMessagesQueryHandler>();
            services.AddTransient<IQueryHandler<AddBlockQuery, bool>, AddBlockQueryHandler>();
            services.AddTransient<IQueryHandler<MarkMainChainQuery, bool>, MarkMainChainQueryHandler>();

            services.AddTransient<IBlockchain, Blockchain.Blockchain>();
            services.AddTransient<IBlockchainCreator, BlockchainCreator>();
            services.AddTransient<IBlockchainValidator, BlockchainValidator>();
            return services;
        }
    }
}
