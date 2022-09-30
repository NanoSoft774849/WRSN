using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.graph
{
    public class ns_delegates<T>
    {

        public delegate bool _find(T item);
        public delegate void _foreach(T item);
        public delegate bool _filter(T item);
        public delegate void _foreachi(T item, int i);
        public delegate void _foreach_obj(T ite, object _obj);
        public delegate bool _orderBy(T item);
        public delegate void _foreachInRange(T item, int index);
        public delegate void _foreach_inparallel(T item1, T item2);
        public delegate T _filter_and_Edit(T item);
        public delegate bool _filterIndex(T item, int i);
        public delegate T _editor(T item);
        public delegate T filter_edit_index(T item, int i);
        public delegate bool _order(T item1, T item2);
        public delegate bool _order_indexer(T first, T second);
    }
    public class Iterator<T> : ns_delegates<T>
    {
        private List<T> _items;
        private object obj;

        public Iterator()
        {
           // List<T> m = new List<T>();
            this._items = new List<T>();
        }
        public Iterator(T[] _xitems)
        {
            this._items = new List<T>();
            this._items.AddRange(_xitems);
        }
        public Iterator(T[] _xitems, object _obj)
        {
            this._items = new List<T>();
            this._items.AddRange(_xitems);
            obj = _obj;
        }
        public Iterator(List<T> _list)
        {
            this._items = _list;
        }
       
        public Iterator<T> append(params T[] items)
        {
           
            
           
            
               this._items.AddRange(items);
            
                // this._items = items;
                return this;
        }
       public List<T> getItems()
        {
            return this._items;
        }
        // public delegate bool _filterpat(params _filter[] _filters);
        public int Count
        {
            get { return _items.Count; }
        }
        
        public T this[int i]
        {
            get { return _items[i]; }
            set { _items[i] = value; }
        }
        public T this[_find __find]
        {
            get
            {
                return this.get(__find, this[0]);
            }
            set
            {
                int i = 0;
                i = indexOf(__find);
                if(i!=-1)
                {
                    this[i] = value;
                }
            }
        }
        public Iterator<T> foreachInRange(int st, int end, _foreachInRange _foreach)
        {
            int n = this.Count;
            int i = 0;
            for (i = st; i < end % n; _foreach(this[i % n], i), i++) ;

            return this;
        }
        public Iterator<T> InParallel(T item1_def, T item2_def, T[] _items2, _foreach_inparallel _inparallel)
        {
            int n = this.Count;
            int i = 0;
            int m = _items2.Length;

            int max = m > n ? m : n;
            for (i = 0; i < max; _inparallel(i >= n ? item1_def : this[i], i >= m ? item2_def : _items2[i]), i++) ;


            return this;
        }
        public Iterator<T> InParallel(int _count, T item1_def, T item2_def, T[] _items2, _foreach_inparallel _inparallel)
        {
            int n = this.Count;
            int i = 0;
            int m = _items2.Length;

            int max = _count;
            for (i = 0; i < max; _inparallel(i >= n ? item1_def : this[i], i >= m ? item2_def : _items2[i]), i++) ;


            return this;
        }
        public Iterator<T> InterLeaveBym(int m)
        {
            m = m < 0 ? -m : m;
            int n = this.Count;
            m = m % n;
            if (m == 0) return this;
            T[] temp = new T[n];
            int k1 = (n / m) + 1;
            int k2 = (n % m);
            int _j = 0;
            for (int i = 0; i < m; i++)
            {
                k1 = (i < k2) ? (1 + (n / m)) : (n / m);
                for (int j = 0; j < k1; j++)
                {
                    int _k = (j * m) + i;
                    temp[_j] = this[_k];
                    _j++;
                }
            }


            return new Iterator<T>(temp);
        }
        public Iterator<T> RotateByn(int j, bool _right)
        {
            int i = 0;
            int n = this.Count;
            int k = 0;
            T[] _temp = new T[n];

            for (i = 0; i < n; i++)
            {
                k = (n + j + i) % (n);
                k = _right ? (n - 1 - k) : k;
                _temp[k] = this[i];
            }

            return new Iterator<T>(_temp);
        }
        public Iterator<T> order(_order __order , _order_indexer __indexer)
        {
            int i = 0, j = 0, k = 0;
            T min = this[0];
            for (i = 0; i < this.Count;i++)
            {
                T temp = this[i];
                min = temp;
                for (j = i; j < this.Count; j++) 
                {
                    min = __order(min, this[j]) ? min : this[j];
                    k = __indexer(min, this[j]) ? j : k;
                }
                this[i] = min;
                this[k] = temp;
            }


                return this;
        }
        public Iterator<T> Remove(int index)
        {
            this._items.RemoveAt(index);
            return this;

        }
        public Iterator<T> Remove(T item)
        {
            this._items.Remove(item);
            return this;
        }
        public bool IsNotEmpty()
        {
            return this._items.Count > 0;
        }
        public bool IsEmpty()
        {
            return this._items.Count <= 0;
        }
        public int indexOf(_find __find)
        {
            int i = 0;
            for( i=0;i<this.Count;i++)
            {
                if(__find(this[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        public T get(_find __find , T _default_value)
        {
            int i = 0;
            for(i=0;i<this.Count;i++)
            {
                if( __find(this[i]))
                {
                    return this[i];
                }
            }
            return _default_value;
        }
        public Iterator<T> OrderBy(_orderBy _order)
        {
            T max = this[0];
            int tempIndex = 0;
            int j = 0, i = 0;
            int len = this.Count;
            T tempv = max;
            for (i = 0; i < len; i++)
            {
                tempv = this[i];
                max = tempv;
                tempIndex = i;
                for (j = 0; j < len - i; j++)
                {
                    max = _order(this[i + j]) ? this[i + j] : max;
                    tempIndex = _order(this[i + j]) ? i + j : tempIndex;
                }

                this[i] = max;
                this[tempIndex] = tempv;
            }


            return this;
        }
        
        public Iterator<T> Foreach(_foreach _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i]), i++) ;


            return this;
        }
        public delegate bool _max(T item1, T item2);
        public delegate bool _exist(T item);
        public bool Exists(_exist __exist)
        {
            int c = this.Count;
            for(int i=0;i<c;i++)
            {
                if(__exist(this[i]))
                {
                    return true;
                }
            }
            return false;
        }
        
        public T Max(_max _max)
        {
            T max = this[0];
            int i = 0;
            for (i = 0; i < this.Count; i++)
            {
                max = _max(max, this[i]) ? max : this[i];
            }
            return max;
        }
        public Iterator<T> ForeachDo(_foreach_obj _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i], this.obj), i++) ;


            return this;
        }
        public Iterator<T> Foreach(_foreachi _for)
        {
            int i = 0, len = this.Count;
            for (i = 0; i < len; _for(this[i], i), i++) ;


            return this;
        }
        public int CountOf(_filter _fil)
        {
            int i = 0, j = 0;
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i])) j++;
            }
            return j;
        }
        public int CountOf(_filterIndex _fil)
        {
            int i = 0, j = 0;
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i], i)) j++;
            }
            return j;
        }
        public Iterator<T> concat(params T[][] args)
        {
            int len = this.Count;
            int i = 0;
            int c = args.Length;
            for (i = 0; i < c; i++)
            {
                len += args[i].Length;
            }
            T[] _concat = new T[len];

            this.Foreach((item, j) =>
            {
                _concat[j] = item;
            });
            int index = (this.Count);
            for (i = 0; i < c; i++)
            {
                Iterator<T> iter = new Iterator<T>(args[i]);
                iter.Foreach((item, k) =>
                {
                    _concat[index + k] = item;
                });
                index += (iter.Count);
            }




            return new Iterator<T>(_concat);
        }
        public Iterator<T> filter_params(params _filter[] _filters)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_filters[0]);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_isMatch(this[i], _filters))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public bool _isMatch(T item, params _filter[] _filters)
        {

            Iterator<_filter> nx_filters = new Iterator<_filter>(_filters);
            return nx_filters.filter(p => p(item)).Count == _filters.Length;
        }
        
        public Iterator<T> filter(_filterIndex _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i], i))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public Iterator<T> filter(_filter _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        
        public Iterator<T> edit(_editor _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                T item = _edit(this[i]);
                this[i] = item;
            }
            return this;
        }
       

        public Iterator<T> filterEdit(_filter _fil, filter_edit_index _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _edit(this[i], i);
                }
            }
            return this;
        }
        public Iterator<T> filterEdit(_filter _fil, filter_edit_index _if, filter_edit_index _else)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _if(this[i], i);
                    continue;
                }
                this[i] = _else(this[i], i);
            }
            return this;
        }
        public Iterator<T> filterEdit(_filter _fil, _filter_and_Edit _edit)
        {
            int i = 0;

            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    this[i] = _edit(this[i]);
                }
            }
            return this;
        }
        public Iterator<T> filter(int start, _filter _fil)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = start; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = this[i];
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public Iterator<T> filter(_filter _fil, filter_edit_index _edit)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = _edit(this[i], i);
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }

        public Iterator<T> filter(_filter _fil, _filter_and_Edit _edit)
        {
            int i = 0, j = 0;
            int c = this.CountOf(_fil);
            T[] mItems = new T[c];
            for (i = 0; i < this.Count; i++)
            {
                if (_fil(this[i]))
                {
                    mItems[j] = _edit(this[i]);
                    j++;
                }
            }
            return new Iterator<T>(mItems);
        }
        public delegate void _inter_leave(params T[] items);
        public delegate void __inter(params Iterator<T>[] __iters);
        public Iterator<T> interleave(_inter_leave interleave, params T[] items)
        {

            interleave(items);
            return null;
        }
        public static Iterator<T> Foreach_(__inter iter, params Iterator<T>[] items)
        {
            int c = items.Length;
            int i = 0;
            for (i = 0; i < c; i++)
            {
                iter(items[0], items[1]);
            }
            return null;
        }

    }

}
