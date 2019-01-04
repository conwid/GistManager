using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistManager.Mvvm.Commands.WeakDelegate
{
    // https://github.com/lbugnion/mvvmlight
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        private Func<T, TResult> _staticFunc;

        public override string MethodName
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

        public override bool IsAlive
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

        public WeakFunc(Func<T, TResult> func) : this(func?.Target, func)
        {
        }

        public WeakFunc(object target, Func<T, TResult> func)
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

        public new TResult Execute() => Execute(default(T));        

        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }

            var funcTarget = FuncTarget;

            if (IsAlive)
            {
                if (Method != null && FuncReference != null && funcTarget != null)
                {
                    return (TResult)Method.Invoke( funcTarget, new object[] { parameter });
                }
            }

            return default(TResult);
        }

        public object ExecuteWithObject(object parameter) => Execute((T)parameter);
        
        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}
