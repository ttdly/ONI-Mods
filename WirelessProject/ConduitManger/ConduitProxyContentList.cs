using System;
using System.Collections.Generic;
using UnityEngine;
using static ConduitFlow;


namespace WirelessProject.ConduitManger {
    public class ConduitProxyContentList {
        public struct ListIdAndIndex {
            public int Id; 
            public int Index;
        }
        public int ProxyListId = -1;
        public List<ConduitContents> contents = new List<ConduitContents>(5);
        public Queue<int> canUseIndex = new Queue<int>(5);
        public List<ConduitUpdater> updaters = new List<ConduitUpdater>();
        public ConduitProxy proxy = null;
        private bool dirtyConduitUpdaters = true;

        public ConduitProxyContentList() {
            for (int i = 0; i < 5; i++) {
                canUseIndex.Enqueue(i);
                contents.Add(ConduitContents.Empty);
            }
        }

        public ListIdAndIndex AddConduitUpdater(Action<float> callback, ConduitFlowPriority priority = ConduitFlowPriority.Default) {
            updaters.Add(new ConduitUpdater {
                priority = priority,
                callback = callback
            });
            dirtyConduitUpdaters = true;
            return new ListIdAndIndex { Id = ProxyListId, Index = canUseIndex.Dequeue()};
        }

        public void RemoveConduitUpdater(Action<float> callback, int contentsIndex) {
            for (int i = 0; i < updaters.Count; i++) {
                if (updaters[i].callback == callback) {
                    updaters.RemoveAt(i);
                    dirtyConduitUpdaters = true;
                    canUseIndex.Enqueue(contentsIndex);
                    break;
                }
            }
        }

    }
}
