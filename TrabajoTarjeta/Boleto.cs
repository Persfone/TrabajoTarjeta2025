
using System;
using System.Globalization;

namespace TrabajoTarjeta
{
    public class Boleto
    {
        public string Linea { get; }
        public double SaldoRestante { get; }
        public DateTime FechaEmision { get; }
        public string TipoTarjeta { get; set; }
        public float TotalAbonado { get; set; }
        public string IdTarjeta { get; set; }

        public Boleto(string linea, Tarjeta tarjeta)
        {
            Linea = linea;
            SaldoRestante = tarjeta.Saldo;
            FechaEmision = Tiempo.Now();
            TipoTarjeta = tarjeta.TipoTarjeta;
            IdTarjeta = tarjeta.Id;
        }

        public void Imprimir()
        {
            var culture = CultureInfo.InvariantCulture;
            Console.WriteLine($"Boleto emitido para la línea: {Linea}");
            Console.WriteLine($"Saldo restante en la tarjeta: {SaldoRestante.ToString("F2", culture)}");
            Console.WriteLine($"Fecha de emisión: {FechaEmision:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Tipo de tarjeta: {TipoTarjeta}");
            Console.WriteLine($"ID de la tarjeta: {IdTarjeta}");
        }
    }
}