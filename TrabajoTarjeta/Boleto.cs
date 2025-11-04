using System;
namespace TrabajoTarjeta
{
    public class Boleto
    {
        public string Linea { get; }
        public double SaldoRestante { get; }
        public DateTime FechaEmision { get; } = DateTime.Now;
        public string TipoTarjeta { get; set; }
        public float TotalAbonado { get; set; }
        public string IdTarjeta { get; set; }


        public Boleto(string linea, Tarjeta tarjeta)
        {
            Linea = linea;
            SaldoRestante = tarjeta.Saldo;
            FechaEmision = DateTime.Now;
            TipoTarjeta = tarjeta.TipoTarjeta;
            IdTarjeta = tarjeta.Id;
        }

        public void Imprimir()
        {
            Console.WriteLine($"Boleto emitido para la línea: {Linea}");
            Console.WriteLine($"Saldo restante en la tarjeta: {SaldoRestante}");
            Console.WriteLine($"Fecha de emisión: {FechaEmision}");
            Console.WriteLine($"Tipo de tarjeta: {TipoTarjeta}");
            Console.WriteLine($"ID de la tarjeta: {IdTarjeta}");
        }
    }
}
 
