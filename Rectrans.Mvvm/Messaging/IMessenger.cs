namespace Rectrans.Mvvm.Messaging
{
    /// <summary>
    /// Messenger 是一个允许对象交换消息的类。
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// 为邮件类型 TMessage 注册收件人。当发送相应的消息时，
        /// 将执行 action。<para> 注册收件人不会创建硬引用，
        /// 因此如果删除该收件人，就不会造成内存泄漏。</para>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="recipient"></param>
        /// <param name="action"></param>
        /// <param name="keepTargetAlive"></param>
        void Register<TMessage>(
            object recipient,
            Action<TMessage>? action,
            bool keepTargetAlive = false);

        void Register<TMessage>(
            object recipient,
            object? token,
            Action<TMessage>? action,
            bool keepTargetAlive = false);

        void Register<TMessage>(
            object recipient,
            object? token,
            bool receiveDerivedMessagesToo,
            Action<TMessage>? action,
            bool keepTargetAlive = false);

        void Register<TMessage>(
            object recipient,
            bool receiveDerivedMessagesToo,
            Action<TMessage>? action,
            bool keepTargetAlive = false);

        void Send<TMessage>(TMessage message);

        void Send<TMessage, TTarget>(TMessage message);

        void Send<TMessage>(TMessage message, object? token);

        void Unregister(object recipient);

        void Unregister<TMessage>(object recipient);

        void Unregister<TMessage>(object recipient, object? token);

        void Unregister<TMessage>(object recipient, Action<TMessage>? action);

        void Unregister<TMessage>(object recipient, object? token, Action<TMessage>? action);
    }
}