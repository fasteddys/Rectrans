namespace Rectrans.Mvvm.Helpers
{
    /// <summary>
    /// 存储一个 <see cref="Action" /> 而不会给 Action 的所有者创建一个硬引用。
    /// 拥有者可以随时被垃圾收集。
    /// </summary>
    /// <typeparam name="T">Action 参数的类型。</typeparam>
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private Action<T>? _staticAction;

        public override string? MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.Method.Name;
                }

                return Method?.Name;
            }
        }

        public override bool IsAlive
        {
            get
            {
                if (_staticAction == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                // Non static action

                if (Reference != null)
                {
                    return Reference.IsAlive;
                }

                return false;
            }
        }

        public WeakAction(Action<T>? action, bool keepTargetAlive = false)
            : this(action?.Target, action, keepTargetAlive)
        {
        }

        public WeakAction(object? target, Action<T>? action, bool keepTargetAlive = false)
        {
            if (action == null) return;

            if (action.Method.IsStatic)
            {
                _staticAction = action;

                if (target != null)
                {
                    // 保留对目标的引用来控制 WeakAction 的生存期。
                    Reference = new(target);
                }

                return;
            }

            Method = action.Method;
            ActionReference = new(action.Target);

            LiveReference = keepTargetAlive ? action.Target : null;
            Reference = new(target);

#if DEBUG
            if (ActionReference != null
                && ActionReference.Target != null
                && !keepTargetAlive)
            {
                var type = ActionReference.Target.GetType();

                if (type.Name.StartsWith("<>")
                    && type.Name.Contains("DisplayClass"))
                {
                    System.Diagnostics.Debug.WriteLine(
                        "You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure? Check http://galasoft.ch/s/mvvmweakaction for more info.");
                }
            }
#endif
        }

        public new void Execute()
        {
            Execute(default!);
        }

        public void Execute(T parameter)
        {
            if (_staticAction != null)
            {
                _staticAction(parameter);
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                    || ActionReference != null)
                    && actionTarget != null)
                {
                    Method.Invoke(actionTarget, new object?[] { parameter });
                }
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            Execute(parameterCasted);
        }

        public new void MarkForDeletion()
        {
            _staticAction = null;
            base.MarkForDeletion();
        }
    }
}
