namespace ServerSider
{
    public abstract class TweakBase
    {
        private bool _hooked = false;

        public abstract bool allowed { get; }

        public bool Enable()
        {
            if (_hooked) return false;
            _hooked = true;

            Hook();

            return true;
        }

        public bool Disable()
        {
            if (!_hooked) return false;
            _hooked = false;

            Unhook();

            return true;
        }

        protected abstract void Hook();
        protected abstract void Unhook();

        protected static string GetExecutingMethod(int index = 0)
        {
            // +2 ∵ this method + method to check
            var caller = new System.Diagnostics.StackTrace().GetFrame(index + 2).GetMethod();
            return $"{caller.DeclaringType}::{caller.Name}";
        }
    }
}
