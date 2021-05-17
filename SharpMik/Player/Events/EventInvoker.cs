using SharpMik.Player.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpMik.Player.Events
{
    public static class EventInvoker
    {
        public delegate void RowEventArgsDelegate(object sender, RowEventArgs rowEventArgs);
        public static event RowEventArgsDelegate OnNewRow;

        public static void RaiseOnNewRow(object sender, Row row)
        {
            OnNewRow?.Invoke(sender, new RowEventArgs(row));
        }
    }
}
