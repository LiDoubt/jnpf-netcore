using JNPF.Common.Extension;
using JNPF.Dependency;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JNPF.Common.Util
{
    /// <summary>
    /// 树结构帮助类
    /// </summary>
    [SuppressSniffer]
    public static class TreeUtil
    {
        /// <summary>
        /// 建造树结构
        /// </summary>
        /// <param name="allNodes">所有的节点</param>
        /// <param name="parentId">节点</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this List<T> allNodes, string parentId = "0") where T : TreeModel, new()
        {
            List<T> resData = new List<T>();
            var rootNodes = allNodes.Where(x => x.parentId == parentId || x.parentId.IsNullOrEmpty()).ToList();
            resData = rootNodes;
            resData.ForEach(aRootNode =>
            {
                aRootNode.hasChildren = HaveChildren(allNodes, aRootNode.id);
                if (aRootNode.hasChildren)
                {
                    aRootNode.children = _GetChildren(allNodes, aRootNode);
                    aRootNode.num = aRootNode.children.Count();
                }
                else
                {
                    aRootNode.isLeaf = !aRootNode.hasChildren;
                    aRootNode.children = null;
                }
            });
            return resData;
        }

        #region 私有成员

        /// <summary>
        /// 获取所有子节点
        /// </summary>
        /// <typeparam name="T">树模型（TreeModel或继承它的模型）</typeparam>
        /// <param name="nodes">所有节点列表</param>
        /// <param name="parentNode">父节点Id</param>
        /// <returns></returns>
        private static List<object> _GetChildren<T>(List<T> nodes, T parentNode) where T : TreeModel, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties().ToList();
            List<object> resData = new List<object>();
            var children = nodes.Where(x => x.parentId == parentNode.id).ToList();
            children.ForEach(aChildren =>
            {
                T newNode = new T();
                resData.Add(newNode);
                //赋值属性
                foreach (var aProperty in properties.Where(x => x.CanWrite))
                {
                    var value = aProperty.GetValue(aChildren, null);
                    aProperty.SetValue(newNode, value);
                }
                newNode.hasChildren = HaveChildren(nodes, aChildren.id);
                if (newNode.hasChildren)
                {
                    newNode.children = _GetChildren(nodes, newNode);
                }
                else
                {
                    newNode.isLeaf = !newNode.hasChildren;
                    newNode.children = null;
                }
            });
            return resData;
        }

        /// <summary>
        /// 判断当前节点是否有子节点
        /// </summary>
        /// <typeparam name="T">树模型</typeparam>
        /// <param name="nodes">所有节点</param>
        /// <param name="nodeId">当前节点Id</param>
        /// <returns></returns>
        private static bool HaveChildren<T>(List<T> nodes, string nodeId) where T : TreeModel, new()
        {
            return nodes.Exists(x => x.parentId == nodeId);
        }

        #endregion
    }

    /// <summary>
    /// 树模型基类
    /// </summary>
    public class TreeModel
    {
        /// <summary>
        /// 获取节点id
        /// </summary>
        /// <returns></returns>
        public string id { get; set; }

        /// <summary>
        /// 获取节点父id
        /// </summary>
        /// <returns></returns>
        //[JsonIgnore]
        public string parentId { get; set; }

        /// <summary>
        /// 是否有子级
        /// </summary>
        public bool hasChildren { get; set; }

        /// <summary>
        /// 设置Children
        /// </summary>
        /// <param name="children"></param>
        public List<object> children { get; set; } = new List<object>();

        /// <summary>
        /// 子节点数量
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 是否为子节点
        /// </summary>
        public bool isLeaf { get; set; } = false;
    }
}
