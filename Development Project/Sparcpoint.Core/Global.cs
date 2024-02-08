using System;

namespace Sparcpoint
{
    public static class Global
    {
        public static void Noop()
        {
            ((Action)(() => { }))();
        }
    }
}
