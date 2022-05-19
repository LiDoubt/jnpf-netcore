﻿using JNPF.Dependency;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace JNPF.EventBridge
{
    /// <summary>
    /// 内存事件存储提供器
    /// </summary>
    /// <remarks>默认实现</remarks>
    [SuppressSniffer]
    public sealed class MemoryEventStoreProvider : IEventStoreProvider
    {
        /// <summary>
        /// 事件处理程序存储对象
        /// </summary>
        private static readonly ConcurrentDictionary<string, EventHandlerMetadata> EventHandlerStore;

        /// <summary>
        /// 事件消息存储对象
        /// </summary>
        private static readonly ConcurrentDictionary<string, EventMessageMetadata> EventMessageStore;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MemoryEventStoreProvider()
        {
            EventHandlerStore = new();
            EventMessageStore = new();
        }

        /// <summary>
        /// 注册事件处理程序对象
        /// </summary>
        /// <param name="eventHandlerMetadata"></param>
        /// <returns></returns>
        public Task RegisterEventHandlerAsync(EventHandlerMetadata eventHandlerMetadata)
        {
            EventHandlerStore.TryAdd(eventHandlerMetadata.Category, eventHandlerMetadata);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据分类获取事件处理程序对象
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Task<EventHandlerMetadata> GetEventHandlerAsync(string category)
        {
            var eventMetadata = EventHandlerStore.TryGetValue(category, out var value) ? value : default;
            return Task.FromResult(eventMetadata);
        }

        /// <summary>
        /// 追加一条事件消息
        /// </summary>
        /// <param name="eventMessageMetadata"></param>
        /// <returns></returns>
        public async Task AppendEventMessageAsync(EventMessageMetadata eventMessageMetadata)
        {
            EventMessageStore.AddOrUpdate($"{eventMessageMetadata.Category}:{eventMessageMetadata.EventId}", eventMessageMetadata, (key, value) => eventMessageMetadata);
            await Event.EmitAsync(eventMessageMetadata);
        }

        /// <summary>
        /// 根据分类及事件Id获取事件消息元数据对象
        /// </summary>
        /// <param name="category"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public Task<EventMessageMetadata> GetEventMessageAsync(string category, string eventId)
        {
            var eventIdMetadata = EventMessageStore.TryGetValue($"{category}:{eventId}", out var value) ? value : default;
            return Task.FromResult(eventIdMetadata);
        }

        /// <summary>
        /// 成功执行一条消息
        /// </summary>
        /// <param name="eventMessageMetadata"></param>
        /// <returns></returns>
        public Task ExecuteSuccessfullyAsync(EventMessageMetadata eventMessageMetadata)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行一条消息失败
        /// </summary>
        /// <param name="eventMessageMetadata"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ExecuteFaildedAsync(EventMessageMetadata eventMessageMetadata, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}