// TrabajoTarjeta/Tiempo.cs
using System;

namespace TrabajoTarjeta
{
    public static class Tiempo
    {
        public static Func<DateTime> Now { get; set; } = () => DateTime.Now;
    }
}