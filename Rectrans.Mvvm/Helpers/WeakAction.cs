using System.Reflection;

namespace Rectrans.Mvvm.Helpers
{

    /// <summary>
    /// 存储一个 <see cref="Action" /> 而不会给 Action 的所有者创建一个硬引用。
    /// 拥有者可以随时被垃圾收集。
    /// </summary>
    public class WeakAction
    {
        private Action? _staticAction;

        /// <summary>
        /// 获取或设置在构造函数中传递进来的 WeakAction 的方法的 <see cref="MethodInfo" /> 。
        /// </summary>
        protected MethodInfo? Method { get; set; }

        /// <summary>
        /// 获取此 WeakAction 表示的方法的名称。
        /// </summary>
        public virtual string? MethodName
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

        /// <summary>
        /// 获取或设置此 WeakAction 的操作目标的 WeakReference。
        /// 这不一定与<see cref="Reference" />相同，例如，
        /// 如果方法是匿名的。
        /// </summary>
        protected WeakReference? ActionReference { get; set; }

        /// <summary>
        /// 将 <see cref="ActionReference"/> 保存为硬引用。
        /// 这是与此实例的构造函数一起使用的，并且仅当构造函数的
        /// keepTargetAlive参数为true时。
        /// </summary>
        protected object? LiveReference { get; set; }

        /// <summary>
        /// 获取或设置在构造传递的目标的 WeakReference 。
        ///  这不一定与<see cref="ActionReference" />相同，
        ///  例如，如果方法是匿名的。
        /// </summary>
        protected WeakReference? Reference { get; set; }

        /// <summary>
        /// 获取一个值，该值指示 WeakAction 是否是静态的。
        /// </summary>
        public bool IsStatic => _staticAction != null;


        /// <summary>
        /// 初始化<see cref="WeakAction" />类的空实例。
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// 初始化<see cref="WeakAction" />类的实例。
        /// </summary>
        /// <param name="action">与此实例关联的操作。</param>
        /// <param name="keepTargetAlive">如果为真，则 Action 的目标将
        /// 保存为硬引用，这可能会导致内存泄漏。只有在操作使用闭包时，才应该
        /// 将此参数设置为 true。具体见：
        /// http://galasoft.ch/s/mvvmweakaction。 </param>
        public WeakAction(Action? action, bool keepTargetAlive = false)
            : this(action?.Target, action, keepTargetAlive)
        {
        }

        /// <summary>
        /// 初始化<see cref="WeakAction" />类的实例。
        /// </summary>
        /// <param name="target"> Action 的所有者。</param>
        /// <param name="action">与此实例关联的 Action 。</param>
        /// <param name="keepTargetAlive">如果为真，则 Action 的目标将
        /// 保存为硬引用，这可能会导致内存泄漏。只有在操作使用闭包时，才应该
        /// 将此参数设置为 true。具体见：
        /// http://galasoft.ch/s/mvvmweakaction。 </param>
        public WeakAction(object? target, Action? action, bool keepTargetAlive = false)
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

        /// <summary>
        /// 获取一个值，该值指示 Action 的所有者是否仍然活动，
        /// 或者是否已由垃圾收集器收集。
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null
                    && Reference == null
                    && LiveReference == null)
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

                if (LiveReference != null)
                {
                    return true;
                }

                if (Reference != null)
                {
                    return Reference.IsAlive;
                }

                return false;
            }
        }

        /// <summary>
        /// 获取 Action 的所有者。
        /// 该对象被存储为<see cref="WeakReference" />. 。
        /// </summary>
        public object? Target => Reference?.Target;

        /// <summary>
        /// 弱引用的所有者。
        /// </summary>
        protected object? ActionTarget
        {
            get
            {
                if (LiveReference != null)
                {
                    return LiveReference;
                }

                return ActionReference?.Target;
            }
        }

        /// <summary>
        /// 执行动作。
        /// 只有当动作的所有者仍然活着时才会发生这种情况。
        /// </summary>
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
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
                    Method.Invoke(LiveReference, null);
                }
            }
        }

        /// <summary>
        /// 将此实例存储的引用设置为 null。
        /// </summary>
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            LiveReference = null;
            Method = null;
            _staticAction = null;
        }
    }
}
