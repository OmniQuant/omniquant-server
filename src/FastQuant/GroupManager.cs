using System.Collections.Generic;
using System.Threading;

namespace FastQuant
{
    public class GroupManager
    {
        private Framework framework;
        private int counter;

        public List<Group> Groups { get; } = new List<Group>();

        public IdArray<Group> GroupById { get; } = new IdArray<Group>();

        public GroupManager(Framework framework)
        {
            this.framework = framework;
        }

        public void Clear()
        {
            GroupById.Clear();
            Groups.Clear();
            this.counter = 0;
        }

        public void Add(Group group)
        {
            group.Id = Interlocked.Increment(ref this.counter);
            GroupById.Add(group.Id, group);
            Groups.Add(group);
            group.Framework = this.framework;
            this.framework.EventServer.OnLog(group);
        }

        internal void OnGroupEvent(GroupEvent e)
        {
            // noop
        }

        internal void OnGroup(Group e)
        {
            // noop
        }
    }
}