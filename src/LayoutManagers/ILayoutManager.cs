using System;
using Zene.Structs;

namespace Zene.GUI
{
    public interface ILayoutManager
    {
        public event EventHandler Change;

        /// <summary>
        /// Called before elements get managed.
        /// </summary>
        /// <param name="args"></param>
        public void SetupManager(LayoutArgs args);
        
        /// <summary>
        /// Called once for every element.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="layoutResult"></param>
        /// <returns></returns>
        public Box GetBounds(LayoutArgs args, Box layoutResult);
    }
}
