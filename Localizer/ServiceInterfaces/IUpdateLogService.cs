using Localizer.Services;

namespace Localizer.ServiceInterfaces
{
    public interface IUpdateLogService : IService
    {
        /// <summary>
        ///     Initialization works of the logger.
        /// </summary>
        /// <param name="name"></param>
        void Init(string name);

        /// <summary>
        ///     Used when something has been added.
        /// </summary>
        /// <param name="content"></param>
        void Add(object content);

        /// <summary>
        ///     Used when something has been removed.
        /// </summary>
        /// <param name="content"></param>
        void Remove(object content);

        /// <summary>
        ///     Used when something has been changed.
        /// </summary>
        /// <param name="content"></param>
        void Change(object content);
    }
}
