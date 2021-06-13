using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DSProject_ItaiG.ClassLibrary;

namespace DSProject_ItaiG.ClassLibrary
{
    internal class TimeManager<T> : IEnumerable<T>
    {
        internal TimeNode end = null;
        internal TimeNode start = null;


        public bool IsEmpty()
        {
            return start == null; //&& end == null; 
        }

        public bool EnQueue(T val)
        {
            TimeNode tmp = new TimeNode(val);
            if (IsEmpty())
            {
                start=tmp;
            }
            else
            {
                end.next = tmp;
                tmp.previous = end;
            }
            end=tmp;
            return true;
        }

        public bool DeQueue(out T removedValue)
        {
            try
            {
                removedValue = default(T);
                if (IsEmpty()) return false;

                removedValue = start.Value;
                start = start.next;
                if (start == null)
                    end.next = null;
                else
                    start.previous = null;
                return true;
            }
            catch (ArgumentException)
            {
                removedValue = default(T);
                return false;
            }
        }

        public bool MoveNodeToQueueYoung(TimeNode timeNode)
        {
            try
            {
                if (timeNode.next == null) return true;
                if (timeNode.previous == null)
                {
                    timeNode.next = start;
                    end.next = timeNode;
                    timeNode.previous = end;
                    start.previous = null;
                    timeNode.next = null;
                    timeNode = end;
                }
                else
                {
                    timeNode.next.previous = timeNode.previous;
                    timeNode.previous.next = timeNode.next;
                    end.next = timeNode;
                    timeNode.previous = end;
                    timeNode = end;
                }
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public bool RemoveNode(TimeNode timeNode)
        {
            try
            {
                if (end == null && start == null || timeNode.previous == null || (timeNode.next == null && timeNode.previous == null))
                    DeQueue(out timeNode.Value);

                if (timeNode.next == null)
                {
                    timeNode.previous = end;
                    timeNode.previous.next = null;
                    timeNode.previous = null;
                    return true;
                }
                if (timeNode.previous == null)
                {
                    timeNode.next = start;
                    timeNode.next.previous = null;
                    timeNode.next = null;
                    return true;
                }
                else
                {
                    timeNode.next.previous = timeNode.previous;
                    timeNode.previous.next = timeNode.next;
                    timeNode.next = null;
                    timeNode.previous = null;
                    return true;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void PrintAll()
        {
            TimeNode tmp = start;
            while (tmp != null)
            {
                try
                {
                    Console.WriteLine(tmp.Value);
                    tmp = tmp.next;
                }
                catch (ArgumentException)
                {

                }
            }
        }

        public bool GetFirst(out T value)
        {
            value = default(T);

            if (!IsEmpty())
            {
                value = start.Value;
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            TimeNode tmp = start;
            while (tmp != null)
            {
                yield return tmp.Value;
                tmp = tmp.next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }

        internal class TimeNode
        {
            public T Value;
            public TimeNode previous;
            public TimeNode next;

            public TimeNode(T Value)
            {
                this.Value = Value;
                previous = null;
                next = null;
            }
        }
    }
}

