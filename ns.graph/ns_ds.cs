using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    

    public class PriorityQueue<Tp , TValue>
    {
       
        private List<PriorityValuePair<Tp, TValue>> q;
        public delegate bool __max(PriorityValuePair<Tp, TValue> item1, PriorityValuePair<Tp, TValue> item2);
        public delegate bool __min(PriorityValuePair<Tp, TValue> item1, PriorityValuePair<Tp, TValue> item2);
        public delegate bool __remove(PriorityValuePair<Tp, TValue> item);
        
        public __max Comparer;// comaper Min;
        public __min CompareMin;
       // public __max CompareMin;
        public PriorityQueue(__max _cmp)
        {
            this.q = new List<PriorityValuePair<Tp, TValue>>();
            this.Comparer = _cmp;
        }
        public PriorityQueue(__max _cmp , __min _min)
        {
            this.q = new List<PriorityValuePair<Tp, TValue>>();
            this.Comparer = _cmp;
            this.CompareMin = _min;
        }
        
        public PriorityQueue<Tp,TValue> insert(TValue obj , Tp priority)
        {
            PriorityValuePair<Tp, TValue> tp = new PriorityValuePair<Tp, TValue>(priority, obj);
            this.q.Add(tp);
            return this;
        }
        public PriorityValuePair<Tp, TValue> this[int index]
        {
            get
            {
                return this.q[index];
            }
            set
            {
                this.q[index] = value;
            }
        }
        public PriorityQueue<Tp,TValue> UpdateComparer(__max __cmp)
        {
            this.Comparer = __cmp;
            return this;
        }
        public PriorityQueue<Tp, TValue> Enqueue(TValue Value , Tp Priority)
        {
            return this.insert(Value, Priority);
        }
        public PriorityValuePair<Tp, TValue> Dequeue()
        {
            return this.Top();
        }
        public bool RemoveNode(__remove rem)
        {
            var first = this[0];
            bool found = false;
            foreach(var item in q)
            {
                if(rem(item))
                {
                    first = item;
                    found = true;
                    break;
                }
            }
            if(found)
            this.Remove(first);
            return found;
        }
        public PriorityQueue<Tp, TValue> Remove(PriorityValuePair<Tp, TValue> item)
        {
             this.q.Remove(item);
             return this;
        }
        public PriorityValuePair<Tp,TValue> First()
        {
            var first = this[0];
            this.Remove(this[0]);
            return first;
        }
        private PriorityValuePair<Tp, TValue> __peak(__max max)
        {
            int i = 0;
            int s = this.size();
            var TpMax = this[0];
            //var TpMax = d.priority;
            for (i = 0; i < s; i++)
            {
                var item = this[i];
                TpMax = max(TpMax, item) ? TpMax : item;

            }
            
            return TpMax;
        }
        private PriorityValuePair<Tp, TValue> __TopMin(__min min)
        {
            int i = 0;
            int s = this.size();
            var TpMax = this[0];
            //var TpMax = d.priority;
            for (i = 0; i < s; i++)
            {
                var item = this[i];
                TpMax = min(TpMax, item) ? TpMax : item;

            }
            this.q.Remove(TpMax);
            return TpMax;
        }
        public PriorityValuePair<Tp, TValue> TopMin()
        {
            return __TopMin((t1,t2) =>
                {
                    return this.CompareMin(t1, t2);
                });
        }
        public PriorityValuePair<Tp, TValue> TopMax()
        {
            return __TopMax((t1, t2) =>
            {
                return this.Comparer(t1, t2);
            });
        } 
        private PriorityValuePair<Tp, TValue> __TopMax(__max max)
        {
            int i = 0;
            int s = this.size();
            var TpMax = this[0];
            //var TpMax = d.priority;
            for (i = 0; i < s; i++)
            {
                var item = this[i];
                TpMax = max(TpMax, item) ? TpMax : item;

            }
            this.q.Remove(TpMax);
            return TpMax;
        }
        private PriorityValuePair<Tp,TValue> __Top(__max max)
        {
            int i = 0;
            int s = this.size();
            var TpMax = this[0];
            //var TpMax = d.priority;
            for (i = 0; i < s;i++)
            {
                var item = this[i];
                TpMax = max(TpMax, item) ? TpMax : item;

            }
            this.q.Remove(TpMax);
            return TpMax;
        }
        public PriorityValuePair<Tp, TValue> Top()
        {
            return __Top((t1,t2) =>
                {
                    return this.Comparer(t1, t2);
                });
        }
        public PriorityValuePair<Tp, TValue> Peak()
        {
            return __peak((t1, t2) =>
            {
                return this.Comparer(t1, t2);
            });
        }

        public int size()
        {
            return this.q.Count;
        }
        public bool IsEmpty()
        {
            return this.q.Count == 0;
        }
        public PriorityQueue<Tp,TValue> Foreach(Action<PriorityValuePair<Tp,TValue>> _foreach)
        {
            foreach(var item in this.q)
            {
                _foreach(item);
            }

            return this;
        }
        public List<PriorityValuePair<Tp,TValue>> ToList()
        {
            return this.q;
        }
        public delegate bool _Contains(PriorityValuePair<Tp, TValue> fx);
        public bool Contains(_Contains fxr)
        {
            if (this.IsEmpty()) return false;
            var fx = this.ToList();
            foreach(var item in fx)
            {
                if (fxr(item)) return true;
            }
            return false;
        }
        public delegate Tp __update(TValue value, int index);
        public void UpdatePriorities(__update update)
        {
            int count = this.size();
            for(int i =0;i < count; i++)
            {
                this[i].priority = update(this[i].Value, i);
            }
        }
    }
    public class PriorityValuePair<Tp, Tvalue>
    {
        public Tp priority;
        public Tvalue Value;
        public PriorityValuePair(Tp priority , Tvalue value)
        {
            this.priority = priority;
            this.Value = value;
        }
    }
    
}
