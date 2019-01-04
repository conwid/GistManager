using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.WeakDelegate
{
    // https://github.com/lbugnion/mvvmlight
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        protected MethodInfo Method { get; set; }

        public bool IsStatic => _staticFunc != null;        

        public virtual string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }
                return Method.Name;
            }
        }

        protected WeakReference FuncReference { get; set; }

        protected WeakReference Reference { get; set; }

        protected WeakFunc() { }

        public WeakFunc(Func<TResult> func) : this(func == null ? null : func.Target, func)
        {
        }

        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    Reference = new WeakReference(target);
                }
                return;
            }

            Method = func.Method;
            FuncReference = new WeakReference(func.Target);
            Reference = new WeakReference(target);
        }
       
        public virtual bool IsAlive
        {
            get
            {
                if (_staticFunc == null && Reference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        public object Target => Reference?.Target;
        protected object FuncTarget => FuncReference?.Target;        
     
        public TResult Execute()
        {
            if (_staticFunc != null)
            {
                return _staticFunc();
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (Method != null && FuncReference != null && funcTarget != null)
                {
                    return (TResult)Method.Invoke(funcTarget, null);
                }
            }

            return default(TResult);
        }

        public void MarkForDeletion()
        {
            Reference = null;
            FuncReference = null;
            Method = null;
            _staticFunc = null;
        }
    }
}
